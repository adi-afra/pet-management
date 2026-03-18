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
        public async Task<IActionResult> CreateClient([FromBody] RegisterClientDto dto)
        {
            //Check if the client data is empty
            if (dto == null) return BadRequest(new { message = "Data required" });

            //Check username
            bool exists = await _context.Users.OfType<Client>()
                .AnyAsync(c => c.Username == dto.Username);

            if (exists) return BadRequest(new { message = "Username already exists" });

            
            var client = new Client(dto.Username, dto.Password);

            _context.Users.Add(client);
            await _context.SaveChangesAsync();

            // Return JSON object with message and client
            return Ok(new { message = "Registration successful!", client });
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