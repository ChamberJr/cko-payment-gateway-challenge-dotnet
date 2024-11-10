using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Externals;

public class BankSubmitPaymentRequest
{
    // TODO: Switch this to snakecase setting in the Serializer
    [JsonPropertyName("card_number")]
    public required string CardNumber { get; init; }
    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; init; }
    public string Currency { get; init; }
    public long Amount { get; init; }
    public string Cvv { get; init; }
}
