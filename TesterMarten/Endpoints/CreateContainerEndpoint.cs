using FastEndpoints;
using Marten;
using TesterMarten.Models;

namespace TesterMarten.Endpoints;

public class CreateContainerEndpoint : EndpointWithoutRequest<ContainerBase>
{
    private readonly IDocumentSession _session;

    public CreateContainerEndpoint(IDocumentSession session) => _session = session;

    public override void Configure()
    {
        Post("/api/container");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var container = await HttpContext.Request.ReadFromJsonAsync<ContainerBase>(cancellationToken: ct);
        if (container is null)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        _session.Store(container);
        await _session.SaveChangesAsync(ct);
        await SendAsync(container, 201, ct);
    }
}