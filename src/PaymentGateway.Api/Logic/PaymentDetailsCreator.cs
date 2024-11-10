using PaymentGateway.Api.Externals;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Logic;

public interface IPaymentDetailsCreator
{
    PaymentDetails Create(SubmitPaymentRequest request, BankSubmitPaymentResponse response);
}

public class PaymentDetailsCreator(Func<Guid> getUniqueGuid) : IPaymentDetailsCreator
{
    public PaymentDetails Create(SubmitPaymentRequest request, BankSubmitPaymentResponse response)
    {
        var id = getUniqueGuid();
        var paymentStatus = response.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
        var cardNumberLastFour = request.CardNumber.Substring(request.CardNumber.Length - 4);

        return new PaymentDetails(id, paymentStatus, cardNumberLastFour, request.ExpiryMonth, request.ExpiryYear, request.Currency, request.Amount);
    }
}

