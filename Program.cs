using BankTransactionApi.DbContexts;
using BankTransactionApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

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

var dbConnectionString = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("AppDb")
    : Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

builder.Services.AddDbContext<TransactionDbContext>(
    o =>
    {
        o.EnableSensitiveDataLogging();
        o.EnableDetailedErrors();
        o.UseNpgsql(dbConnectionString);
    },
    ServiceLifetime.Transient);

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
