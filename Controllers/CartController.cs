using Microsoft.AspNetCore.Mvc;

namespace Fashion.Api.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCart()
        {
            return Ok(new { message = "Cart feature coming soon" });
        }
    }
}
