using backend.Data;
using backend.classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{/*
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
           sername = data.GetProperty("username").GetString();
                var password = data.GetProperty("password").GetString();

                if (string.IsNullOrWhiteSpace(username))
                    return BadRequest(new { message = "Username is required." });

                if (string.IsNullOrWhiteSpace(password))
                    return BadRequest(new { message = "Password is required." });

                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (client == null || client.Password != password)
                    return Unauthorized(new { message = "Invalid username or password." });

                
                HttpContext.Session.SetInt32("UserId", client.Id);
                HttpContext.Session.SetString("Username", client.Username);
                HttpContext.Session.SetString("UserRole", client.getRole());
                
                return Ok(new { message = "Login successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        } }

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
    }*/
}