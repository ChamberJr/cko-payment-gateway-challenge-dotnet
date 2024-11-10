using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Logic.Validation;

public class ModelValidator : IValidator<object>
{
    public bool Validate(object model, out List<string> validationErrors)
    {

        var context = new ValidationContext(model, null, null);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(model, context, validationResults, true);

        validationErrors = validationResults
            .Where(x => x.ErrorMessage != null)
            .Select(x => x.ErrorMessage!)
            .ToList();

        return validationErrors.Count != 0;
    }
}
