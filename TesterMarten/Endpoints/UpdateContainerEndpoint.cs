using FastEndpoints;
using Marten;
using TesterMarten.Models;

namespace TesterMarten.Endpoints;

public class UpdateContainerEndpoint : EndpointWithoutRequest
{
    private readonly IDocumentSession _session;

    public UpdateContainerEndpoint(IDocumentSession session) => _session = session;

    public override void Configure()
    {
        Put("/api/container/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        var req = await HttpContext.Request.ReadFromJsonAsync<ContainerBase>();

        if (req is null)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var existing = await _session.LoadAsync<ContainerBase>(id, ct);
        if (existing is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Update shared fields
        existing.ContainerName = req.ContainerName;
        existing.Description = req.Description;
        existing.Type = req.Type;
        existing.shortID = req.shortID;
        existing.cID = req.cID;

        if (existing is CompilerContainer compiler && req is CompilerContainer u1)
        {
            compiler.CompilerURL = u1.CompilerURL;
        }
        else if (existing is DocumentContainer doc && req is DocumentContainer u2)
        {
            doc.DocumentId = u2.DocumentId;
        }

        _session.Store(existing);
        await _session.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}