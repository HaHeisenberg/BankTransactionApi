using BankTransactionApi.DbContexts;
using BankTransactionApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var accountsBaseAddress = builder.Environment.IsDevelopment()
        ? "http://"
            + builder.Configuration.GetSection("Services").GetSection("Accounts").GetValue(typeof(string), "Host")
            + ":"
            + builder.Configuration.GetSection("Services").GetSection("Accounts").GetValue(typeof(string), "Port")
        : Environment.GetEnvironmentVariable("ACCOUNTS_API_ADDRESS");
builder.Services.AddHttpClient(
    "AccountsAPI",
    httpClient => 
    {
        httpClient.BaseAddress = new Uri(accountsBaseAddress);
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "banktransactionapi";
        }
    );

SecretClientOptions options = new SecretClientOptions()
{
    Retry =
        {
            Delay= TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
         }
};
var client = new SecretClient(new Uri("https://bankprojectkeyvault.vault.azure.net/"), new DefaultAzureCredential(), options);

KeyVaultSecret secret = client.GetSecret("BankProjectSQLConnectionString");

string dbConnectionString = secret.Value;

builder.Services.AddSqlServer<TransactionDbContext>(dbConnectionString, options => options.EnableRetryOnFailure());

//builder.Services.AddDbContext<TransactionDbContext>(
//    o =>
//    {
//        o.EnableSensitiveDataLogging();
//        o.EnableDetailedErrors();
//        o.UseNpgsql(dbConnectionString);
//    },
//    ServiceLifetime.Transient);

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
