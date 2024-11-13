using Moq;
using Moq.Protected;
using NUnit.Framework;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Externals;

namespace PaymentGateway.Api.Tests.UnitTests.Externals;

[TestFixture]
// We would like to test when the response is null and when deserialization fails, but these are difficult to mock fully
// and the real implementations don't allow null returns already.
internal class BankPaymentSubmitterTests
{
    private const string Url = "http://url.com";
    private static readonly BankSubmitPaymentRequest BankSubmitPaymentRequest = new()
    {
        Amount = 1,
        CardNumber = "CardNumber",
        Currency = "Currency",
        Cvv = "Cvv",
        ExpiryDate = "ExpiryDate"
    };

    private MockRepository _repository;
    private Mock<IHttpClientFactory> _httpClientFactory;
    private Mock<HttpMessageHandler> _httpMessageHandler;
    private HttpClient _httpClient;

    private BankPaymentSubmitter _submitter;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockRepository(MockBehavior.Strict);
        _httpClientFactory = _repository.Create<IHttpClientFactory>();
        _httpMessageHandler = _repository.Create<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandler.Object);

        _submitter = new BankPaymentSubmitter(_httpClientFactory.Object, Url);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Verify();
    }

    [Test]
    [Description("A failure response should raise an exception.")]
    public void TestSubmit_FailureResponse()
    {
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.InternalServerError,
            Content = new StringContent("{}")
        };

        SetUpCreateClient();
        SetUpSendAsync(response);

        var exception = Assert.ThrowsAsync<PaymentProcessingException>(async () => await _submitter.Submit(BankSubmitPaymentRequest));

        Assert.That(exception, Has.Message.EqualTo("Request to bank was not successful."));
    }

    [Test]
    [Description("A successful request to the bank with a parsable message should return the message.")]
    public async Task TestSubmit_Success()
    {
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent("""{"authorized": true, "authorization_code": "code"}""")
        };

        SetUpCreateClient();
        SetUpSendAsync(response);

        var result = await _submitter.Submit(BankSubmitPaymentRequest);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Authorized, Is.True);
        Assert.That(result.AuthorizationCode, Is.EqualTo("code"));
    }

    private void SetUpCreateClient()
    {
        _httpClientFactory
            .Setup(x => x.CreateClient("BankPaymentSubmitterClient"))
            .Returns(_httpClient)
            .Verifiable();
    }

    private void SetUpSendAsync(HttpResponseMessage result)
    {
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(nameof(_httpClient.SendAsync), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(result)
            .Verifiable();
    }
}
