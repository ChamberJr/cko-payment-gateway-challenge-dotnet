
using Moq;

using NUnit.Framework;

using PaymentGateway.Api.Logic;
using PaymentGateway.Api.Logic.Validation;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.UnitTests.Logic;

[TestFixture]
internal class PaymentSubmitterTests
{
    private const long Amount = 1;
    private const string CardNumber = "CardNumber";
    private const string Currency = "Currency";
    private const string Cvv = "Cvv";
    private const int Year = 2024;

    private MockRepository _repository;
    private Mock<IValidator<SubmitPaymentRequest>> _validator;
    private Mock<IPaymentSubmitter> _validPaymentSubmitter;

    private PaymentSubmitter _submitter;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockRepository(MockBehavior.Strict);
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
}
