namespace PaymentGateway.Api.Exceptions;

public class PaymentProcessingException : Exception
{
    public PaymentProcessingException() : base() { }

    public PaymentProcessingException(string message) : base(message) { }

    public PaymentProcessingException(string message, Exception innerException) : base(message, innerException) { }
}
