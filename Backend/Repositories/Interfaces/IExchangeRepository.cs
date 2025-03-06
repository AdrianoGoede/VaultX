namespace VaultX.Repositories.Interfaces
{
    public interface IExchangeRepository
    {
        public Task<double> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    }
}