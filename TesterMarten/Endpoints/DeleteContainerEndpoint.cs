using FastEndpoints;
using Marten;
using TesterMarten.Models;

namespace TesterMarten.Endpoints;

public class DeleteContainerEndpoint : EndpointWithoutRequest
{
    private readonly IDocumentSession _session;

    public DeleteContainerEndpoint(IDocumentSession session) => _session = session;

    public override void Configure()
    {
        Delete("/api/container/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        _session.Delete<ContainerBase>(id);
        await _session.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}