using IdentityServer;
using IdentityServer.Configuration;
using IdentityServer.Database;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(Config).Assembly.GetName().Name;

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
    //options.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

// Register dynamic provider services
builder.Services.AddScoped<IDynamicProviderService, DynamicProviderService>();
builder.Services.AddScoped<DynamicAuthenticationSchemeService>();

// Register dynamic OIDC options configuration
builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, DynamicOidcOptionsConfiguration>();

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    options.EmitStaticAudienceClaim = true;
}).AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
    opt => opt.MigrationsAssembly(migrationsAssembly)))
  .AddOperationalStore(options => options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
    opt => opt.MigrationsAssembly(migrationsAssembly)))
  //.AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString,
  //  opt => opt.MigrationsAssembly(migrationsAssembly)))
  //.AddOperationalStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString,
  //  opt => opt.MigrationsAssembly(migrationsAssembly)))
  .AddAspNetIdentity<IdentityUser>();
  //.AddTestUsers(Config.Users);

// Configure authentication with support for dynamic providers
var authBuilder = builder.Services.AddAuthentication();

// Dynamic OIDC providers will be registered on-demand through DynamicOidcOptionsConfiguration
// This is a placeholder registration that enables the OpenIdConnect authentication handler
// The actual configuration for each scheme comes from the database
authBuilder.AddOpenIdConnect("dynamic-oidc", options => { });

builder.Services.AddSerilog((ctx, lc) =>
{
    lc.MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspnetCore.Authentication", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
    .Enrich.FromLogContext();
});

var app = builder.Build();

app.UseIdentityServer();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();
app.MapControllers();

if (args.Contains("/seed"))
{
    Log.Information("Seeding database ...");
    SeedData.EnsureSeedData(app);
    Log.Information("Done seeding database.");
    
    if (args.Contains("/seedproviders"))
    {
        Log.Information("Seeding dynamic providers ...");
        DynamicProviderSeedData.EnsureSeedData(app);
        Log.Information("Done seeding dynamic providers.");
    }
    
    Log.Information("Exiting.");
    return;
}

app.Run();
