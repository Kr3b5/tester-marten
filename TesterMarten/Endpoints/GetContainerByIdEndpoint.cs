using FastEndpoints;
using Marten;
using TesterMarten.Models;

namespace TesterMarten.Endpoints;

public class GetContainerByIdEndpoint : EndpointWithoutRequest<ContainerBase>
{
    private readonly IDocumentSession _session;

    public GetContainerByIdEndpoint(IDocumentSession session) => _session = session;

    public override void Configure()
    {
        Get("/api/container/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id"); // get manual because fastendpoints support no primitive types 

        var container = await _session.LoadAsync<ContainerBase>(id, ct);
        if (container is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(container, cancellation: ct);
    }
}
