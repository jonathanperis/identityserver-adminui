using IdentityExpress.Identity;
using IdentityExpress.Manager.BusinessLogic.Configuration;
using IdentityExpress.Manager.UI.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAdminUI(options =>
{
    options.IdentityType = IdentityType.DefaultIdentity;
    options.MigrationOptions = MigrationOptions.None;
});

//// Register Identity using IdentityExpress entities and stores
//builder.Services.AddIdentity<IdentityExpressUser, IdentityExpressRole>()
//  .AddUserStore<IdentityExpressUserStore>()
//  .AddRoleStore<IdentityExpressRoleStore>()
//  .AddIdentityExpressUserClaimsPrincipalFactory();

//// Register the DbContext for the IdentityExpress schema
//builder.Services.AddDbContext<IdentityExpressDbContext>();

var app = builder.Build();

app.UseAdminUI();

app.Run();
