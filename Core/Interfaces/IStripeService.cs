namespace Fashion.Api.Core.Interfaces
{
    public interface IStripeService
    {
        string CreatePaymentIntent(decimal amount, string currency);
    }
}
