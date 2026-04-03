using backend.classes;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _storageConnectionString;

        public PetsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
        }
        
        
        
        // GET: api/pets
        [HttpGet("pet")]
        public async Task<IActionResult> GetPets()
        {
            try
            {
                var pets = await _context.Pets
                    .Where(p => p.Status == PetStatus.Registered)
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Age,
                        Type = p.GetType().Name, // Dog or Cat
                        p.Breed,
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

        // GET: api/Pets/search?query=dog
        [HttpGet("search")]
        public async Task<ActionResult> SearchPets(string query)
        {
            try
            {
                var pets = await _context.Pets
                    .Where(p => p.Status == PetStatus.Registered)
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Age,
                        Type = p.GetType().Name, // Dog or Cat
                        p.Breed,
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
            [FromQuery] int? minAge,
            [FromQuery] int? maxAge
            )
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


                if (minAge.HasValue)
                {
                    query = query.Where(p => p.Age >= minAge.Value);
                }

                if (maxAge.HasValue)
                {
                    query = query.Where(p => p.Age <= maxAge.Value);
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

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            //Validate file
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Only image files allowed");

            //Connect to Azure Blob Storage
            var blobServiceClient = new BlobServiceClient(_storageConnectionString);

            //Get container
            var containerClient = blobServiceClient.GetBlobContainerClient("pet-images");

            //Create container if not exists
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            //Generate unique filename
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            //Create blob reference
            var blobClient = containerClient.GetBlobClient(fileName);

            //Upload file
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // 8. Return URL
            return Ok(new { imageUrl = blobClient.Uri.AbsoluteUri });
        }
    }
}