using backend.Data;
using backend.classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        //database context used to read CLient table and the storage for pet images
        private readonly AppDbContext _context;
        private readonly string _storageConnectionString;

        //giving my controler access to database through constructor
        public ClientsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
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

            if (!string.IsNullOrEmpty(meeting.Pet?.ImageUrl))
            {
                //Connect to Azure Blob Storage
                var blobServiceClient = new BlobServiceClient(_storageConnectionString);

                //Get container
                var containerClient = blobServiceClient.GetBlobContainerClient("pet-images");

                //extracting the file name from the string url we have 
                var fileName = Path.GetFileName(new Uri(meeting.Pet.ImageUrl).LocalPath);

                //getting the refrence from the blob
                var blobImage = containerClient.GetBlobClient(fileName);

                //deleting the image from the blob if it exist
                await blobImage.DeleteIfExistsAsync();
            }
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
                Console.WriteLine("Received JSON: " + data.GetRawText());
                //  Validate required fields
                if (!data.TryGetProperty("animalType", out var animalTypeProp))
                    return BadRequest(new { message = "animalType is required" });

                var animalTypeString = animalTypeProp.GetString();

                //  Convert string → enum
                if (!Enum.TryParse<PetType>(animalTypeString, true, out var petType))
                    return BadRequest("Invalid animal type");

                // Create correct object using switch
                Pet pet = petType switch
                {
                    PetType.Dog => new Dog
                    (
                        data.GetProperty("name").GetString(),
                        data.GetProperty("age").GetInt32(),
                        data.GetProperty("breed").GetString(),
                        data.GetProperty("imageUrl").GetString()
                    ),

                    PetType.Cat => new Cat
                    (
                        data.GetProperty("name").GetString(),
                        data.GetProperty("age").GetInt32(),
                        data.GetProperty("breed").GetString(),
                        data.GetProperty("imageUrl").GetString()
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
                    data.GetProperty("userId").GetInt32(),
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
        
        
        
        // POST: api/clients/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement data)
        {
            try
            {
                var username = data.GetProperty("username").GetString();
                var password = data.GetProperty("password").GetString();

                if (string.IsNullOrWhiteSpace(username))
                    return BadRequest(new { message = "Username is required." });

                if (string.IsNullOrWhiteSpace(password))
                    return BadRequest(new { message = "Password is required." });

                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (client == null || client.Password != password)
                    return Unauthorized(new { message = "Invalid username or password." });

                
                HttpContext.Session.SetInt32("UserId", client.Id);
                HttpContext.Session.SetString("Username", client.Username);
                HttpContext.Session.SetString("UserRole", client.getRole());
                
                return Ok(new { message = "Login successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }
        
        //Get method to see if a user is logged in
        [HttpGet("session")]
        public IActionResult GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var username = HttpContext.Session.GetString("Username");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (userId == null)
                return Unauthorized(new { message = "Not logged in" });

            return Ok(new { userId, username,userRole });
        }
        
        //logging out
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Clear the session completely
            HttpContext.Session.Clear();

            // Return a success message
            return Ok(new { message = "Logged out successfully." });
        }

    }
}