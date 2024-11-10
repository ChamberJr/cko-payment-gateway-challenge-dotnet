using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public class PaymentDetails(Guid id, PaymentStatus status, string cardNumberLastFour, int expiryMonth, int expiryYear, string currency, long amount)
{
    public Guid Id => id;
    public PaymentStatus Status => status;
    public string CardNumberLastFour => cardNumberLastFour;
    public int ExpiryMonth => expiryMonth;
    public int ExpiryYear => expiryYear;
    public string Currency => currency;
    public long Amount => amount;
}