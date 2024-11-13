using Moq;
using NUnit.Framework;
using PaymentGateway.Api.Logic.Validation;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.UnitTests.Logic.Validation;

[TestFixture]
internal class FutureMonthValidatorTests
{
    private static readonly DateTime CurrentDateTime = new(2024, 06, 01);

    private MockRepository _repository;
    private Mock<Func<DateTime>> _getCurrentDateTimeDelegate;

    private FutureMonthValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockRepository(MockBehavior.Strict);
        _getCurrentDateTimeDelegate = _repository.Create<Func<DateTime>>();

        _validator = new FutureMonthValidator(_getCurrentDateTimeDelegate.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Verify();
    }

    [Test]
    [Description("A future year should be valid without errors.")]
    public void TestValidate_FutureYear()
    {
        SetUpGetCurrentDateTime(CurrentDateTime);

        var request = CreateRequest(06, 2030);

        var result = _validator.Validate(request, out var validationErrors);

        Assert.That(result, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }


    [Test]
    [Description("A past year should be invalid with an errors message")]
    public void TestValidate_PastYear()
    {
        SetUpGetCurrentDateTime(CurrentDateTime);

        var request = CreateRequest(06, 2020);

        var result = _validator.Validate(request, out var validationErrors);

        Assert.That(result, Is.False);
        Assert.That(validationErrors, Has.Exactly(1).Items);
        Assert.That(validationErrors.Single(), Is.EqualTo("Expiry Year must be in the future."));
    }

    [Test]
    [Description("A current year with a future month should be valid without errors.")]
    public void TestValidate_CurrentYear_FutureMonth([Values(6, 12)] int currentMonth)
    {
        SetUpGetCurrentDateTime(CurrentDateTime);

        var request = CreateRequest(currentMonth, 2024);

        var result = _validator.Validate(request, out var validationErrors);

        Assert.That(result, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }


    [Test]
    [Description("A current year with a past month should be invalid with an errors message")]
    public void TestValidate_CurrentYear_PastMonth()
    {
        SetUpGetCurrentDateTime(CurrentDateTime);

        var request = CreateRequest(05, 2024);

        var result = _validator.Validate(request, out var validationErrors);

        Assert.That(result, Is.False);
        Assert.That(validationErrors, Has.Exactly(1).Items);
        Assert.That(validationErrors.Single(), Is.EqualTo("Expiry Month must be in the future."));
    }

    private void SetUpGetCurrentDateTime(DateTime result)
    {
        _getCurrentDateTimeDelegate
            .Setup(x => x())
            .Returns(result)
            .Verifiable();
    }

    private SubmitPaymentRequest CreateRequest(int expiryMonth, int expiryYear)
    {
        return new SubmitPaymentRequest {
            Amount = 1,
            CardNumber = "CardNumber",
            Currency = "Currency",
            Cvv = "Cvv",
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear };

    }
}
