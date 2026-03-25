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