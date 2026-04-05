using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Enums;
using Fashion.Api.Infrastructure.Data;
using Fashion.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Auth;

namespace Fashion.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtTokenGenerator _jwt;

        public AuthController(AppDbContext db, JwtTokenGenerator jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // =====================
        // LOGIN (Firebase)
        // =====================
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (!authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing Firebase token");

            var firebaseToken = authHeader.Replace("Bearer ", "");

            FirebaseToken decoded;
            try
            {
                decoded = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(firebaseToken);
            }
            catch
            {
                return Unauthorized("Invalid Firebase token");
            }

            var uid = decoded.Uid;
            var email = decoded.Claims["email"]?.ToString();
            var emailVerified =
                decoded.Claims.ContainsKey("email_verified") &&
                (bool)decoded.Claims["email_verified"];

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Email not found");

            if (!emailVerified)
                return Unauthorized("Email not verified");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.FirebaseUid == uid);

            if (user == null)
            {
                user = new User
                {
                    FirebaseUid = uid,
                    Email = email,
                    Role = UserRole.User
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            var token = _jwt.Generate(user);

            return Ok(new
            {
                token,
                role = user.Role.ToString().ToLower()
            });
        }

        // =====================
        // GOOGLE LOGIN (Firebase)
        // =====================
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (!authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing Firebase token");

            var firebaseToken = authHeader.Replace("Bearer ", "");

            FirebaseToken decoded;
            try
            {
                decoded = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(firebaseToken);
            }
            catch
            {
                return Unauthorized("Invalid Firebase token");
            }

            var uid = decoded.Uid;
            var email = decoded.Claims["email"]?.ToString();

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Email not found");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.FirebaseUid == uid);

            if (user == null)
            {
                user = new User
                {
                    FirebaseUid = uid,
                    Email = email,
                    Role = UserRole.User
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            var token = _jwt.Generate(user);

            return Ok(new
            {
                token,
                role = user.Role.ToString().ToLower()
            });
        }

    }
}
