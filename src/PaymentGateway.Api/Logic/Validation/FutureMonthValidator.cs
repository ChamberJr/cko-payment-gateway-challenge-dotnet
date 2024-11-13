using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Logic.Validation;
public class FutureMonthValidator(Func<DateTime> getCurrentDateTime) : IValidator<SubmitPaymentRequest>
{
    public bool Validate(SubmitPaymentRequest request, out List<string> validationErrors)
    {
        validationErrors = [];
        var currentDateTime = getCurrentDateTime();

        if (request.ExpiryYear > currentDateTime.Year)
        {
            return true;
        }

        if (request.ExpiryYear < currentDateTime.Year)
        {
            validationErrors.Add("Expiry Year must be in the future.");
            return false;
        }

        if (request.ExpiryMonth >= currentDateTime.Month)
        {
            return true;
        }

        validationErrors.Add("Expiry Month must be in the future.");
        return false;
    }
}

