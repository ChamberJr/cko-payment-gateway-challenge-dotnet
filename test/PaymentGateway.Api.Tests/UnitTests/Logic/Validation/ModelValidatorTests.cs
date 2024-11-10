using System.ComponentModel.DataAnnotations;

using NUnit.Framework;
using PaymentGateway.Api.Logic.Validation;

namespace PaymentGateway.Api.Tests.UnitTests.Logic.Validation;

internal class ModelValidatorTests
{
    private class DataClass()
    {
        [StringLength(3, MinimumLength = 3, ErrorMessage = "ErrorMessage1")]
        [RegularExpression("^[A-Z]+$", ErrorMessage = "ErrorMessage2")]
        public string? Property { get; set; }
    }

    private ModelValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new ModelValidator();
    }


    [Test]
    [Description("Valid models should be valid without errors.")]
    public void TestValidate_Valid()
    {
        var data = new DataClass()
        {
            Property = "ABC"
        };

        var result = _validator.Validate(data, out var validationErrors);

        Assert.That(result, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }


    [Test]
    [Description("Invalid models should be invalid with all error messages.")]
    public void TestValidate_Invalid()
    {
        var data = new DataClass()
        {
            Property = "aB"
        };

        var result = _validator.Validate(data, out var validationErrors);

        Assert.That(result, Is.False);
        Assert.That(validationErrors, Has.Exactly(2).Items);
        Assert.That(validationErrors, Contains.Item("ErrorMessage1"));
        Assert.That(validationErrors, Contains.Item("ErrorMessage2"));
    }
}