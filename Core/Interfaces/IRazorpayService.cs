namespace Fashion.Api.Core.Interfaces
{
    public interface IRazorpayService
    {
        object CreateOrder(decimal amount, string currency);
        bool VerifyPayment(string orderId, string paymentId, string signature);
    }
}