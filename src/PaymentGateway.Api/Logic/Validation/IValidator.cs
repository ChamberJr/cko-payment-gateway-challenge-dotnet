namespace PaymentGateway.Api.Logic.Validation;
public interface IValidator<in T>
{
    public bool Validate(T model, out List<string> validationErrors);
}