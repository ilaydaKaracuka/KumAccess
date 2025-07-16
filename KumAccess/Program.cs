using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Data.SqlClient;
using KumAccess.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();


builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<UserGet>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<RoleSet>();
builder.Services.AddScoped<GroupSet>();
builder.Services.AddScoped<RoleGet>();
builder.Services.AddScoped<GroupGet>();
builder.Services.AddScoped<ApplicationGet>();
builder.Services.AddScoped<UserGroupSet>();
builder.Services.AddScoped<GroupAppRoleSet>();
builder.Services.AddScoped<UserAppRoleGet>();
builder.Services.AddScoped<UserGroupGet>();
builder.Services.AddScoped<GroupAppRoleGet>();

builder.Services.AddTransient<UserAppRoleSet>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
