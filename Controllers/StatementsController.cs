using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _21GoatBackend.Models;
using System.IO;

namespace _21GoatBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatementsController : ControllerBase
    {
        private readonly GoatContext _context;

        public StatementsController(GoatContext context)
        {
            _context = context;
        }


        [HttpGet("api/import",Name = "GetImport")]
        [Route("import")]
        public IActionResult Import()
        {
            string filename = "statements_en.txt";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/wwwroot/" + filename;
            string[] lines = System.IO.File.ReadAllLines(filepath);
            int counter = 0;
            foreach (string line in lines)
            {
                if (!StatementExists(line))
                {
                    counter++;
                    _context.Statements.Add(new Statement { Content = line, Language = "EN" });
                }
            }
            _context.SaveChanges();
            return new JsonResult($"Added {counter} statements");
        }

        [HttpGet("api/random", Name = "GetRandom")]
        [Route("random")]
        public ActionResult<Statement> Random()
        {
            Random random = new Random();
            int number = random.Next(0, _context.Statements.Count()-1);
            var statement = _context.Statements.Skip(number-1).FirstOrDefault();

            if (statement == null)
            {
                return NotFound();
            }

            return statement;
        }

        // GET: api/Statements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Statement>>> GetStatements()
        {
            return await _context.Statements.ToListAsync();
        }

        // GET: api/Statements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Statement>> GetStatement(int id)
        {
            var statement = await _context.Statements.FindAsync(id);

            if (statement == null)
            {
                return NotFound();
            }

            return statement;
        }

        // PUT: api/Statements/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStatement(int id, Statement statement)
        {
            if (id != statement.Id)
            {
                return BadRequest();
            }

            _context.Entry(statement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatementExists(id))
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

        // POST: api/Statements
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Statement>> PostStatement(Statement statement)
        {
            if(!StatementExists(statement.Content))
            { 
                _context.Statements.Add(statement);
                await _context.SaveChangesAsync();
            }
            else
                throw new ArgumentException($"This statement already exists.");
            return CreatedAtAction(nameof(GetStatement), new { id = statement.Id }, statement);
        }

        // DELETE: api/Statements/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Statement>> DeleteStatement(int id)
        {
            var statement = await _context.Statements.FindAsync(id);
            if (statement == null)
            {
                return NotFound();
            }

            _context.Statements.Remove(statement);
            await _context.SaveChangesAsync();

            return statement;
        }

        private bool StatementExists(int id)
        {
            return _context.Statements.Any(e => e.Id == id);
        }
        private bool StatementExists(string content)
        {
            return _context.Statements.Any(e => _context.Statements.Any(e => e.Content.ToLower()   == content.ToLower()));
        }
    }
}
