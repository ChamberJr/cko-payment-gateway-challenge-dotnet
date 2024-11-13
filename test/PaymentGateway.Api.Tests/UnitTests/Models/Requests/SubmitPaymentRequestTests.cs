using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.UnitTests.Models.Requests;

[TestFixture]
internal class SubmitPaymentRequestTests
{
    private const long ValidAmount = 9;
    private const string ValidCardNumber = "1234567890123456789";
    private const string ValidCurrency = "ABC";
    private const string ValidCvv = "012";
    private const int ValidExpiryMonth = 5;
    private const int ValidExpiryYear = 2024;

    #region Amount

    [TestCase(0)]
    [TestCase(100)]
    [TestCase(129878)]
    [TestCase(long.MaxValue)]
    [Description("Amount should be non-negative.")]
    public void TestAmount_Valid(long amount)
    {
        var request = CreateRequest(amount: amount);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }

    [TestCase(-1)]
    [TestCase(-50)]
    [TestCase(long.MinValue)]
    [Description("Amount should be non-negative.")]
    public void TestAmount_Value_Invalid(long amount)
    {
        var request = CreateRequest(amount: amount);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("Amount must be non-negative"));
    }

    #endregion

    #region CardNumber

    [TestCase("12345678901234")]
    [TestCase("123456789012345")]
    [TestCase("1234567890123456")]
    [TestCase("12345678901234567")]
    [TestCase("123456789012345678")]
    [TestCase("1234567890123456789")]
    [Description("CardNumber should be between 14 and 19 characters long and consist of only digits.")]
    public void TestCardNumber_Valid(string cardNumber)
    {
        var request = CreateRequest(cardNumber: cardNumber);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }

    [TestCase("1")]
    [TestCase("1234567890")]
    [TestCase("1234567890123")]
    [TestCase("12345678901234567890")]
    [Description("CardNumber should be between 14 and 19 characters long and consist of only digits.")]
    public void TestCardNumber_Length_Invalid(string cardNumber)
    {
        var request = CreateRequest(cardNumber: cardNumber);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("CardNumber must be between 14 and 19 characters in length"));
    }

    [TestCase("A2345678901234")]
    [TestCase("abcdefghijklmno")]
    [TestCase("()34567890&23456")]
    [TestCase("12345678901234..@")]
    [Description("CardNumber should be between 14 and 19 characters long and consist of only digits.")]
    public void TestCardNumber_Characters_Invalid(string cardNumber)
    {
        var request = CreateRequest(cardNumber: cardNumber);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("CardNumber must consist of only the digits 0 to 9"));
    }

    #endregion

    #region Currency

    [TestCase("ABC")]
    [TestCase("QWE")]
    [Description("Currency should be 3 characters long and consist of only capital letters.")]
    public void TestCurrency_Valid(string currency)
    {
        var request = CreateRequest(currency: currency);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }

    [Test]
    [Description("Currency should be 3 characters long and consist of only capital letters.")]
    public void TestCurrency_Required_Invalid()
    {
        var request = CreateRequest(currency: "");

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Is.Not.Empty);
    }

    [TestCase("A")]
    [TestCase("AB")]
    [TestCase("ABCD")]
    [TestCase("ABCDE")]
    [Description("Currency should be 3 characters long and consist of only capital letters.")]
    public void TestCurrency_Length_Invalid(string currency)
    {
        var request = CreateRequest(currency: currency);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("Currency must be 3 characters in length"));
    }

    [TestCase("abc")]
    [TestCase("Abc")]
    [TestCase("123")]
    [TestCase("@BC")]
    [Description("Currency should be 3 characters long and consist of only capital letters.")]
    public void TestCurrency_Characters_Invalid(string currency)
    {
        var request = CreateRequest(currency: currency);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("Currency must consist of capital letters A to Z"));
    }

    #endregion

    #region Cvv

    [TestCase("123")]
    [TestCase("1234")]
    [Description("Cvv should be 3 or 4 characters long and consist of only digits.")]
    public void TestCvv_Valid(string cvv)
    {
        var request = CreateRequest(cvv: cvv);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }

    [Test]
    [Description("Cvv should be 3 or 4 characters long and consist of only digits.")]
    public void TestCvv_Required_Invalid()
    {
        var request = CreateRequest(cvv: "");

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Is.Not.Empty);
    }

    [TestCase("1")]
    [TestCase("12")]
    [TestCase("12345")]
    [Description("Cvv should be 3 or 4 characters long and consist of only digits.")]
    public void TestCvv_Length_Invalid(string cvv)
    {
        var request = CreateRequest(cvv: cvv);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("Cvv must be 3 or 4 characters in length"));
    }

    [TestCase("ABC")]
    [TestCase("abc")]
    [TestCase("12@")]
    [TestCase("i23")]
    [Description("CardNumber should be between 14 and 19 characters long and consist of only digits.")]
    public void TestCvv_Characters_Invalid(string cvv)
    {
        var request = CreateRequest(cvv: cvv);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("Cvv must consist of only the digits 0 to 9"));
    }

    #endregion

    #region ExpiryMonth

    [TestCase(1)]
    [TestCase(3)]
    [TestCase(8)]
    [TestCase(12)]
    [Description("ExpiryMonth should be a valid month from 1 to 12.")]
    public void TestExpiryMonth_Valid(int expiryMonth)
    {
        var request = CreateRequest(expiryMonth: expiryMonth);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(13)]
    [TestCase(60)]
    [Description("ExpiryMonth should be a valid month from 1 to 12.")]
    public void TestExpiryMonth_Value_Invalid(int expiryMonth)
    {
        var request = CreateRequest(expiryMonth: expiryMonth);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("ExpiryMonth must be a valid month from 1 to 12"));
    }

    #endregion

    #region ExpiryYear

    [TestCase(2024)]
    [TestCase(2030)]
    [TestCase(8888)]
    [TestCase(9999)]
    [Description("ExpiryYear should be a four-digit year, earliest being 2024.")]
    public void TestExpiryYear_Valid(int expiryYear)
    {
        var request = CreateRequest(expiryYear: expiryYear);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.True);
        Assert.That(validationErrors, Is.Empty);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(2023)]
    [TestCase(1999)]
    [TestCase(10000)]
    [Description("ExpiryYear should be a four-digit year, earliest being 2024.")]
    public void TestExpiryYear_Value_Invalid(int expiryYear)
    {
        var request = CreateRequest(expiryYear: expiryYear);

        var valid = ValidateModel(request, out var validationErrors);

        Assert.That(valid, Is.False);
        Assert.That(validationErrors, Contains.Item("ExpiryYear must be a four-digit year in the future"));
    }

    #endregion

    private static SubmitPaymentRequest CreateRequest(
        long amount = ValidAmount,
        string cardNumber = ValidCardNumber,
        string currency = ValidCurrency,
        string cvv = ValidCvv,
        int expiryMonth = ValidExpiryMonth,
        int expiryYear = ValidExpiryYear)
    {
        return new SubmitPaymentRequest
        {
            Amount = amount,
            Cvv = cvv,
            CardNumber = cardNumber,
            Currency = currency,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear };
    }

    private static bool ValidateModel(SubmitPaymentRequest model, out List<string> validationErrors)
    {
        var context = new ValidationContext(model, null, null);
        var validationResults = new List<ValidationResult>();

        var isValid =  Validator.TryValidateObject(model, context, validationResults, true);

        validationErrors = validationResults
            .Where(x => x.ErrorMessage != null)
            .Select(x => x.ErrorMessage!)
            .ToList();

        return isValid;
    }
}
