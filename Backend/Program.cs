using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VaultX;
using VaultX.Database;
using VaultX.Repositories.Implementations;
using VaultX.Repositories.Interfaces;
using VaultX.Services.Implementations;
using VaultX.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MainDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("MainDatabase"));
});

builder.Services.AddControllers().AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
}).AddOData(options => {
    options.Select().Filter().OrderBy().Expand().Count();
    options.AddRouteComponents("Admin", Startup.GetAdminAreaEdmModel());
    options.AddRouteComponents("Customer", Startup.GetCustomerAreaEdmModel());
});

builder.Services.AddHttpClient("CoinGecko", opt => {
    opt.BaseAddress = new Uri(builder.Configuration["ExchangeRates:Cryptocurrencies:ApiBaseUrl"]?.Trim());
    opt.DefaultRequestHeaders.Add("Accept", "application/json");
    opt.DefaultRequestHeaders.Add(
        builder.Configuration["ExchangeRates:Cryptocurrencies:Authentication:ApiTokenHeaderName"],
        builder.Configuration["ExchangeRates:Cryptocurrencies:Authentication:ApiTokenValue"]
    );
});

builder.Services.AddSingleton<IExchangeRepository, ExchangeRepository>();

builder.Services.AddTransient<IAdminAreaService, AdminAreService>();
builder.Services.AddTransient<ICustomerAreaService, CustomerAreaService>();

var app = builder.Build();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();