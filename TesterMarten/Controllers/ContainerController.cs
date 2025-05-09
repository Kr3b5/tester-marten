using Marten;
using Microsoft.AspNetCore.Mvc;
using TesterMarten.Models;

namespace TesterMarten.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContainerController : ControllerBase
{
    private readonly IDocumentSession _session;
    public ContainerController(IDocumentSession session) => _session = session;

    [HttpGet]
    public async Task<IEnumerable<ContainerBase>> GetAll()
        => await _session.Query<ContainerBase>().ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<ContainerBase>> Get(int id)
    {
        var container = await _session.LoadAsync<ContainerBase>(id);
        return container == null ? NotFound() : Ok(container);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContainerBase container)
    {
        _session.Store(container);
        await _session.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = container.ID }, container);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ContainerBase updated)
    {
        // Load with polymorphism (Marten returns actual runtime type)
        var existing = await _session.LoadAsync<ContainerBase>(id);
        if (existing == null) return NotFound();

        Console.WriteLine($"Loaded existing type: {existing.GetType().Name}");
        Console.WriteLine($"Updated type: {updated.GetType().Name}");

        // Common base fields
        existing.ContainerName = updated.ContainerName;
        existing.Description = updated.Description;
        existing.Type = updated.Type;
        existing.shortID = updated.shortID;
        existing.cID = updated.cID;

        // Type-specific fields (CompilerContainer, etc.)
        if (existing is CompilerContainer compiler && updated is CompilerContainer u1)
        {
            compiler.CompilerURL = u1.CompilerURL;
        }
        else if (existing is DocumentContainer document && updated is DocumentContainer u2)
        {
            document.DocumentId = u2.DocumentId;
        }
        else
        {
            return BadRequest("Mismatched or unsupported container types.");
        }

        // Save tracked document
        _session.Store(existing); 
        await _session.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _session.Delete<ContainerBase>(id);
        await _session.SaveChangesAsync();
        return NoContent();
    }
}