using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using VaultX.Services.Interfaces;
using VaultX.Services.Models;

namespace VaultX.Controllers
{
    public class CustomersController : ODataController
    {
        private readonly ICustomerAreaService _customerAreaService;

        public CustomersController(ICustomerAreaService customerAreaService)
        {
            _customerAreaService = customerAreaService ?? throw new ArgumentNullException(nameof(customerAreaService));
        }

        [HttpPost("Customer/Login")]
        public async Task<IActionResult> LoginAsync([FromBody] AuthenticationRequest loginRequest)
        {
            var response = await _customerAreaService.LoginAsync(loginRequest);
            return StatusCode((!string.IsNullOrWhiteSpace(response.Token) ? 201 : 400), response);
        }

        [HttpDelete("Customer/Logout")]
        public async Task<IActionResult> LogoutAsync([FromHeader] string AuthToken)
        {
            var response = await _customerAreaService.LogoutAsync(AuthToken);
            return StatusCode((response.Success ? 200 : 400), response);
        }

        [HttpGet("Customer/CustomerData")]
        [EnableQuery]
        public IActionResult GetCustomerData([FromHeader] string authToken) => Ok(_customerAreaService.GetCustomerData(authToken));

        [HttpGet("Customer/Accounts")]
        [EnableQuery(EnsureStableOrdering = true)]
        public IActionResult GetAccounts([FromHeader] string authToken) => Ok(_customerAreaService.GetAccounts(authToken));
        
        [HttpPost("Customer/CustomerData")]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerOperationRequest customerRequest)
        {
            var response = await _customerAreaService.CreateCustomerAsync(customerRequest);
            return StatusCode((response.Success ? 201 : 400), response);
        }

        [HttpPost("Customer/Accounts")]
        public async Task<IActionResult> CreateAccountAsync([FromBody] AccountOperationRequest newAccount, [FromHeader] string authToken)
        {
            var response = await _customerAreaService.CreateAccountAsync(newAccount, authToken);
            return StatusCode((response.Success ? 201 : 400), response);
        }

        [HttpPatch("Customer/CustomerData")]
        public async Task<IActionResult> UpdateCustomerAsync([FromBody] CustomerOperationRequest updatedCustomer, [FromHeader] string authToken)
        {
            var response = await _customerAreaService.UpdateCustomerAsync(updatedCustomer, authToken);
            return StatusCode((response.Success ? 201 : 400), response);
        }

        [HttpPost("Customer/Deposits")]
        public async Task<IActionResult> CreateDepositAsync([FromBody] DepositOperationRequest newDeposit)
        {
            var response = await _customerAreaService.CreateDepositAsync(newDeposit);
            return StatusCode((response.Success ? 201 : 400), response);
        }

        [HttpPost("Customer/Transferencies")]
        public async Task<IActionResult> CreateTransferenceAsync([FromBody] TransferenceOperationRequest newTransaction, [FromHeader] string authToken)
        {
            var response = await _customerAreaService.CreateTransferenceAsync(newTransaction, authToken);
            return StatusCode((response.Success ? 201 : 400), response);
        }
    }
}