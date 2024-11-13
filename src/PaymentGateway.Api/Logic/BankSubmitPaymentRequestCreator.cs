using PaymentGateway.Api.Externals;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Logic;

public interface IBankSubmitPaymentRequestCreator
{
    BankSubmitPaymentRequest Create(SubmitPaymentRequest request);
}

public class BankSubmitPaymentRequestCreator : IBankSubmitPaymentRequestCreator
{
    public BankSubmitPaymentRequest Create(SubmitPaymentRequest request)
    {
        var paddedMonth = request.ExpiryMonth.ToString().Length == 1
            ? "0" + request.ExpiryMonth
            : request.ExpiryMonth.ToString();

        return new BankSubmitPaymentRequest
        {
            Amount = request.Amount,
            CardNumber = request.CardNumber,
            Currency = request.Currency,
            Cvv = request.Cvv,
            ExpiryDate = $"{paddedMonth}/{request.ExpiryYear}"
        };
    }
}
