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

        [HttpGet]
        public async Task<IActionResult> GetAllPets()
        {
            var pets = await _context.Pets.ToListAsync();
            return Ok(pets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPetById(int id)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
            {
                return NotFound(new { message = "Pet not found." });
            }

            return Ok(pet);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterPets(
            [FromQuery] string? animalType,
            [FromQuery] string? breed,
            [FromQuery] int? minAge,
            [FromQuery] int? maxAge,
            [FromQuery] PetStatus? status)
        {
            IQueryable<Pet> query = _context.Pets;

            if (!string.IsNullOrWhiteSpace(animalType))
            {
                if (animalType.Equals("Dog", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p is Dog);
                }
                else if (animalType.Equals("Cat", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p is Cat);
                }
            }

            if (!string.IsNullOrWhiteSpace(breed))
            {
                query = query.Where(p => p.Breed.ToLower() == breed.ToLower());
            }

            if (minAge.HasValue)
            {
                query = query.Where(p => p.Age >= minAge.Value);
            }

            if (maxAge.HasValue)
            {
                query = query.Where(p => p.Age <= maxAge.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            var pets = await query.ToListAsync();
            return Ok(pets);
        }
    }
}