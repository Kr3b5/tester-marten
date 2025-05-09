using FastEndpoints;
using Marten;
using TesterMarten.Models;

namespace TesterMarten.Endpoints;

public class GetAllContainersEndpoint : EndpointWithoutRequest<IEnumerable<ContainerBase>>
{
    private readonly IDocumentSession _session;

    public GetAllContainersEndpoint(IDocumentSession session) => _session = session;

    public override void Configure()
    {
        Get("/api/container");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var containers = await _session.Query<ContainerBase>().ToListAsync(ct);
        await SendAsync(containers, cancellation: ct);
    }
}