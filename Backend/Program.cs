using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VaultX;
using VaultX.Database;
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

builder.Services.AddTransient<IAdminAreaService, AdminAreService>();
builder.Services.AddTransient<ICustomerAreaService, CustomerAreaService>();

var app = builder.Build();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();