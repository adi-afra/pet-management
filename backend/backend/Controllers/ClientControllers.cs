using backend.classes;
using Microsoft.AspNetCore.Http;
using backend.Data;
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

		//post method for creating an adoption meeting
		[HttpPost("adoptionMeetings/{userId}")]
		 public async Task<IActionResult> CreateAdoptionMeeting(int userId, [FromBody] JsonElement data)
		{
    		try
    			{
   					int petId = data.GetProperty("petId").GetInt32();

        			if (!data.TryGetProperty("date", out var dateProp))
            			return BadRequest(new { message = "date is required" });

        			DateTime date = dateProp.GetDateTime();
					
					//check to see if the user selected past dates
       				if (date.Date < DateTime.Today)
            			return BadRequest(new { message = "You cannot book a meeting in the past." });


					//fetching pet from the database
					var pet = await _context.Pets.FindAsync(petId);
					
        			// Check if this pet already has an adoption meeting on the same date
       				 bool alreadyBooked = await _context.Meetings
            .				AnyAsync(m => m.PetId == petId 
                           	&& m.Type == MeetingType.Adoption 
                           	&& m.Date.Date == date.Date); // compare only the date part

        			if (alreadyBooked)
            				return BadRequest(new { message = "This pet already has an adoption meeting on the selected date." });


        			// Create the adoption meeting
        			var meeting = new Meeting(date, pet, userId, MeetingType.Adoption);

        			_context.Meetings.Add(meeting);
        			await _context.SaveChangesAsync();

        			return Ok(new { message = "Adoption meeting created successfully", meeting });
    			}
    		catch (Exception ex)
    		{
       	 		return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
				
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
                try
                {
                    // Extract file name from URL
                    var fileName = Path.GetFileName(new Uri(meeting.Pet.ImageUrl).LocalPath);

                    // Build full local path
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/petPhotos", fileName);

                    // Delete file if exists
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    // Optional: log error instead of crashing
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }
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

        [HttpPost("surrenderMeetings")]
        public async Task<IActionResult> CreateSurrender([FromBody] JsonElement data)
        {
            try
            {
                Console.WriteLine("Received JSON: " + data.GetRawText());

                //  Validate required fields
                if (!data.TryGetProperty("animalType", out var animalTypeProp))
                    return BadRequest(new { message = "animalType is required" });

                var petTypeString = animalTypeProp.GetString();

                if (!Enum.TryParse<PetType>(petTypeString, true, out var petType))
                    return BadRequest(new { message = "Invalid animal type" });

                var name = data.GetProperty("name").GetString()?.Trim();
                var age = data.GetProperty("age").GetInt32();
                var breed = data.GetProperty("breed").GetString()?.Trim();
                var imageUrl = data.GetProperty("imageUrl").GetString();
                var userId = data.GetProperty("userId").GetInt32();
                var date = data.GetProperty("date").GetDateTime();

                // Try find existing pet
                var pet = await _context.Pets.FirstOrDefaultAsync(p =>
                    p.UserId == userId &&
                    p.Name == name &&
                    p.Age == age &&
                    p.Breed == breed 
                );

                // Create if not exists
                if (pet == null)
                {
                    pet = petType switch
                    {
                        PetType.Dog => new Dog(name, age, breed, imageUrl, userId),
                        PetType.Cat => new Cat(name, age, breed, imageUrl, userId),
                        _ => throw new Exception("Invalid animal type")
                    };

                    pet.SetStatus(PetStatus.Potential);

                    _context.Pets.Add(pet);

                    try
                    {
                        await _context.SaveChangesAsync(); // ensures unique constraint enforced
                    }
                    catch (DbUpdateException)
                    {
                        //If race condition happens, fetch existing pet
                        pet = await _context.Pets.FirstOrDefaultAsync(p =>
                            p.UserId == userId &&
                            p.Name == name &&
                            p.Age == age &&
                            p.Breed == breed
                        );

                        if (pet == null)
                            throw; // something unexpected
                    }
                }

                //Prevent duplicate surrender meeting
                bool surrenderExists = await _context.Meetings.AnyAsync(m =>
                    m.PetId == pet.Id &&
                    m.Type == MeetingType.Surrender 

                );

                if (surrenderExists)
                {
                    return BadRequest(new
                    {
                        message = "This pet already has a surrender meeting."
                    });
                }

                //Create meeting
                var meeting = new Meeting(
                    date,
                    pet,
                    userId,
                    MeetingType.Surrender
                );

                _context.Meetings.Add(meeting);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return BadRequest(new
                    {
                        message = "This pet already has a surrender meeting."
                    });
                }

                return Ok(new
                {
                    message = "Surrender meeting created successfully",
                    petId = pet.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    detail = ex.Message
                });
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