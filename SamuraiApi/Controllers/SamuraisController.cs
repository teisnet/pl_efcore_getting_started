using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;

namespace SamuraiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamuraisController : ControllerBase
    {
        private readonly SamuraiContext _context;

        public SamuraisController(SamuraiContext context)
        {
            _context = context;
        }

        // GET: api/Samurais
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Samurai>>> GetSamurais()
        {
            return await _context.Samurais.ToListAsync();
        }

        // GET: api/Samurais/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Samurai>> GetSamurai(int id)
        {
            var samurai = await _context.Samurais.FindAsync(id);

            if (samurai == null)
            {
                return NotFound();
            }

            return samurai;
        }

        // PUT: api/Samurais/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSamurai(int id, Samurai samurai)
        {
            if (id != samurai.Id)
            {
                return BadRequest();
            }

            // Using the 'Entry' method to ensure no attached data is included in the update,
            // as this controller medhod is only intended to update a samurai.
            // 'Entry' will only update the main object and ignore anything that is connected to it.
            _context.Entry(samurai).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SamuraiExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // 204 No content
            return NoContent();
        }

        // POST: api/Samurais
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Samurai>> PostSamurai(Samurai samurai)
        {
            _context.Samurais.Add(samurai);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSamurai", new { id = samurai.Id }, samurai);
        }

        // DELETE: api/Samurais/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Samurai>> DeleteSamurai(int id)
        {
            var samurai = await _context.Samurais.FindAsync(id);
            if (samurai == null)
            {
                return NotFound();
            }

            // Using 'Remove' and not 'Context.Entry' as the code knows that it only retrieved a Samurai, not a graph with related data.
            // Teis question: how can the code be sure about that?
            _context.Samurais.Remove(samurai);
            await _context.SaveChangesAsync();

            return samurai;
        }

        // DELETE using stored procedure
        [HttpDelete("sproc/{id}")]
        public async Task<ActionResult<string>> DeleteQuotesForSamurai(int id)
		{
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC DeleteQuotesFromSamurai {id}");
            return $"{rowsAffected} Quotes deleted";
		}

        private bool SamuraiExists(int id)
        {
            return _context.Samurais.Any(e => e.Id == id);
        }
    }
}
