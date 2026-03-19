using backend.Data;
using backend.classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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



            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 success
        }
        
        
        //api for deleting surrender meeting
        [HttpDelete("surrenderMeeting/{meetingId}")]
        public async Task<IActionResult> DeleteSurrenderMeeting(int meetingId)
        {
            // Find the meeting in the database
            var meeting = await _context.Meetings
                .Include(m => m.Pet)
                .FirstOrDefaultAsync(m => m.Id == meetingId);

            //check if the meeting is empty
            if (meeting == null)
                return NotFound($"Meeting with Id {meetingId} not found.");

            //check that it's actually a surrender meeting
            if (meeting.Type != MeetingType.Surrender)
                return BadRequest("Cannot delete a meeting that is not a surrender meeting.");

            // Remove potential pet if it has no other purpose
            if (meeting.Pet.Status == PetStatus.Potential)
            {
                _context.Pets.Remove(meeting.Pet);
            }

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // GET: api/Clients/surrenderMeetings/{userId}
        [HttpGet("surrenderMeetings/{userId}")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetSurrenderMeetings(int userId)
        {
            var meetings = await _context.Meetings
                .Where(m => m.UserId == userId && m.Type == MeetingType.Surrender)
                .Include(m => m.Pet)
                .ToListAsync();


            return Ok(meetings);
        }
    }
}