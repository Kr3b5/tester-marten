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
    public async Task<IActionResult> Update(int id, ContainerBase updated)
    {
        var existing = await _session.LoadAsync<ContainerBase>(id);
        if (existing == null) return NotFound();

        _session.Store(updated);
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