using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using VaultX.Services.Interfaces;
using VaultX.Services.Models;

namespace VaultX.Controllers
{
    public class AdminController : ODataController
    {
        private readonly IAdminAreaService _adminAreaService;

        public AdminController(IAdminAreaService adminAreaService)
        {
            _adminAreaService = adminAreaService ?? throw new ArgumentNullException(nameof(adminAreaService));
        }

        [HttpGet("Admin/Customers")]
        [EnableQuery(EnsureStableOrdering = true)]
        public IActionResult GetCustomers() => Ok(_adminAreaService.GetCustomers());

        [HttpGet("Admin/Currencies")]
        [EnableQuery(EnsureStableOrdering = true)]
        public IActionResult GetCurrencies() => Ok(_adminAreaService.GetCurrencies());

        [HttpPost("Admin/Currencies")]
        public async Task<IActionResult> CreateCurrencyAsync([FromBody] CurrencyOperationRequest newCurrency)
        {
            var response = await _adminAreaService.CreateCurrencyAsync(newCurrency);
            return StatusCode((response.Success ? 201 : 400), response);
        }

        [HttpPatch("Admin/Currencies/{currencyCode}")]
        public async Task<IActionResult> UpdateCurrencyAsync([FromODataUri] string currencyCode, [FromBody] CurrencyOperationRequest updateCurrency)
        {
            var response = await _adminAreaService.UpdateCurrencyAsync(currencyCode, updateCurrency);
            return StatusCode((response.Success ? 200 : 400), response);
        }

        [HttpDelete("Admin/Currencies/{currencyCode}")]
        public async Task<IActionResult> DeleteCurrencyAsync([FromODataUri] string currencyCode)
        {
            var response = await _adminAreaService.DeleteCurrencyAsync(currencyCode);
            return StatusCode((response.Success ? 200 : 400), response);
        }

        [HttpDelete("Admin/Customers/{customerId}")]
        public async Task<IActionResult> DeleteCustomerAsync([FromODataUri] uint customerId)
        {
            var response = await _adminAreaService.DeleteCustomerAsync(customerId);
            return StatusCode((response.Success ? 200 : 400), response);
        }
    }
}