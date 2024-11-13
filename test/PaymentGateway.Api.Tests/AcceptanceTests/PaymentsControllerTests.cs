using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Externals;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PaymentGateway.Api.Tests.IntegrationTests;

[TestFixture]
public class PaymentsControllerTests
{
    #region Framework

    private const string BankUrl = "http://localhost:9734";
    private const string Id1 = "aaaaaaaa-6d44-4b50-a14f-7ae0beff13ad";
    private const string Id2 = "bbbbbbbb-6d44-4b50-a14f-7ae0beff13ad";
    private const string AuthorizationCode = "cccccccc-6d44-4b50-a14f-7ae0beff13ad";
    private const long Amount = 1;
    private const string CardNumber = "1234567890123456";
    private const string Currency = "GBP";
    private const string Cvv = "789";
    private const string CombinedDate = "05/2025";
    private const int Month = 5;
    private const int Year = 2025;

    private static readonly DateTime CurrentDateTime = new DateTime(2020, 07, 08);
    private static readonly SubmitPaymentRequest SubmitPaymentRequest = new()
    {
        Amount = Amount,
        CardNumber = CardNumber,
        Currency = Currency,
        Cvv = Cvv,
        ExpiryMonth = Month,
        ExpiryYear = Year
    };

    private MockRepository _repository;
    private Mock<Func<Guid>> _getGuid;

    private WireMockServer _server;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockRepository(MockBehavior.Strict);
        _getGuid = _repository.Create<Func<Guid>>();

