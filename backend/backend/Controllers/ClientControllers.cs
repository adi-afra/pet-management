using backend.classes;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static backend.Controllers.AuthController;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        //database context used to read CLient table
        private readonly AppDbContext _context;

        //giving my controler access to database through constructor
        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        // DTO for receiving registration data
        public class RegisterClientDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // POST: api/clients
        [HttpPost("register")]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            if (client == null)
                return BadRequest(new { message = "Client data is required." });

            try
            {
                // Check username
                bool exist = await _context.Clients.AnyAsync(c => c.Username == client.Username);
                if (exist)
                    return BadRequest(new { message = "Username already exists." });

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                // Return JSON object with message and client
                return Ok(new { message = "Registration successful!", client });
            }
            catch (Exception ex)
            {
                // Return JSON even on server error
                return StatusCode(500, new { message = "Internal server error.", detail = ex.Message });
            }
        }

        [HttpGet("adoptionMeetings/{userId}")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetAdoptionMeetings(int userId)
        {
            var meetings = await _context.Meetings
                .Where(m => m.UserId == userId)
                .Include(m => m.Pet)
                .ToListAsync();


            return Ok(meetings);
        }

        // DELETE: api/clients/adoptionMeetings/5
        [HttpDelete("adoptionMeetings/{meetingId}")]
        public async Task<IActionResult> DeleteMeeting(int meetingId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);
            if (meeting == null)
                return NotFound(new { message = "Meeting not found" });

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent(); //204 success
        }


        // DELETE: api/clients/logout
        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            return Ok(new { message = "Logged out successfully" });
        }
    }
}