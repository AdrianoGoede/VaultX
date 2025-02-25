using VaultX.Database.Models;
using VaultX.Services.Models;

namespace VaultX.Services.Interfaces
{
    public interface ICustomerAreaService
    {
        public IQueryable<Customer> GetCustomerData(string authToken);
        public IQueryable<Account> GetAccounts(string authToken);
        public Task<AuthenticationResponse> LoginAsync(AuthenticationRequest loginRequest);
        public Task<StandardOperationResponse> LogoutAsync(string token); 
        public Task<StandardOperationResponse> CreateCustomerAsync(CustomerOperationRequest newCustomer);
        public Task<StandardOperationResponse> UpdateCustomerAsync(CustomerOperationRequest updatedCustomer, string accessToken);
        public Task<StandardOperationResponse> CreateAccountAsync(AccountOperationRequest newAccount, string accessToken);
        public Task<StandardOperationResponse> CreateTransferenceAsync(TransferenceOperationRequest newTransaction, string accessToken);
        public Task<StandardOperationResponse> CreateDepositAsync(DepositOperationRequest newDeposit);
    }
}