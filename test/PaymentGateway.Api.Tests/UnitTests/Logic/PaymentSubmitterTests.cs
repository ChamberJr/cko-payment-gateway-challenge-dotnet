
using Moq;

using NUnit.Framework;

using PaymentGateway.Api.Logic;
using PaymentGateway.Api.Logic.Validation;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.UnitTests.Logic;

[TestFixture]
internal class PaymentSubmitterTests
{
    private static readonly SubmitPaymentRequest Request = new() {
        Amount = 1,
        CardNumber = "CardNumber",
        Currency = "Currency",
        Cvv = "Cvv",
        ExpiryMonth = 2,
        ExpiryYear = 3
    };

    private static readonly PaymentSubmissionResult Result = PaymentSubmissionResult.Failure("Failure");

    private MockRepository _repository;
    private Mock<IValidator<SubmitPaymentRequest>> _validator;
    private Mock<IPaymentSubmitter> _validPaymentSubmitter;

    private PaymentSubmitter _submitter;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockRepository(MockBehavior.Strict);
        _validator = _repository.Create<IValidator<SubmitPaymentRequest>>();
        _validPaymentSubmitter = _repository.Create<IPaymentSubmitter>();

        _submitter = new PaymentSubmitter(_validator.Object, _validPaymentSubmitter.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Verify();
    }


    [Test]
    [Description("Valid requests should be submitted and returned as-is.")]
    public async Task TestSubmitPayment_ValidRequest()
    {
        SetUpValidate(true, []);
        SetUpSubmitPayment();

        var result = await _submitter.SubmitPayment(Request);

        Assert.That(result, Is.EqualTo(Result));
    }

    [Test]
    [Description("Invalid requests should be failures, and if there are no errors, this should be stated.")]
    public async Task TestSubmitPayment_InvalidRequestWithoutErrors()
    {
        SetUpValidate(false, []);

        var result = await _submitter.SubmitPayment(Request);

        Assert.That(result.Successful, Is.False);
        Assert.That(result.ValidationErrorMessage, Is.EqualTo("Payment rejected because request was invalid. Unable to find reason for invalid request."));
    }

    [Test]
    [Description("Invalid requests should be failures, and if there are errors, they should be stated.")]
    public async Task TestSubmitPayment_InvalidRequestWithErrors()
    {
        SetUpValidate(false, ["ErrorReason1", "ErrorReason2"]);

        var result = await _submitter.SubmitPayment(Request);

        Assert.That(result.Successful, Is.False);
        Assert.That(result.ValidationErrorMessage, Is.EqualTo("Payment rejected because request was invalid. Invalid reasons: ErrorReason1, ErrorReason2"));
    }

    private void SetUpValidate(bool result, List<string> validationErrors)
    {
        _validator
            .Setup(x => x.Validate(Request, out validationErrors))
            .Returns(result)
            .Verifiable();
    }

    private void SetUpSubmitPayment()
    {
        _validPaymentSubmitter
            .Setup(x => x.SubmitPayment(Request))
            .ReturnsAsync(Result)
            .Verifiable();
    }
}
