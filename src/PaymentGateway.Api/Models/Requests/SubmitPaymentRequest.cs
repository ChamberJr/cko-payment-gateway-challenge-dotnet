using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

public class SubmitPaymentRequest
{
    [Range(0, long.MaxValue, ErrorMessage = $"{nameof(Amount)} must be non-negative.")]
    public required long Amount { get; init; }

    [StringLength(19, MinimumLength = 14, ErrorMessage = $"{nameof(CardNumber)} must be between 14 and 19 characters in length.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = $"{nameof(CardNumber)} must consist of only the digits 0 to 9.")]
    public required string CardNumber { get; init; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = $"{nameof(Currency)} must be 3 characters in length.")]
    [RegularExpression("^[A-Z]+$", ErrorMessage = $"{nameof(Currency)} must consist of capital letters A to Z.")]
    public required string Currency { get; init; }

    [StringLength(4, MinimumLength = 3, ErrorMessage = $"{nameof(Cvv)} must be 3 or 4 characters in length.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = $"{nameof(Cvv)} must consist of only the digits 0 to 9.")]
    public required string Cvv { get; init; }

    [Range(1, 12, ErrorMessage = $"{nameof(ExpiryMonth)} must be a valid month from 1 to 12.")]
    public required int ExpiryMonth { get; init; }

    [Range(2024, 9999, ErrorMessage = $"{nameof(ExpiryYear)} must be a four-digit year in the future.")]
    public required int ExpiryYear { get; init; }
}