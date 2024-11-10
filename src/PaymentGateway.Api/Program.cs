using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Dal;
using PaymentGateway.Api.Externals;
using PaymentGateway.Api.Logic;
using PaymentGateway.Api.Logic.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<IPaymentsRepository, PaymentsRepository>()
    .AddSingleton<IBankSubmitPaymentRequestCreator, BankSubmitPaymentRequestCreator>()
    .AddSingleton<IPaymentDetailsCreator, PaymentDetailsCreator>()
    .AddSingleton<CurrencyCodeValidator>()
    .AddSingleton<FutureMonthValidator>()
    .AddSingleton<ModelValidator>()
    .AddSingleton<ValidPaymentSubmitter>()
    .AddSingleton(Guid.NewGuid)
    .AddSingleton(() => DateTime.UtcNow)
    .AddSingleton<IBankPaymentSubmitter>(
        provider => new BankPaymentSubmitter(
            provider.GetService<IHttpClientFactory>()!,
            "http://localhost:8080/payments"))
    .AddSingleton<IPaymentSubmitter>(
        provider => new PaymentSubmitter(
            provider.GetService<SubmitPaymentRequestValidator>()!,
            provider.GetService<ValidPaymentSubmitter>()!
            ))
    .AddSingleton(
        provider => new SubmitPaymentRequestValidator(
            provider.GetService<ModelValidator>()!,
            provider.GetService<FutureMonthValidator>()!,
            provider.GetService<CurrencyCodeValidator>()!))
    ;

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
