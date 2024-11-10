using NUnit.Framework;
using PaymentGateway.Api.Logic.Validation;

namespace PaymentGateway.Api.Tests.UnitTests.Logic.Validation;

[TestFixture]
internal class CurrencyCodeValidatorTests
{
    private CurrencyCodeValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CurrencyCodeValidator();
    }


    [Test]
    [Description("Valid currency codes should be valid without errors.")]
    public void TestValidate_Valid([Values("ADA", "GBP", "USD", "LVL")] string currencyCode)
    {
        var result = _validator.Validate(currencyCode, out var validationErrors);

        Assert.That(result, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }


    [Test]
    [Description("Invalid currency codes should be invalid with an error message.")]
    public void TestValidate_Invalid([Values("ada", "gBP", "USB", "DOGE", "LINK", "LUNA", "GB", "Pounds")] string currencyCode)
    {
        var result = _validator.Validate(currencyCode, out var validationErrors);

        Assert.That(result, Is.False);
        Assert.That(validationErrors, Has.Exactly(1).Items);
        Assert.That(validationErrors.Single(), Is.EqualTo("Currency Code must be a recognised three character code, in upper case."));
    }
}
