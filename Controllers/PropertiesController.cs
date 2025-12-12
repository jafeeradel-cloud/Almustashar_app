using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Almustashar_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PropertiesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/properties
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var properties = await _context.Properties.ToListAsync();
            return Ok(properties);
        }

        // GET: api/properties/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            return Ok(property);
        }

        // POST: api/properties
        [HttpPost]
        public async Task<IActionResult> Create(Property property)
        {
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = property.Id }, property);
        }

        // DELETE: api/properties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
