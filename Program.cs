using TransactionAPI.Services;
using TransactionAPI.Services.Interfaces;
using TransactionAPI.Validators;
using TransactionAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep original property names
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Transaction API", Version = "v1" });
});

// Register application services following Dependency Inversion principle
builder.Services.AddScoped<ITransactionValidator, TransactionValidator>();
builder.Services.AddScoped<IPartnerAuthenticationService, PartnerAuthenticationService>();
builder.Services.AddScoped<ISignatureValidationService, SignatureValidationService>();
builder.Services.AddScoped<IDiscountCalculationService, DiscountCalculationService>();
builder.Services.AddScoped<ITransactionProcessingService, TransactionProcessingService>();
builder.Services.AddScoped<ITransactionLogRepository, TransactionLogRepository>();

// Add log4net
builder.Logging.AddLog4Net("log4net.config");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();