using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Logic.Validation;

public class SubmitPaymentRequestValidator(
    IValidator<SubmitPaymentRequest> modelValidator,
    IValidator<SubmitPaymentRequest> futureMonthValidator,
    IValidator<string> currencyCodeValidator) : IValidator<SubmitPaymentRequest>
{
    public bool Validate(SubmitPaymentRequest model, out List<string> validationErrors)
    {
        var modelValid = modelValidator.Validate(model, out var modelValidationErrors);
        var futureMonthValid = futureMonthValidator.Validate(model, out var futureMonthValidationErrors);
        var currencyCodeValid = currencyCodeValidator.Validate(model.Currency, out var currencyCodeValidationErrors);

        validationErrors = [];

        validationErrors.AddRange(modelValidationErrors);
        validationErrors.AddRange(futureMonthValidationErrors);
        validationErrors.AddRange(currencyCodeValidationErrors);

        return modelValid && futureMonthValid && currencyCodeValid;
    }
}

