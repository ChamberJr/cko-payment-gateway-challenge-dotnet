﻿using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Externals;

public class BankSubmitPaymentResponse
{
    [JsonPropertyName("authorized")]
    public required bool Authorized { get; init; }
    [JsonPropertyName("authorization_code")]
    public required string AuthorizationCode { get; init; }

}

