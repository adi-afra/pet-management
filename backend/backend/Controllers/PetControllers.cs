using backend.Data;
using backend.classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.classes;

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
    }
}