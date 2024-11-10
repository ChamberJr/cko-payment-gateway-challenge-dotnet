using System.Text.Json;

namespace PaymentGateway.Api.Externals;

public interface IBankPaymentSubmitter
{
    Task<BankSubmitPaymentResponse> Submit(BankSubmitPaymentRequest request);
}

public class BankPaymentSubmitter(string url) : IBankPaymentSubmitter
{
    private static readonly HttpClient HttpClient = new();

    public async Task<BankSubmitPaymentResponse> Submit(BankSubmitPaymentRequest request)
    {
        var response = await HttpClient.PostAsJsonAsync(url, request, CancellationToken.None);

        if (response == null || !response.IsSuccessStatusCode)
        {
            // TODO - make this custom
            throw new InvalidOperationException();
        }

        var content = await response.Content.ReadAsStringAsync();

        var parsedContent = JsonSerializer.Deserialize<BankSubmitPaymentResponse>(content);

        if (parsedContent == null)
        {
            // TODO - make this custom
            throw new InvalidOperationException();
        }

        return parsedContent;
    }
}