using System.Text.Json;
using ClientAppdotnet.Models;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Returns a tuple: Products and optional ErrorMessage. Caller should display errors appropriately.
    public async Task<(List<Product> Products, string? ErrorMessage)> GetProductsAsync(int timeoutSeconds = 10)
    {
        var empty = new List<Product>();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        try
        {
            using var response = await _httpClient.GetAsync("/api/products", cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                return (empty, $"Server returned {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cts.Token);
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

            var products = await JsonSerializer.DeserializeAsync<List<Product>>(stream, options, cts.Token)
                           ?? new List<Product>();

            return (products, null);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            return (empty, "Request timed out. Please try again later.");
        }
        catch (JsonException je)
        {
            return (empty, $"Invalid JSON response: {je.Message}");
        }
        catch (HttpRequestException he)
        {
            return (empty, $"Network error: {he.Message}");
        }
        catch (Exception ex)
        {
            return (empty, $"Unexpected error: {ex.Message}");
        }
    }
}