using backend.classes;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
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

            // --- Manual validation ---
            if (string.IsNullOrWhiteSpace(client.Username))
                return BadRequest(new { message = "Username is required." });

            if (string.IsNullOrWhiteSpace(client.Password))
                return BadRequest(new { message = "Password is required." });
            
            if (string.IsNullOrWhiteSpace(client.Email))
                return BadRequest(new { message = "Email is required." });

            if (string.IsNullOrWhiteSpace(client.Email) || 
                !System.Text.RegularExpressions.Regex.IsMatch(client.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { message = "Invalid email format." });
            }

            // Check username uniqueness
            if (await _context.Clients.AnyAsync(c => c.Username == client.Username))
                return BadRequest(new { message = "Username already exists." });

            try
            {
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registration successful!", client });
            }
            catch (Exception ex)
            {
                // Only for unexpected runtime errors
                return StatusCode(500, new { message = "Internal server error.", detail = ex.Message });
            }
        }

        
        
        [HttpGet("adoptionMeetings/{userId}")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetAdoptionMeetings(int userId)
        {
            var meetings = await _context.Meetings
                .Where(m => m.UserId == userId && m.Type == MeetingType.Adoption)
                .Include(m => m.Pet)
                .ToListAsync();


            return Ok(meetings);
        }

        
        
        // DELETE: api/clients/adoptionMeetings/5
        [HttpDelete("adoptionMeetings/{meetingId}")]
        public async Task<IActionResult> DeleteMeeting(int meetingId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);

            //check if the meeting is empty
            if (meeting == null)
                return NotFound($"Meeting with Id {meetingId} not found.");

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 success
        }
        
        
        //api for deleting surrender meeting
        [HttpDelete("surrenderMeetings/{meetingId}")]
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

        // POST: api/clients
        [HttpPost("surrenderMeetings")]
        public async Task<IActionResult> CreateSurrender([FromBody] JsonElement data)
        {
            
            try
            {
                //  Validate required fields
                if (!data.TryGetProperty("animalType", out var animalTypeProp))
                    return BadRequest(new { message = "animalType is required" });

                var animalTypeString = animalTypeProp.GetString();

                //  Convert string → enum
                if (!Enum.TryParse<PetType>(animalTypeString, true, out var petType))
                    return BadRequest("Invalid animal type");

                // 🐾 Create correct object using switch
                Pet pet = petType switch
                {
                    PetType.Dog => new Dog
                    (
                        data.GetProperty("name").GetString(),
                        data.GetProperty("age").GetInt32(),
                        data.GetProperty("breed").GetString()
                    ),

                    PetType.Cat => new Cat
                    (
                        data.GetProperty("name").GetString(),
                        data.GetProperty("age").GetInt32(),
                        data.GetProperty("breed").GetString()
                    ),

                    _ => throw new Exception("Invalid animal type")
                };

                //setting the satus as potenial
                pet.SetStatus(PetStatus.Potential);

                //  Create meeting
                var meeting = new Meeting
                (
                    data.GetProperty("date").GetDateTime(),
                    pet,
                    data.GetProperty("userId").GetInt16(),
                    MeetingType.Surrender 
                );

                //  Save to DB
                _context.Pets.Add(pet);
                _context.Meetings.Add(meeting);
                _context.SaveChanges();

                return Ok(new { message = "Surrender meeting created successfully" });
            }
            catch (Exception ex)
            {
                // Return JSON even on server error
                return StatusCode(500, new { message = "Internal server error.", detail = ex.Message });
            }
        }

    }
}