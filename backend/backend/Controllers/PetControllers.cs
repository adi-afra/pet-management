using backend.classes;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Pets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPetById(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null) return NotFound("Pet not found");
            return Ok(pet);
        }

        // GET: api/Pets/search?query=dog
        [HttpGet("search")]
        public async Task<ActionResult> SearchPets(string query)
        {
            try
            {
                var pets = await _context.Pets
                    .Where(p => p.Status == PetStatus.Registered)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Age,
                        p.Breed,
                        Type = p.GetType().Name,
                        p.ImageUrl
                    })
                    .ToListAsync();

                // Optional: Filter list by query if needed
                if (!string.IsNullOrEmpty(query))
                {
                    var q = query.ToLower();
                    var filtered = pets.Where(p =>
                        p.Name.ToLower().Contains(q) ||
                        p.Breed.ToLower().Contains(q) ||
                        p.Type.ToLower().Contains(q)
                    ).ToList();
                    return Ok(filtered);
                }

                return Ok(pets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving pets", detail = ex.Message });
            }
        }

        // POST: api/Pets/bookMeeting
        [HttpPost("bookMeeting")]
        public async Task<IActionResult> BookMeeting([FromBody] Meeting meeting)
        {
            if (meeting == null) return BadRequest("Meeting data required");

            try
            {
                _context.Meetings.Add(meeting);
                await _context.SaveChangesAsync();
                return Ok(meeting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to book meeting", detail = ex.Message });
            }
        }

        [HttpPut("bookPet/{id}")] // 'id' is the Pet's ID
        public async Task<IActionResult> BookPet(int id, [FromBody] Meeting incomingMeeting)
        {
            // 1. Find the existing Pet in the database
            // We 'Include' AdoptionMeetings so EF knows they are linked
            var pet = await _context.Pets
                .Include(p => p.Meetings)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null) return NotFound("Pet not found");

            // 2. Use your SetPet function to link the meeting to the 'Tracked' pet
            incomingMeeting.SetPet(pet);

            // 3. Update the Pet's status (e.g., Status 2 = Pending Adoption)
            // pet.Status = 2; 

            // 4. Add the meeting to the Pet's list
            _context.Meetings.Add(incomingMeeting);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Pet successfully booked for a meeting!" });
        }

        [HttpPost("savePet")]
        public async Task<IActionResult> SavePet([FromBody] SavedRequest request)
        {
            // Check if the record already exists
            var existingSave = await _context.SavedPets
                .FirstOrDefaultAsync(s => s.ClientId == request.UserId && s.PetId == request.PetId);

            if (existingSave != null)
            {
                // TOGGLE OFF: If it exists, remove it
                _context.SavedPets.Remove(existingSave);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Pet unsaved", isSaved = false });
            }

            // TOGGLE ON: If it doesn't exist, add it
            var savedPet = new SavedPet
            {
                ClientId = request.UserId,
                PetId = request.PetId
            };

            _context.SavedPets.Add(savedPet);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pet saved", isSaved = true });
        }

        [HttpGet("savedPets/{userId}")]
        public async Task<IActionResult> GetSavedPets(int userId)
        {
            var saved = await _context.SavedPets
                .Where(s => s.ClientId == userId)
                .ToListAsync();

            return Ok(saved);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets()
        {
            // This returns EVERY pet in the database
            return await _context.Pets.ToListAsync();
        }

        // Simple helper class for the request body
        public class SavedRequest
        {
            public int UserId { get; set; }
            public int PetId { get; set; }
        }
    }
}