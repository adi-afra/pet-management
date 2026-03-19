using backend.Data;
using backend.classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Client loginData)
        {
            if (loginData == null || string.IsNullOrWhiteSpace(loginData.Username) || string.IsNullOrWhiteSpace(loginData.Password))
                return BadRequest(new { message = "Login data is required" });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginData.Username);

            if (user == null || user.Password != loginData.Password)
                return Unauthorized(new { message = "Invalid username or password" });

            // Login successful: store info in session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.getRole());
            HttpContext.Session.SetString("Username", user.Username);

            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.getRole()
            });
        }


        [HttpGet("status")]
        public IActionResult CheckLoginStatus()
        {
            // Try to get the user id from session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // No session means user is not logged in
                return Unauthorized(new { message = "Not logged in" });
            }

            // Optionally, you can fetch the client from the database
            var client = _context.Clients.FirstOrDefault(c => c.Id == userId.Value);

            if (client == null)
            {
                // Session has a UserId that no longer exists
                return Unauthorized(new { message = "User not found" });
            }

            // Return logged-in user info
            return Ok(new
            {
                Id = client.Id,
                Username = client.Username,
                Role = client.getRole()
            });
        }
    }
}