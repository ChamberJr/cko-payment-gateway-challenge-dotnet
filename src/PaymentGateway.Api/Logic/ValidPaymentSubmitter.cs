using PaymentGateway.Api.Dal.Database;
using PaymentGateway.Api.Externals;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Logic;

public class ValidPaymentSubmitter(
    IBankPaymentSubmitter bankPaymentSubmitter,
    IBankSubmitPaymentRequestCreator bankSubmitPaymentRequestCreator,
    IPaymentsRepository paymentsRepository,
    IPaymentDetailsCreator paymentDetailsCreator) : IPaymentSubmitter
{
    public async Task<PaymentSubmissionResult> SubmitPayment(SubmitPaymentRequest request)
    {
        var bankSubmitPaymentRequest = bankSubmitPaymentRequestCreator.Create(request);
        var response = await bankPaymentSubmitter.Submit(bankSubmitPaymentRequest);
        var paymentDetails = paymentDetailsCreator.Create(request, response);
        paymentsRepository.AddPayment(paymentDetails);

        return PaymentSubmissionResult.Success(paymentDetails);
    }
}
