using VaultX.Database;
using VaultX.Services.Interfaces;
using VaultX.Services.Models;
using Microsoft.EntityFrameworkCore;
using VaultX.Database.Models;

namespace VaultX.Services.Implementations
{
    public class AdminAreService : IAdminAreaService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AdminAreService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public IQueryable<Customer> GetCustomers()
        {
            var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>();
            return dbContext.Customers.AsQueryable();
        }
        
        public IQueryable<Currency> GetCurrencies()
        {
            var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>();
            return dbContext.Currencies.AsQueryable();
        }

        public async Task<CurrencyOperationResponse> CreateCurrencyAsync(CurrencyOperationRequest newCurrency)
        {
            try
            {
                if (newCurrency is null)
                    throw new ArgumentNullException("Request body cannot be empty");
                if (string.IsNullOrWhiteSpace(newCurrency.Code))
                    throw new ArgumentException("Inform the code");
                if (string.IsNullOrWhiteSpace(newCurrency.Name))
                    throw new ArgumentException("Inform the name");
                if (newCurrency.IsCrypto is null)
                    throw new ArgumentException("Inform if is cryptocurrency");

                var currency = new Currency {
                    Code = newCurrency.Code.Trim(),
                    Name = newCurrency.Name.Trim(),
                    IsCrypto = (newCurrency.IsCrypto ?? false),
                    IconUrl = newCurrency.IconUrl?.Trim()
                };

                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    dbContext.Currencies.Add(currency);
                    await dbContext.SaveChangesAsync();
                }

                return new CurrencyOperationResponse { Code = currency.Code, Success = true, Message = $"Currency {currency.Code} created successfuly!" };
            }
            catch (Exception ex) { return new CurrencyOperationResponse { Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        public async Task<CurrencyOperationResponse> UpdateCurrencyAsync(string currencyCode, CurrencyOperationRequest updatedCurrency)
        {
            try
            {
                if (updatedCurrency is null)
                    throw new ArgumentNullException("Request body cannot be empty");

                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    var existingCurrency = await dbContext.Currencies.SingleOrDefaultAsync(it => it.Code == currencyCode.Trim());
                    if (existingCurrency is null)
                        throw new ArgumentException($"Currency {currencyCode.Trim()} not found");

                    existingCurrency.Name = updatedCurrency.Name?.Trim() ?? existingCurrency.Name;
                    existingCurrency.IconUrl = updatedCurrency.IconUrl?.Trim() ?? existingCurrency.IconUrl;
                    existingCurrency.IsCrypto = updatedCurrency.IsCrypto ?? existingCurrency.IsCrypto;

                    await dbContext.SaveChangesAsync();
                }

                return new CurrencyOperationResponse { Code = currencyCode.Trim(), Success = true, Message = "Changes saved!" };
            }
            catch (Exception ex) { return new CurrencyOperationResponse { Code = currencyCode.Trim(), Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        public async Task<CurrencyOperationResponse> DeleteCurrencyAsync(string currencyCode)
        {
            try
            {
                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    var currency = await dbContext.Currencies.SingleOrDefaultAsync(it => it.Code == currencyCode.Trim());
                    if (currency is null)
                        throw new ArgumentException($"Currency {currencyCode.Trim()} not found");
                    dbContext.Currencies.Remove(currency);
                    await dbContext.SaveChangesAsync();
                }

                return new CurrencyOperationResponse { Code = currencyCode.Trim(), Success = true, Message = $"Currency {currencyCode.Trim()} deleted successfuly" };
            }
            catch (Exception ex) { return new CurrencyOperationResponse { Code = currencyCode.Trim(), Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        public async Task<StandardOperationResponse> DeleteCustomerAsync(uint customerId)
        {
            try
            {
                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    var customer = await dbContext.Customers.Include(e => e.AccessTokens).Include(e => e.Accounts).FirstOrDefaultAsync(e => e.Id == customerId);
                    if (customer is null)
                        throw new ArgumentException($"Customer with ID {customerId} not found");

                    dbContext.AccessTokens.RemoveRange(customer.AccessTokens);
                    dbContext.Accounts.RemoveRange(customer.Accounts);
                    dbContext.Customers.Remove(customer);
                    await dbContext.SaveChangesAsync();
                }

                return new StandardOperationResponse { Success = true, Message = $"User with ID {customerId} successfuly removed" };
            }
            catch (Exception ex) { return new StandardOperationResponse { Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }
    }
}