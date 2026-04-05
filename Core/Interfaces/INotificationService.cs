namespace Fashion.Api.Core.Interfaces
{
    public interface INotificationService
    {
        Task SendTrackingEmailAsync(
            string email,
            string trackingNumber,
            string shippingCompany);
    }
}