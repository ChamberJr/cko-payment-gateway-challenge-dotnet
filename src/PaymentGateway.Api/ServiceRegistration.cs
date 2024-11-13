using PaymentGateway.Api.Dal;
using PaymentGateway.Api.Externals;
using PaymentGateway.Api.Logic.Validation;
using PaymentGateway.Api.Logic;

namespace PaymentGateway.Api;

public static class ServiceRegistration
{
    public static void Register(IServiceCollection service, Func<Guid> getGuid, Func<DateTime> getCurrentDateTime, string bankUrl)
    {
        service
            .AddSingleton<IPaymentsRepository, PaymentsRepository>()
            .AddSingleton<IBankSubmitPaymentRequestCreator, BankSubmitPaymentRequestCreator>()
            .AddSingleton<IPaymentDetailsCreator, PaymentDetailsCreator>()
            .AddSingleton<CurrencyCodeValidator>()
            .AddSingleton<FutureMonthValidator>()
            .AddSingleton<ModelValidator>()
            .AddSingleton<ValidPaymentSubmitter>()
            .AddSingleton(getGuid)
            .AddSingleton(getCurrentDateTime)
            .AddSingleton<IBankPaymentSubmitter>(
                provider => new BankPaymentSubmitter(
                    provider.GetService<IHttpClientFactory>()!,
                    bankUrl))
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
    }

    public static void Register(IServiceCollection service)
    {
        Register(service, Guid.NewGuid, () => DateTime.UtcNow, "http://localhost:8080/payments");
    }
}
