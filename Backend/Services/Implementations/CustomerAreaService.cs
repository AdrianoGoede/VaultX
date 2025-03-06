using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using VaultX.Database;
using VaultX.Database.Models;
using VaultX.Extensions;
using VaultX.Repositories.Interfaces;
using VaultX.Services.Interfaces;
using VaultX.Services.Models;

namespace VaultX.Services.Implementations
{
    public class CustomerAreaService : ICustomerAreaService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IExchangeRepository _exchangeRepository;

        public CustomerAreaService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, IExchangeRepository exchangeRepository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _exchangeRepository = exchangeRepository ?? throw new ArgumentNullException(nameof(exchangeRepository));
        }

        public IQueryable<Customer> GetCustomerData(string authToken)
        {
            try
            {
                var customerId = GetCustomerIdFromAuthTokenAsync(authToken).Result;
                var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>();
                return dbContext.Customers.Where(it => it.Id == customerId).AsQueryable();
            }
            catch { return Array.Empty<Customer>().AsQueryable(); }
        }

        public IQueryable<Account> GetAccounts(string authToken)
        {
            try
            {
                var customerId = GetCustomerIdFromAuthTokenAsync(authToken).Result;
                var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>();
                return dbContext.Accounts.Where(it => it.CustomerId == customerId).AsQueryable();
            }
            catch { return Array.Empty<Account>().AsQueryable(); }
        }
        
        public async Task<AuthenticationResponse> LoginAsync(AuthenticationRequest loginRequest)
        {
            try
            {
                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    var customer = await dbContext.Customers.Where(it => it.Id == loginRequest.CustomerId).Select(it => new {
                        it.PasswordHash,
                        it.PasswordSalt
                    }).SingleOrDefaultAsync();
                    if (customer is null)
                        throw new ArgumentException($"Incorrect Customer ID or password, please try again!");

                    var inputPasswordHash = $"{loginRequest.Password}{customer.PasswordSalt}".GetStringHash();
                    if (inputPasswordHash != customer.PasswordHash)
                        throw new ArgumentException($"Incorrect Customer ID or password, please try again!");

                    var newToken = new AccessToken {
                        Token = SecurityExtensions.GetRandomString().GetStringHash(),
                        CustomerId = loginRequest.CustomerId,
                        IssuedAt = DateTime.UtcNow,
                        ValidUntil = DateTime.UtcNow.AddSeconds(_configuration.GetValue<int>("Authentication:TokenValidity"))
                    };
                    dbContext.AccessTokens.Add(newToken);
                    await dbContext.SaveChangesAsync();

                    return new AuthenticationResponse { Token = newToken.Token, ValidUntil = newToken.ValidUntil };
                }
            }
            catch (Exception ex) { return new AuthenticationResponse { Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        public async Task<StandardOperationResponse> LogoutAsync(string token)
        {
            try
            {
                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    var accessToken = await dbContext.AccessTokens.SingleOrDefaultAsync(it => it.Token == token.ToUpper().Trim());
                    if (accessToken is null)
                        throw new UnauthorizedAccessException("Access Denied!");
                    dbContext.AccessTokens.Remove(accessToken);
                    await dbContext.SaveChangesAsync();
                    return new StandardOperationResponse { Success = true, Message = "Logout Successful!" };
                }
            }
            catch (Exception ex) { return new StandardOperationResponse { Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        public async Task<StandardOperationResponse> CreateCustomerAsync(CustomerOperationRequest newCustomer)
        {
            try
            {
                if (newCustomer is null)
                    throw new ArgumentException("Request body cannot be empty");
                if (string.IsNullOrWhiteSpace(newCustomer.FirstName))
                    throw new ArgumentException("Inform the first name");
                if (string.IsNullOrWhiteSpace(newCustomer.LastName))
                    throw new ArgumentException("Inform the last name");
                if (newCustomer.BirthDate is null || newCustomer.BirthDate == DateOnly.MinValue)
                    throw new ArgumentException("Inform the birth date");
                if (string.IsNullOrWhiteSpace(newCustomer.Password))
                    throw new ArgumentException("Inform the password");

                var salt = SecurityExtensions.GetRandomString().GetStringHash();
                var customer = new Customer {
                    FirstName = newCustomer.FirstName.Trim(),
                    LastName = newCustomer.LastName.Trim(),
                    BirthDate = (newCustomer.BirthDate ?? DateOnly.MinValue),
                    JoinedAt = Date.Now,
                    PasswordHash = SecurityExtensions.GetStringHash($"{newCustomer.Password}{salt}"),
                    PasswordSalt = salt,
                    IsActive = true
                };

                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    dbContext.Add(customer);
                    await dbContext.SaveChangesAsync();
                }

                return new StandardOperationResponse { Id = customer.Id, Success = true, Message = "Customer created successfuly!" };
            }
            catch (Exception ex) { return new StandardOperationResponse { Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        public async Task<StandardOperationResponse> UpdateCustomerAsync(CustomerOperationRequest updatedCustomer, string accessToken)
        {
            try
            {
                var customerId = await GetCustomerIdFromAuthTokenAsync(accessToken);
                if (updatedCustomer is null)
                    throw new ArgumentNullException("Request body cannot be empty");

                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    var existingCustomer = await dbContext.Customers.SingleOrDefaultAsync(it => it.Id == customerId) ?? throw new ArgumentException($"Customer with ID {customerId} not found");

                    existingCustomer.FirstName = updatedCustomer.FirstName ?? existingCustomer.FirstName;
                    existingCustomer.LastName = updatedCustomer.LastName ?? existingCustomer.LastName;
                    existingCustomer.BirthDate = updatedCustomer.BirthDate ?? existingCustomer.BirthDate;

                    if (!string.IsNullOrWhiteSpace(updatedCustomer.Password))
                    {
                        existingCustomer.PasswordSalt = SecurityExtensions.GetRandomString().GetStringHash();
                        existingCustomer.PasswordHash = SecurityExtensions.GetStringHash($"{updatedCustomer.Password}{existingCustomer.PasswordSalt}");
                    }

                    await dbContext.SaveChangesAsync();
                }

                return new StandardOperationResponse { Id = customerId, Success = true, Message = "Changes saved!" };
            }
            catch (Exception ex)
            {
                return new StandardOperationResponse { Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") };
            }
        }

        public async Task<StandardOperationResponse> CreateAccountAsync(AccountOperationRequest newAccount, string accessToken)
        {
            try
            {
                if (newAccount is null)
                    throw new ArgumentNullException("Request body cannot be empty");
                if (string.IsNullOrWhiteSpace(newAccount.CurrencyCode))
                    throw new ArgumentException("Inform the currency code");

                var account = new Account {
                    CustomerId = await GetCustomerIdFromAuthTokenAsync(accessToken),
                    CurrencyCode = newAccount.CurrencyCode.Trim(),
                    Balance = 0d,
                    IsActive = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                };

                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    dbContext.Accounts.Add(account);
                    await dbContext.SaveChangesAsync();
                }

                return new StandardOperationResponse { Id = account.Id, Success = true, Message = "Account successfuly created" };
            }
            catch (Exception ex) { return new StandardOperationResponse { Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        public async Task<StandardOperationResponse> CreateTransferenceAsync(TransferenceOperationRequest newTransaction, string accessToken)
        {
            try
            {
                if (newTransaction is null)
                    throw new ArgumentNullException("Request body cannot be empty");
                if (newTransaction.OriginAccountId == newTransaction.DestinationAccountId)
                    throw new ArgumentException("Cannot transfer to origin account");
                if (newTransaction.Amount <= 0)
                    throw new ArgumentException("Inform the amount");
                
                var customerId = await GetCustomerIdFromAuthTokenAsync(accessToken);
                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    var originAccount = await dbContext.Accounts.SingleOrDefaultAsync(it => it.Id == newTransaction.OriginAccountId);
                    if (originAccount is null)
                        throw new ArgumentException($"Account {newTransaction.OriginAccountId} not found");
                    if (originAccount.Balance < newTransaction.Amount)
                        throw new ArgumentException("Insuficient funds");
                    if (originAccount.CustomerId != customerId)
                        throw new UnauthorizedAccessException("Access Denied!");
                    
                    var destinationAccount = await dbContext.Accounts.SingleOrDefaultAsync(it => it.Id == newTransaction.DestinationAccountId);
                    if (destinationAccount is null)
                        throw new ArgumentException($"Account {newTransaction.DestinationAccountId} not found");

                    var transference = new Transference {
                        OriginAccountId = originAccount.Id,
                        DestinationAccountId = destinationAccount.Id,
                        ConversionRate = 1, // TO DO!!
                        AmountInOriginCurrency = newTransaction.Amount,
                        AmountInDestinationCurrency = newTransaction.Amount, // TO DO!!!
                        Timestamp = DateTime.UtcNow
                    };
                    dbContext.Transactions.Add(transference);
                    originAccount.Balance -= newTransaction.Amount;
                    destinationAccount.Balance += newTransaction.Amount; // TO DO!
                    await dbContext.SaveChangesAsync();

                    return new StandardOperationResponse { Id = transference.Id, Success = true, Message = "Transference successful" };
                }
            }
            catch (Exception ex) { return new StandardOperationResponse { Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        public async Task<StandardOperationResponse> CreateDepositAsync(DepositOperationRequest newDeposit)
        {
            try
            {
                if (newDeposit is null)
                    throw new ArgumentNullException("Request body cannot be empty");
                if (string.IsNullOrWhiteSpace(newDeposit.Depositant))
                    throw new ArgumentException("Inform the depositant name");
                if (string.IsNullOrWhiteSpace(newDeposit.CurrencyCode))
                    throw new ArgumentException("Inform the currency");
                if (newDeposit.Amount <= 0)
                    throw new ArgumentException("Inform the amount");

                using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
                {
                    var account = await dbContext.Accounts.Include(it => it.Currency).SingleOrDefaultAsync(it => it.Id == newDeposit.DestinationAccountId);
                    if (account is null)
                        throw new ArgumentException($"Account {newDeposit.DestinationAccountId} not found");
                    
                    var exchangeRate = await _exchangeRepository.GetExchangeRateAsync(account.Currency.Name, newDeposit.CurrencyCode);
                    var deposit = new Deposit {
                        DestinationAccountId = newDeposit.DestinationAccountId,
                        Depositant = newDeposit.Depositant.Trim(),
                        CurrencyCode = newDeposit.CurrencyCode.Trim(),
                        ConversionRate = exchangeRate,
                        OriginalAmount = newDeposit.Amount,
                        ConvertedAmount = (newDeposit.Amount / exchangeRate),
                        Timestamp = DateTime.UtcNow
                    };
                    dbContext.Deposits.Add(deposit);
                    account.Balance += (newDeposit.Amount / exchangeRate);
                    await dbContext.SaveChangesAsync();

                    return new StandardOperationResponse { Id = deposit.Id, Success = true, Message = "Deposit successful" };
                }
            }
            catch (Exception ex) { return new StandardOperationResponse { Success = false, Message = (ex.InnerException?.Message ?? ex.Message ?? "Internal Error") }; }
        }

        private async Task<uint> GetCustomerIdFromAuthTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Access Denied!");

            using (var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>())
            {
                var accessToken = await dbContext.AccessTokens.Where(it => it.Token == token.ToUpper().Trim()).Select(it => new {
                    it.CustomerId,
                    it.ValidUntil
                }).SingleOrDefaultAsync();
                if (accessToken is null || accessToken.ValidUntil < DateTime.UtcNow)
                    throw new UnauthorizedAccessException("Access Denied!");
                return accessToken.CustomerId;
            }
        }
    }
}