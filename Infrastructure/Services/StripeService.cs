using Fashion.Api.Core.Interfaces;
using Microsoft.Extensions.Options;
using Fashion.Api.Infrastructure.Configurations;
using Stripe;


namespace Fashion.Api.Infrastructure.Services
{
    public class StripeService : IStripeService
    {
        public StripeService(IOptions<StripeSettings> options)
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }


        public string CreatePaymentIntent(decimal amount, string currency)
        {
            var service = new PaymentIntentService();
            var intent = service.Create(new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency
            });


            return intent.ClientSecret;
        }
    }
}