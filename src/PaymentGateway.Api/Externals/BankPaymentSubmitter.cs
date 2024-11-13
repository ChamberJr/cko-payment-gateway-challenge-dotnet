using System.Text.Json;

using PaymentGateway.Api.Exceptions;

namespace PaymentGateway.Api.Externals;

public interface IBankPaymentSubmitter
{
    Task<BankSubmitPaymentResponse> Submit(BankSubmitPaymentRequest request);
}

public class BankPaymentSubmitter(IHttpClientFactory httpClientFactory, string url) : IBankPaymentSubmitter
{
    public async Task<BankSubmitPaymentResponse> Submit(BankSubmitPaymentRequest request)
    {
        var httpClient = httpClientFactory.CreateClient("BankPaymentSubmitterClient");

        var response = await httpClient.PostAsJsonAsync(url, request, CancellationToken.None);

        if (response == null || !response.IsSuccessStatusCode)
        {
            throw new PaymentProcessingException("Request to bank was not successful.");
        }

        var content = await response.Content.ReadAsStringAsync();

        var parsedContent = JsonSerializer.Deserialize<BankSubmitPaymentResponse>(content);

        if (parsedContent == null)
        {
            throw new PaymentProcessingException("Request to bank was successful but was unable to parse the response data and could not store it.");
        }

        return parsedContent;
    }
}