using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Dal;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Logic;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.UnitTests.Controllers;

[TestFixture]
internal class PaymentsControllerTests
{
    private const string ValidationErrorMessage = "ValidationErrorMessage";

    private static readonly Guid Id = new("aaaaaaaa-6d44-4b50-a14f-7ae0beff13ad");
    private static PaymentDetails? PaymentDetails = new(
        Id,
        PaymentStatus.Authorized,
        "CardNumberLastFour",
        4,
        5,
        "Currency",
        6);
    private static readonly SubmitPaymentRequest Request = new()
    {
        Amount = 1,
        CardNumber = "CardNumber",
        Currency = "Currency",
        Cvv = "Cvv",
        ExpiryMonth = 2,
        ExpiryYear = 3,
    };

    private MockRepository _repository;
    private Mock<IPaymentsRepository> _paymentsRepository;
    private Mock<IPaymentSubmitter> _paymentSubmitter;

    private PaymentsController _controller;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockRepository(MockBehavior.Strict);
        _paymentsRepository = _repository.Create<IPaymentsRepository>();
        _paymentSubmitter = _repository.Create<IPaymentSubmitter>();

        _controller = new PaymentsController(_paymentsRepository.Object, _paymentSubmitter.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Verify();
    }

    [Test]
    [Description("Getting a payment should return 404 if the id doesn't exist.")]
    public void TestGetPayment_DoesNotExist()
    {
        SetUpTryGetPaymentDetails(false);
        var result = _controller.GetPayment(Id);

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    [Description("Getting a payment should return 200 with the payment details if the id does exist.")]
    public void TestGetPayment_Exists()
    {
        SetUpTryGetPaymentDetails(true);
        var result = _controller.GetPayment(Id);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var value = (PaymentDetails?)(result.Result as OkObjectResult)!.Value;
        Assert.That(value, Is.EqualTo(PaymentDetails));
    }

    [Test]
    [Description("Submitting a payment should return 400 with validation error details if submission failed.")]
    public async Task TestSubmitPayment_Failure()
    {
        var paymentResult = PaymentSubmissionResult.Failure(ValidationErrorMessage);

        SetUpSubmitPaymentRequest(paymentResult);
        var result = await _controller.SubmitPayment(Request);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var value = (string?)(result.Result as BadRequestObjectResult)!.Value;
        Assert.That(value, Is.EqualTo(ValidationErrorMessage));
    }

    [Test]
    [Description("Submitting a payment should return 200 with the payment details if submission succeeded.")]
    public async Task TestSubmitPayment_Success()
    {
        var paymentResult = PaymentSubmissionResult.Success(PaymentDetails!);

        SetUpSubmitPaymentRequest(paymentResult);
        var result = await _controller.SubmitPayment(Request);

        Assert.That(result.Result, Is.InstanceOf<CreatedAtRouteResult>());
        var createdResult = (result.Result as CreatedAtRouteResult)!;
        var value = createdResult.Value;
        Assert.That(value, Is.EqualTo(PaymentDetails));
        Assert.That(createdResult.RouteName, Is.EqualTo("Payments"));
        Assert.That(createdResult.RouteValues, Has.Exactly(1).Items);
        Assert.That(createdResult.RouteValues.Single().Key, Is.EqualTo("id"));
        Assert.That(createdResult.RouteValues.Single().Value, Is.EqualTo(Id));
    }

    private void SetUpSubmitPaymentRequest(PaymentSubmissionResult result)
    {
        _paymentSubmitter
            .Setup(x => x.SubmitPayment(Request))
            .ReturnsAsync(result)
            .Verifiable();
    }

    private void SetUpTryGetPaymentDetails(bool result)
    {
        _paymentsRepository
            .Setup(x => x.TryGetPaymentDetails(Id, out PaymentDetails))
            .Returns(result)
            .Verifiable();
    }
}