        // Sometimes NUnit won't run the command to stop the server properly in the TearDown after an error -
        // so putting it in SetUp to guarentee it's run.
        _server?.Stop();
        _server = WireMockServer.Start(9734);
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        _client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(service => ServiceRegistration.Register(
                service,
                _getGuid.Object,
                () => CurrentDateTime,
                BankUrl
                )))
            .CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Verify();
    }

    #endregion

    #region Tests

    [Test]
    [Description("Users should be able to submit successful payments successfully.")]
    public async Task TestSubmitASuccessfulPaymentSuccessfully()
    {
        var response = await SubmitPaymentSuccessfulAction(true, Id1);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        await VerifyPaymentDetails(response, Id1, PaymentStatus.Authorized);
    }

    [Test]
    [Description("Users should be able to submit declined payments successfully.")]
    public async Task TestSubmitADeclinedPaymentSuccessfully()
    {
        var response = await SubmitPaymentSuccessfulAction(false, Id1);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        await VerifyPaymentDetails(response, Id1, PaymentStatus.Declined);
    }

    [Test]
    [Description("Users should not be able to submit incorrect payment details.")]
    public async Task TestSubmitsAPaymentSuccessfully()
    {
        var request = new SubmitPaymentRequest{
            Amount = Amount,
            CardNumber = "abcdefghijklmnopq",
            Currency = Currency,
            Cvv = Cvv,
            ExpiryMonth = Month,
            ExpiryYear = Year
        };

        var response = await SubmitPayment(request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var paymentResponse = await response.Content.ReadAsStringAsync();
        Assert.That(paymentResponse, Contains.Substring("CardNumber"));
    }

    [Test]
    [Description("Users should receive a 500 when the bank request errors.")]
    public async Task TestSubmitsAPaymentBankError()
    {
        SetUpBankErrorResponse();

        var response = await SubmitPayment(SubmitPaymentRequest);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    [Description("Users should receive a 500 when the bank request is not expected.")]
    public async Task TestSubmitsAPaymentBankUnexpected()
    {
        SetUpBankUnexpectedResponse();

        var response = await SubmitPayment(SubmitPaymentRequest);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    [Description("Users should be able to retrieve payment details after a succesful submission.")]
    public async Task TestGetsAPaymentSuccessfully()
    {
        await SubmitPaymentSuccessfulAction(true, Id1);
        var response = await GetPayment(Id1);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        await VerifyPaymentDetails(response, Id1, PaymentStatus.Authorized);
    }

    [Test]
    [Description("Users should receive a 404 if they attempt to retrieve non-existent payment details.")]
    public async Task TestGetsAPaymentNonExistent()
    {
        var response = await GetPayment(Id1);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Description("Users should be able to keep multiple payments stored at once.")]
    public async Task TestGetsAPaymentMultiple()
    {
        await SubmitPaymentSuccessfulAction(true, Id1);
        await SubmitPaymentSuccessfulAction(false, Id2);
        var response1 = await GetPayment(Id1);
        var response2 = await GetPayment(Id2);
        Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        await VerifyPaymentDetails(response1, Id1, PaymentStatus.Authorized);
        await VerifyPaymentDetails(response2, Id2, PaymentStatus.Declined);
    }

    #endregion

    #region Shared Business Actions

    private async Task<HttpResponseMessage> SubmitPaymentSuccessfulAction(bool authorized, string id)
    {
        var bankResponse = new BankSubmitPaymentResponse { AuthorizationCode = AuthorizationCode, Authorized = authorized };
        SetUpBankSuccessfulResponse(bankResponse);
        SetUpGetGuid(id);
        return await SubmitPayment(SubmitPaymentRequest);
    }

    private async Task<HttpResponseMessage> SubmitPayment(SubmitPaymentRequest request)
    {
        return await _client.PostAsJsonAsync("api/Payments", request);
    }

    private async Task<HttpResponseMessage> GetPayment(string id)
    {
        return await _client.GetAsync($"api/Payments/{id}");
    }

    #endregion

    #region Setup and Verification

    private async Task VerifyPaymentDetails(HttpResponseMessage response, string id, PaymentStatus authorized)
    {
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentDetails>();
        Assert.Multiple(() =>
        {
            Assert.That(paymentResponse.Id.ToString(), Is.EqualTo(id));
            Assert.That(paymentResponse.Currency, Is.EqualTo(Currency));
            Assert.That(paymentResponse.ExpiryYear, Is.EqualTo(Year));
            Assert.That(paymentResponse.ExpiryMonth, Is.EqualTo(Month));
            Assert.That(paymentResponse.Status, Is.EqualTo(authorized));
            Assert.That(paymentResponse.Amount, Is.EqualTo(Amount));
        });
    }

    private bool VerifyBankRequest(string bankSubmitPaymentRequest)
    {
        var request = JsonSerializer.Deserialize<BankSubmitPaymentRequest>(bankSubmitPaymentRequest);
        Assert.Multiple(() =>
        {
            Assert.That(request.Amount, Is.EqualTo(Amount));
            Assert.That(request.Currency, Is.EqualTo(Currency));
            Assert.That(request.ExpiryDate, Is.EqualTo(CombinedDate));
            Assert.That(request.Cvv, Is.EqualTo(Cvv));
            Assert.That(request.CardNumber, Is.EqualTo(CardNumber));
        });

        return true;
    }

    private void SetUpBankSuccessfulResponse(BankSubmitPaymentResponse response)
    {
        _server
            .Given(Request
                .Create()
                .UsingPost()
                .WithBody(VerifyBankRequest))
            .RespondWith(Response
                .Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/xml")
                .WithBody(JsonSerializer.Serialize(response)));
    }

    private void SetUpBankErrorResponse()
    {
        _server
            .Given(Request
                .Create()
                .UsingPost()
                .WithBody(VerifyBankRequest))
            .RespondWith(Response
                .Create()
                .WithStatusCode(500));
    }

    private void SetUpBankUnexpectedResponse()
    {
        _server
            .Given(Request
                .Create()
                .UsingPost()
                .WithBody(VerifyBankRequest))
            .RespondWith(Response
                .Create()
                .WithStatusCode(200)
                .WithBody("SomethingUnexpected"));
    }

    private void SetUpGetGuid(string id)
    {
        _getGuid
            .Setup(x => x())
            .Returns(new Guid(id))
            .Verifiable();
    }

    #endregion
}