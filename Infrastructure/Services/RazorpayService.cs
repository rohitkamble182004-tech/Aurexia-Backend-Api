using Fashion.Api.Core.Interfaces;
using Fashion.Api.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace Fashion.Api.Infrastructure.Services
{
    public class RazorpayService : IRazorpayService
    {
        private readonly RazorpaySettings _settings;

        public RazorpayService(IOptions<RazorpaySettings> options)
        {
            _settings = options.Value;
        }

        public object CreateOrder(decimal amount, string currency)
        {
            var client = new RazorpayClient(
                _settings.Key,
                _settings.Secret
            );

            var options = new Dictionary<string, object>
            {
                { "amount", (long)(amount * 100) }, // Razorpay uses paise
                { "currency", currency },
                { "receipt", Guid.NewGuid().ToString() }
            };

            var order = client.Order.Create(options);

            return new
            {
                OrderId = order["id"].ToString(),
                Amount = order["amount"],
                Currency = order["currency"]
            };
        }

        public bool VerifyPayment(string orderId, string paymentId, string signature)
        {
            string payload = orderId + "|" + paymentId;

            var hash = new HMACSHA256(
                Encoding.UTF8.GetBytes(_settings.Secret)
            );

            var generatedSignature = BitConverter
                .ToString(hash.ComputeHash(
                    Encoding.UTF8.GetBytes(payload)))
                .Replace("-", "")
                .ToLower();

            return generatedSignature == signature;
        }
    }
}