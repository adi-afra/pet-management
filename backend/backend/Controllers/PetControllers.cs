using backend.Data;
using backend.classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetController(AppDbContext context)
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
                .Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(q)) ||
                    (p.Breed != null && p.Breed.ToLower().Contains(q)) ||
                    // This is the magic line that searches the hidden "Discriminator" column
                    EF.Property<string>(p, "Discriminator").ToLower().Contains(q)
                )
                .ToListAsync();

            return Ok(pets);
        }

        // POST: api/Pet/bookMeeting
        [HttpPost("bookMeeting")]
        public async Task<IActionResult> BookMeeting([FromBody] Meeting meeting)
        {
            if (meeting == null)
                return BadRequest("Meeting data required");

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return Ok(meeting);
        }
    }
}