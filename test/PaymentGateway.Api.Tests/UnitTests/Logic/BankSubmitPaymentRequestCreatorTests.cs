using NUnit.Framework;

using PaymentGateway.Api.Logic;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.UnitTests.Logic;

[TestFixture]
internal class BankSubmitPaymentRequestCreatorTests
{
    private const long Amount = 1;
    private const string CardNumber = "CardNumber";
    private const string Currency = "Currency";
    private const string Cvv = "Cvv";
    private const int Year = 2024;

    private BankSubmitPaymentRequestCreator _creator;

    [SetUp]
    public void SetUp()
    {
        _creator = new BankSubmitPaymentRequestCreator();
    }


    [Test]
    [Description("Most information should be transferred without modification.")]
    public void TestCreate_GeneralInformation()
    {
        var request = new SubmitPaymentRequest
        {
            Amount = Amount,
            CardNumber = CardNumber,
            Currency = Currency,
            Cvv = Cvv,
            ExpiryMonth = 1,
            ExpiryYear = Year
        };

        var result = _creator.Create(request);

        Assert.That(result.Amount, Is.EqualTo(Amount));
        Assert.That(result.CardNumber, Is.EqualTo(CardNumber));
        Assert.That(result.Currency, Is.EqualTo(Currency));
        Assert.That(result.Cvv, Is.EqualTo(Cvv));
    }

    [Test]
    [Description("Double digit months should be displayed as normal in ExpiryDate.")]
    public void TestCreate_DoubleDigitMonth()
    {
        var request = new SubmitPaymentRequest
        {
            Amount = Amount,
            CardNumber = CardNumber,
            Currency = Currency,
            Cvv = Cvv,
            ExpiryMonth = 11,
            ExpiryYear = Year
        };

        var result = _creator.Create(request);

        Assert.That(result.ExpiryDate, Is.EqualTo("11/2024"));
    }

    [Test]
    [Description("Single digit months should be displayed with a leading 0 in ExpiryDate.")]
    public void TestCreate_SingleDigitMonth()
    {
        var request = new SubmitPaymentRequest
        {
            Amount = Amount,
            CardNumber = CardNumber,
            Currency = Currency,
            Cvv = Cvv,
            ExpiryMonth = 3,
            ExpiryYear = Year
        };

        var result = _creator.Create(request);

        Assert.That(result.ExpiryDate, Is.EqualTo("03/2024"));
    }
}
