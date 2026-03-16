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
            //check if the client object is empty
            if (client == null)
            {
                //return a message if it is empty
                return BadRequest("Client data is required");
            }

            //variable for checking if a username already is in use
            bool exist = await _context.Clients.AnyAsync(c => c.Username == client.Username);
            
            //check if exist is equal to true
            if (exist)
            {
                return BadRequest("Username already exists.");
            }
            

            //add the new client to the database
            _context.Clients.Add(client);
            
            //save the changes
            await _context.SaveChangesAsync();

            //return a successful message
            return Ok(client);
        }

        [HttpGet("adoptionMeetings/{userId}")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetAdoptionMeetings(string userId)
        {
            var meetings = await _context.Meetings
                .Where(m => m.UserId == userId)
                .Include(m => m.Pet)
                .ToListAsync();

            if (!meetings.Any())
                return NotFound("No meetings found for this user.");

            return Ok(meetings);
        }

        // DELETE: api/clients/adoptionMeetings/5
        [HttpDelete("adoptionMeetings/{meetingId}")]
        public async Task<IActionResult> DeleteMeeting(int meetingId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);

            if (meeting == null)
                return NotFound("Meeting not found.");

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 success
        }
    }
}