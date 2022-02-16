using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

#region Additional Namespaces
using ChinookSystem;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Given
//supplied database connection due to the fact that we create this web app 
//  to use Individual Accounts
//Code retreives the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//Added
//Code retreived the ChinookDB connection string 
var connectionStringChinook = builder.Configuration.GetConnectionString("ChinookDB");

//Given
//Register the supplied connection string with the IServiceCollection (.Services)
//Registers the connection string for Individual Accounts 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//Added
//Code the logic to add our class library services to IServiceCollection
//One could do the registration code here in Program.cs
//HOWEVER, every time a service class is added, you would be changing this file 
//The implementation of the DBContent and AddTransient(..) code in this example 
//   will be done in an extension method to IServiceCollection 
//The extension method will be coded inside the ChinookSystem class library 
//The extension method will have a parameter: and this paramater is and options => options.UseSqlServer()
builder.Services.ChinookSystemBackendDependencies(options =>
    options.UseSqlServer(connectionStringChinook));

//Given
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
