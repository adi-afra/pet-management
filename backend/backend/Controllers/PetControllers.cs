using backend.classes;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text.Json;

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
        public async Task<ActionResult> SearchPets(
            [FromQuery] string? query
            )
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Ok(new List<object>()); 
                }

                query = query.ToLower();

                var pets = await _context.Pets
                    .Where(p => p.Status == PetStatus.Registered)
                    .Where(p =>

                    (p.Name != null && EF.Functions.Like(p.Name, $"%{query}%")) ||
                    (p.Breed != null && EF.Functions.Like(p.Breed, $"%{query}%"))
)
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Age,
                        Type = p.GetType().Name,
                        p.Breed,
                        p.ImageUrl
                    })
                    .ToListAsync();

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
        
        
        [HttpPost("toggleSavePets")]
        public async Task<IActionResult> ToggleSavePet([FromBody] JsonElement body)
        {
            try
            {
                int userId = body.GetProperty("userId").GetInt32();
                int petId = body.GetProperty("petId").GetInt32();

                var existing = await _context.SavedPets
                    .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PetId == petId);

                if (existing != null)
                {
                    // removing save
                    _context.SavedPets.Remove(existing);
                    await _context.SaveChangesAsync();

                    return Ok(new { saved = false });
                }

                // save
                var savedPet = new SavedPets
                {
                    UserId = userId,
                    PetId = petId
                };

                _context.SavedPets.Add(savedPet);
                await _context.SaveChangesAsync();

                return Ok(new { saved = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        
        //getting saved pets based on user id 
        [HttpGet("savedPets/{userId}")]
        public async Task<IActionResult> GetSavedPets(int userId)
        {
            var savedPets = await _context.SavedPets
                .Where(s => s.UserId == userId)
                .Include(s => s.Pet) 
                .Select(s => new {
                    s.Pet.Id,
                    s.Pet.Name,
                    s.Pet.Breed,
                    s.Pet.Age,
                    s.Pet.ImageUrl
                })
                .ToListAsync();

            return Ok(savedPets);
        }
    }
}