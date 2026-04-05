using Fashion.Api.DTOs;
using Fashion.Api.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly IRazorpayService _razorpayService;

    public PaymentController(
        IStripeService stripeService,
        IRazorpayService razorpayService)
    {
        _stripeService = stripeService;
        _razorpayService = razorpayService;
    }

    [HttpPost("create")]
    public IActionResult CreatePayment(
        [FromBody] PaymentRequestDto request)
    {
        if (request.Provider.ToLower() == "stripe")
        {
            var clientSecret =
                _stripeService.CreatePaymentIntent(
                    request.Amount,
                    "usd");

            return Ok(new
            {
                Provider = "stripe",
                ClientSecret = clientSecret
            });
        }

        if (request.Provider.ToLower() == "razorpay")
        {
            decimal exchangeRate = 90;
            decimal inrAmount = request.Amount * exchangeRate;

            var order =
                _razorpayService.CreateOrder(
                    inrAmount,
                    "INR");

            return Ok(new
            {
                Provider = "razorpay",
                Order = order,
                InrAmount = inrAmount
            });
        }

        return BadRequest("Invalid payment provider");
    }

    [HttpPost("verify-razorpay")]
    public IActionResult VerifyRazorpay(
        [FromBody] VerifyPaymentDto request)
    {
        bool isValid = _razorpayService.VerifyPayment(
            request.OrderId,
            request.PaymentId,
            request.Signature);

        if (!isValid)
            return BadRequest("Payment verification failed");

        return Ok("Payment successful");
    }
}