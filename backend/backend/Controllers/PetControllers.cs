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

        // GET: api/Pet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPetById(int id)
        {
            var pet = await _context.Pets.FindAsync(id);

            if (pet == null)
                return NotFound("Pet not found");

            return Ok(pet);
        }

        // GET: api/Pet/search?query=dog
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Pet>>> SearchPets(string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest("Search query required");

            var q = query.ToLower();

            var pets = await _context.Pets
                    .Where(p => p.Status == PetStatus.Registered)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Age,
                        Type = p.GetType().Name, // Dog or Cat
                        p.ImageUrl
                    })
                .ToListAsync();

            return Ok(pets);
        }
            catch (Exception ex)
            {
                return StatusCode(500, new

        // POST: api/Pet/bookMeeting
        [HttpPost("bookMeeting")]
        public async Task<IActionResult> BookMeeting([FromBody] Meeting meeting)
        {
                    message = "Error retrieving pets",
                    detail = ex.Message
                });
            }
            if (meeting == null)
                return BadRequest("Meeting data required");

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return Ok(meeting);
        }
    }
}