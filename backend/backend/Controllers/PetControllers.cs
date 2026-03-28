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

        // GET: api/pets
        [HttpGet("pet")]
        public async Task<IActionResult> GetPets()
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
                        Type = p.GetType().Name, // Dog or Cat
                        p.ImageUrl
                    })
                    .ToListAsync();

                return Ok(pets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving pets",
                    detail = ex.Message
                });
            }
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

            try
            {
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

                var pets = await query.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Age,
                    Type = p.GetType().Name, // Dog or Cat
                    p.ImageUrl
                }).ToListAsync();
                return Ok(pets);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving pets",
                    detail = ex.Message
                });
            }
        }
    }
}