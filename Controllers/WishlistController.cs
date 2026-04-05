using Microsoft.AspNetCore.Mvc;


namespace Fashion.Api.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    public class WishlistController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetWishlist()
        {
            return Ok(new { message = "Wishlist feature coming soon" });
        }
        [HttpPost]
        public IActionResult PostWishlist()
        {
            return Ok(new { message = "User can send theire Wishesh" });
        }
    }
}
