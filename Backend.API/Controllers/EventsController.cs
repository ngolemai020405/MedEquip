using Backend.API.Data;
using Backend.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.OrderByDescending(e => e.EventId).ToListAsync();
        }

        // GET: api/Events/Active
        [HttpGet("Active")]
        public async Task<ActionResult<IEnumerable<Event>>> GetActiveEvents()
        {
            return await _context.Events
                .Where(e => e.TrangThai && e.NgayKetThuc >= DateTime.Now)
                .OrderByDescending(e => e.EventId)
                .ToListAsync();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);

            if (evt == null)
            {
                return NotFound();
            }

            return evt;
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, Event evt)
        {
            if (id != evt.EventId)
            {
                return BadRequest();
            }

            _context.Entry(evt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Events
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(Event evt)
        {
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = evt.EventId }, evt);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
            {
                return NotFound();
            }

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
