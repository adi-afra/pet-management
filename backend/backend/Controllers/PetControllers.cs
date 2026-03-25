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
    }
}