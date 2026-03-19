using backend.Data;
using backend.classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        //database context used to read CLient table
        private readonly AppDbContext _context;

        //giving my controler access to database through constructor
        public ClientsController(AppDbContext context)
        {
            _context = context;
        }
        

        // POST: api/clients/register
        [HttpPost("register")]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            //check if the client object is empty
            if (client == null)
            {
                //return a message if it is empty
                return BadRequest(new {message="Client data is required"});
            }

            //variable for checking if a username already is in use
            bool exist = await _context.Clients.AnyAsync(c => c.Username == client.Username);
            
            //check if exist is equal to true
            if (exist)
            {
                return BadRequest(new {message = "Username already exists."});
            }
            

            //add the new client to the database
            _context.Clients.Add(client);
            
            //save the changes
            await _context.SaveChangesAsync();

            //return a successful message
            return Ok(client);
        }

        // POST: api/clients/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var client = await _context.Clients
                .SingleOrDefaultAsync(c => c.Username == request.Username && c.Password == request.Password);

            if (client == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // store authenticated user info in session
            HttpContext.Session.SetInt32("UserId", client.Id);
            HttpContext.Session.SetString("Username", client.Username);
            HttpContext.Session.SetString("UserRole", client.getRole());

            return Ok(new { message = "Logged in.", userId = client.Id, username = client.Username });
        }

        // GET: api/clients/login?username={username}&password={password}
        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var client = await _context.Clients
                .SingleOrDefaultAsync(c => c.Username == username && c.Password == password);

            if (client == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // store authenticated user info in session
            HttpContext.Session.SetInt32("UserId", client.Id);
            HttpContext.Session.SetString("Username", client.Username);
            HttpContext.Session.SetString("UserRole", client.getRole());

            return Ok(new { message = "Logged in.", userId = client.Id, username = client.Username });
        }

        public record LoginRequest(string Username, string Password);

        // GET: api/clients/session
        [HttpGet("session")]
        public IActionResult GetSession()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var username = HttpContext.Session.GetString("Username");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return Unauthorized(new { message = "Not logged in." });
            }

            return Ok(new { userId, username, userRole });
        }

        // DELETE: api/clients/logout
        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return NoContent();
        }

        [HttpGet("adoptionMeetings/{userId}")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetAdoptionMeetings(int userId)
        {
            var meetings = await _context.Meetings
                .Where(m => m.UserId == userId)
                .Include(m => m.Pet)
                .ToListAsync();


            return Ok(meetings);
        }

        // DELETE: api/clients/adoptionMeetings/5
        [HttpDelete("adoptionMeetings/{meetingId}")]
        public async Task<IActionResult> DeleteMeeting(int meetingId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);



            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 success
        }
    }
}