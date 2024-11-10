using PaymentGateway.Api.Logic.Validation;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Logic;

public class PaymentSubmissionResult
{
    private PaymentSubmissionResult(bool successful, string validationErrorMessage, PaymentDetails? paymentDetails)
    {
        Successful = successful;
        ValidationErrorMessage = validationErrorMessage;
        PaymentDetails = paymentDetails;
    }

    public bool Successful { get; }
    public string ValidationErrorMessage { get; }
    public PaymentDetails? PaymentDetails { get; }

    public static PaymentSubmissionResult Success(PaymentDetails paymentDetails) => new(true, "", paymentDetails);
    public static PaymentSubmissionResult Failure(string validationErrorMessage) => new(false, validationErrorMessage, null);
}

public interface IPaymentSubmitter
{
    Task<PaymentSubmissionResult> SubmitPayment(SubmitPaymentRequest request);
}

public class PaymentSubmitter(
    IValidator<SubmitPaymentRequest> submitPaymentRequestValidator,
    IPaymentSubmitter validPaymentSubmitter) : IPaymentSubmitter
{
    public async Task<PaymentSubmissionResult> SubmitPayment(SubmitPaymentRequest request)
    {
        if (submitPaymentRequestValidator.Validate(request, out var validationErrors))
        {
            return await validPaymentSubmitter.SubmitPayment(request);
        }

        var validationErrorMessagePrefix = $"Payment rejected because request was invalid. ";

        var validationErrorMessagePostfix = validationErrors.Count == 1
            ? "Unable to find reason for invalid request."
            : $"Invalid reasons: {string.Join(", ", validationErrors)}";

        return PaymentSubmissionResult.Failure(validationErrorMessagePrefix + validationErrorMessagePostfix);
    }
}
