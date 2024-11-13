using NUnit.Framework;
using PaymentGateway.Api.Dal;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.UnitTests.Dal;

internal class PaymentsRepositoryTests
{
    private static readonly Guid Id = new("aaaaaaaa-6d44-4b50-a14f-7ae0beff13ad");
    private static readonly PaymentDetails PaymentDetails = new(
        Id,
        PaymentStatus.Authorized,
        "CardNumberLastFour",
        4,
        5,
        "Currency",
        6);

    private PaymentsRepository _paymentsRepository;

    [SetUp]
    public void SetUp()
    {
        _paymentsRepository = new PaymentsRepository();
    }


    [Test]
    [Description("Adding a Payment should allow it be retrieved.")]
    public void TestAddPayment_TryGetPaymentDetails_Exists()
    {
        _paymentsRepository.AddPayment(PaymentDetails);
        var result = _paymentsRepository.TryGetPaymentDetails(Id, out var paymentDetails);

        Assert.That(result, Is.True);
        Assert.That(paymentDetails, Is.EqualTo(PaymentDetails));
    }

    [Test]
    [Description("Retrieving a payment which doesn't exist returns false.")]
    public void TestTryGetPaymentDetails_DoesNotExist()
    {
        var result = _paymentsRepository.TryGetPaymentDetails(Id, out var paymentDetails);

        Assert.That(result, Is.False);
        Assert.That(paymentDetails, Is.Null);
    }
}
