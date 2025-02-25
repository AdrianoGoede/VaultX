using VaultX.Database.Models;
using VaultX.Services.Models;

namespace VaultX.Services.Interfaces
{
    public interface IAdminAreaService
    {
        public IQueryable<Customer> GetCustomers();
        public IQueryable<Currency> GetCurrencies();
        public Task<CurrencyOperationResponse> CreateCurrencyAsync(CurrencyOperationRequest newCurrency);
        public Task<CurrencyOperationResponse> UpdateCurrencyAsync(string currencyCode, CurrencyOperationRequest updatedCurrency);
        public Task<CurrencyOperationResponse> DeleteCurrencyAsync(string currencyCode);
        public Task<StandardOperationResponse> DeleteCustomerAsync(uint customerId);
    }
}