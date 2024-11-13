using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Logic.Validation;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.UnitTests.Logic.Validation;

[TestFixture]
internal class SubmitPaymentRequestValidatorTests
{
    private const string Currency = "Currency";
    private const string ValidationError1 = "ValidationError1";
    private const string ValidationError2 = "ValidationError2";
    private const string ValidationError3 = "ValidationError3";

    private static readonly SubmitPaymentRequest Request = new()
    {
        Amount = 1,
        CardNumber = "CardNumber",
        Currency = Currency,
        Cvv = "Cvv",
        ExpiryMonth = 2,
        ExpiryYear = 3,
    };
    private static List<string> ValidationErrors1 = [ValidationError1];
    private static List<string> ValidationErrors2 = [ValidationError2];
    private static List<string> ValidationErrors3 = [ValidationError3];

    private MockRepository _repository;
    private Mock<IValidator<SubmitPaymentRequest>> _modelValidator;
    private Mock<IValidator<SubmitPaymentRequest>> _futureMonthValidator;
    private Mock<IValidator<string>> _currencyCodeValidator;

    private SubmitPaymentRequestValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockRepository(MockBehavior.Strict);
        _modelValidator = _repository.Create<IValidator<SubmitPaymentRequest>>();
        _futureMonthValidator = _repository.Create<IValidator<SubmitPaymentRequest>>();
        _currencyCodeValidator = _repository.Create<IValidator<string>>();

        _validator = new SubmitPaymentRequestValidator(
            _modelValidator.Object,
            _futureMonthValidator.Object,
            _currencyCodeValidator.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Verify();
    }

    [Test]
    [Description("A request which passes all validators should be valid.")]
    public void TestValidate_Valid()
    {
        SetUpValidateModel(true);
        SetUpValidateFutureMonth(true);
        SetUpValidateCurrencyCode(true);

        var result = _validator.Validate(Request, out _);

        Assert.That(result, Is.True);
    }

    [TestCase(false, true, true)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(false, false, true)]
    [TestCase(false, false, false)]
    [Description("A request which fails any validators should be invalid.")]
    public void TestValidate_Invalid(bool validModel, bool validMonth, bool validCurrency)
    {
        SetUpValidateModel(validModel);
        SetUpValidateFutureMonth(validMonth);
        SetUpValidateCurrencyCode(validCurrency);

        var result = _validator.Validate(Request, out _);

        Assert.That(result, Is.False);
    }

    [Test]
    [Description("All validation errors should be collated.")]
    public void TestValidate_ValidationErrors()
    {
        SetUpValidateModel(true);
        SetUpValidateFutureMonth(true);
        SetUpValidateCurrencyCode(true);

        _validator.Validate(Request, out var validationErrors);

        Assert.That(validationErrors, Has.Exactly(3).Items);
        Assert.That(validationErrors, Contains.Item(ValidationError1));
        Assert.That(validationErrors, Contains.Item(ValidationError2));
        Assert.That(validationErrors, Contains.Item(ValidationError3));
    }

    private void SetUpValidateModel(bool result)
    {
        _modelValidator
            .Setup(x => x.Validate(Request, out ValidationErrors1))
            .Returns(result)
            .Verifiable();
    }

    private void SetUpValidateFutureMonth(bool result)
    {
        _futureMonthValidator
            .Setup(x => x.Validate(Request, out ValidationErrors2))
            .Returns(result)
            .Verifiable();
    }

    private void SetUpValidateCurrencyCode(bool result)
    {
        _currencyCodeValidator
            .Setup(x => x.Validate(Currency, out ValidationErrors3))
            .Returns(result)
            .Verifiable();
    }
}
