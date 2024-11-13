using NUnit.Framework;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Externals;
using PaymentGateway.Api.Logic;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.UnitTests.Logic;


[TestFixture]
internal class PaymentDetailsCreatorTests
{
    private const string AuthorizationCode = "AuthorizationCode";
    private const long Amount = 1;
    private const string CardNumber = "CardNumber";
    private const string Currency = "Currency";
    private const string Cvv = "Cvv";
    private const int Month = 12;
    private const int Year = 2024;

    private static readonly Guid Guid = new("aaaaaaaa-6d44-4b50-a14f-7ae0beff13ad");
    private static readonly SubmitPaymentRequest Request = new()
        {
            Amount = Amount,
            CardNumber = CardNumber,
            Currency = Currency,
            Cvv = Cvv,
            ExpiryMonth = Month,
            ExpiryYear = Year
        };

    private PaymentDetailsCreator _creator;

    [SetUp]
    public void SetUp()
    {
        _creator = new PaymentDetailsCreator(() => Guid);
    }


    [Test]
    [Description("Most information should be transferred without modification.")]
    public void TestCreate_GeneralInformation()
    {
        var result = _creator.Create(Request, CreateResponse(true));

        Assert.That(result.Id, Is.EqualTo(Guid));
        Assert.That(result.Amount, Is.EqualTo(Amount));
        Assert.That(result.Currency, Is.EqualTo(Currency));
        Assert.That(result.ExpiryMonth, Is.EqualTo(Month));
        Assert.That(result.ExpiryYear, Is.EqualTo(Year));
    }

    [Test]
    [Description("CardNumber should only include the last four characters.")]
    public void TestCreate_CardNumber()
    {
        var result = _creator.Create(Request, CreateResponse(true));

        Assert.That(result.CardNumberLastFour, Is.EqualTo("mber"));
    }

    [TestCase(true, ExpectedResult = PaymentStatus.Authorized)]
    [TestCase(false, ExpectedResult = PaymentStatus.Declined)]
    [Description("PaymentStatus should be mapped based on Authorized.")]
    public PaymentStatus TestCreate_PaymentStatus(bool authorized)
    {
        var result = _creator.Create(Request, CreateResponse(authorized));

        return result.Status;
    }

    private BankSubmitPaymentResponse CreateResponse(bool authorized)
    {
        return new BankSubmitPaymentResponse { AuthorizationCode = AuthorizationCode, Authorized = authorized };
    }
}

