using Newtonsoft.Json;
using VaultX.Repositories.Interfaces;
using VaultX.Repositories.Models;

namespace VaultX.Repositories.Implementations
{
    public class ExchangeRepository : IExchangeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        private IEnumerable<CoinListRecord> _coinGeckoCoinList = [];
        private IEnumerable<string> _coinGeckoSupportedVsCurrencies = [];
        private DateTime _lastDataFetch = DateTime.MinValue;

        public ExchangeRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }
        
        public async Task<double> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            if ((DateTime.UtcNow - _lastDataFetch).TotalMinutes >= _configuration.GetValue<int>("ExchangeRates:Cryptocurrencies:UpdateInterval"))
                await GetSupportedCurrenciesListAsync();

            fromCurrency = fromCurrency.ToLower().Trim();
            toCurrency = toCurrency.ToLower().Trim();
            
            if (!_coinGeckoCoinList.Any(it => it.Id == fromCurrency))
                throw new ArgumentException($"Exchange from currency '{fromCurrency}' not available");
            if (!_coinGeckoSupportedVsCurrencies.Contains(toCurrency))
                throw new ArgumentException($"Exchange to currency '{toCurrency}' not available");

            return await FetchExchangeRateAsync(fromCurrency, toCurrency);
        }

        private async Task<double> FetchExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            using (var client = _httpClientFactory.CreateClient("CoinGecko"))
            {
                using (var response = await client.GetAsync($"simple/price?ids={fromCurrency}&vs_currencies={toCurrency}"))
                {
                    response.EnsureSuccessStatusCode();
                    var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                    var value = result?[fromCurrency][toCurrency] ?? throw new Exception("Error when fetching exchange rate");
                    return (double)value;
                }
            }
        }

        private async Task GetSupportedCurrenciesListAsync()
        {
            using (var client = _httpClientFactory.CreateClient("CoinGecko"))
            {
                using (var response = await client.GetAsync("coins/list?include_platform=false&status=active"))
                {
                    response.EnsureSuccessStatusCode();
                    _coinGeckoCoinList = (JsonConvert.DeserializeObject<IEnumerable<CoinListRecord>>(await response.Content.ReadAsStringAsync()) ?? []);
                }

                using (var response = await client.GetAsync("simple/supported_vs_currencies"))
                {
                    response.EnsureSuccessStatusCode();
                    _coinGeckoSupportedVsCurrencies = (JsonConvert.DeserializeObject<IEnumerable<string>>(await response.Content.ReadAsStringAsync()) ?? []);
                }

                _lastDataFetch = DateTime.UtcNow;
            }
        }
    }
}