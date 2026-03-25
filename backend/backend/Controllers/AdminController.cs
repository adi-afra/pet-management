using backend.Data;
using backend.classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/admin/updatePetStatus/5
        [HttpPost("updatePetStatus/{petId}")]
        public async Task<IActionResult> UpdatePetStatus(int petId, [FromBody] UpdatePetStatusRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest(new { message = "Status is required." });
            }

            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null)
            {
                return NotFound(new { message = "Pet not found." });
            }

            // Validate status values, e.g., only allow certain statuses
            var validStatuses = new[] { "Available", "Adopted", "Pending", "Unavailable" };
            if (!validStatuses.Contains(request.Status))
            {
                return BadRequest(new { message = "Invalid status value." });
            }

            pet.SetStatus(request.Status);
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pet status updated successfully.", petId = petId, status = request.Status });
        }
    }

    public class UpdatePetStatusRequest
    {
        public string? Status { get; set; }
    }
}