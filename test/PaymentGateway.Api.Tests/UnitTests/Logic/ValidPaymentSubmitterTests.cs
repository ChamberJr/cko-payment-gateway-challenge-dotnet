using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Logic;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Externals;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Dal;
using PaymentGateway.Api.
    Enums;

namespace PaymentGateway.Api.Tests.UnitTests.Logic;

[TestFixture]
internal class ValidPaymentSubmitterTests
{
    private static readonly SubmitPaymentRequest SubmitPaymentRequest = new()
    {
        Amount = 1,
        CardNumber = "CardNumber",
        Currency = "Currency",
        Cvv = "Cvv",
        ExpiryMonth = 2,
        ExpiryYear = 3
    };
    private static readonly BankSubmitPaymentRequest BankSubmitPaymentRequest = new()
    {
        Amount = 1,
        CardNumber = "CardNumber",
        Currency = "Currency",
        Cvv = "Cvv",
        ExpiryDate = "ExpiryDate"
    };
    private static readonly BankSubmitPaymentResponse BankSubmitPaymentResponse = new()
    {
        AuthorizationCode = "AuthotizationCode",
        Authorized = true
    };
    private static readonly PaymentDetails PaymentDetails = new(
        new Guid("aaaaaaaa-6d44-4b50-a14f-7ae0beff13ad"),
        PaymentStatus.Authorized,
        "CardNumberLastFour",
        4,
        5,
        "Currency",
        6);

    private MockRepository _repository;
    private Mock<IBankPaymentSubmitter> _bankPaymentSubmitter;
    private Mock<IBankSubmitPaymentRequestCreator> _bankSubmitPaymentRequestCreator;
    private Mock<IPaymentsRepository> _paymentsRepository;
    private Mock<IPaymentDetailsCreator> _paymentDetailsCreator;

    private ValidPaymentSubmitter _submitter;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockRepository(MockBehavior.Strict);
        _bankPaymentSubmitter = _repository.Create<IBankPaymentSubmitter>();
        _bankSubmitPaymentRequestCreator = _repository.Create<IBankSubmitPaymentRequestCreator>();
        _paymentsRepository = _repository.Create<IPaymentsRepository>();
        _paymentDetailsCreator = _repository.Create<IPaymentDetailsCreator>();

        _submitter = new ValidPaymentSubmitter(
            _bankPaymentSubmitter.Object,
            _bankSubmitPaymentRequestCreator.Object,
            _paymentsRepository.Object,
            _paymentDetailsCreator.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Verify();
    }


    [Test]
    [Description("Submitting a payment should return a successful result.")]
    public async Task TestSubmitPayment()
    {
        SetUpCreateBankSubmitPaymentRequest();
        SetUpSubmit();
        SetUpCreatePaymentDetails();
        SetUpAddPayment();

        var result = await _submitter.SubmitPayment(SubmitPaymentRequest);

        Assert.That(result.Successful, Is.True);
        Assert.That(result.PaymentDetails, Is.EqualTo(PaymentDetails));
    }

    private void SetUpCreateBankSubmitPaymentRequest()
    {
        _bankSubmitPaymentRequestCreator
            .Setup(x => x.Create(SubmitPaymentRequest))
            .Returns(BankSubmitPaymentRequest)
            .Verifiable();
    }

    private void SetUpSubmit()
    {
        _bankPaymentSubmitter
            .Setup(x => x.Submit(BankSubmitPaymentRequest))
            .ReturnsAsync(BankSubmitPaymentResponse)
            .Verifiable();
    }

    private void SetUpCreatePaymentDetails()
    {
        _paymentDetailsCreator
            .Setup(x => x.Create(SubmitPaymentRequest, BankSubmitPaymentResponse))
            .Returns(PaymentDetails)
            .Verifiable();
    }

    private void SetUpAddPayment()
    {
        _paymentsRepository
            .Setup(x => x.AddPayment(PaymentDetails))
            .Verifiable();
    }
}
