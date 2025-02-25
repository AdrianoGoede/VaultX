using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using VaultX.Database.Models;

namespace VaultX
{
    public static class Startup
    {
        public static IEdmModel GetAdminAreaEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<AccessToken>("AccessTokens");
            builder.EntitySet<Currency>("Currencies");
            builder.EntitySet<Account>("Accounts");

            return builder.GetEdmModel();
        }

        public static IEdmModel GetCustomerAreaEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Customer>("CustomerData");
            builder.EntitySet<Account>("Accounts");
            builder.EntitySet<Transference>("Transferences");
            builder.EntitySet<Deposit>("Deposits");
            builder.EntitySet<AccessToken>("AccessTokens");

            return builder.GetEdmModel();
        }
    }
}