using System.Diagnostics.CodeAnalysis;

using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Dal;
public interface IPaymentsRepository
{
    void AddPayment(PaymentDetails payment);
    bool TryGetPaymentDetails(Guid id, [NotNullWhen(true)] out PaymentDetails? paymentDetails);
}

public class PaymentsRepository : IPaymentsRepository
{
    private readonly Dictionary<Guid, PaymentDetails> Payments = [];

    public void AddPayment(PaymentDetails payment)
    {
        Payments.Add(payment.Id, payment);
    }

    public bool TryGetPaymentDetails(Guid id, [NotNullWhen(true)] out PaymentDetails? paymentDetails)
    {
        return Payments.TryGetValue(id, out paymentDetails);
    }
}