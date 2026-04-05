using Fashion.Api.Core.Interfaces;

namespace Fashion.Api.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        public async Task SendTrackingEmailAsync(
            string email,
            string trackingNumber,
            string shippingCompany)
        {
            // TODO: integrate real email provider (SMTP / SendGrid)

            Console.WriteLine($"Sending tracking email to {email}");
            Console.WriteLine($"Carrier: {shippingCompany}");
            Console.WriteLine($"Tracking #: {trackingNumber}");

            await Task.CompletedTask;
        }
    }
}