using FastEndpoints;
using FastEndpoints.Swagger;
using Marten;
using TesterMarten.Converters;
using TesterMarten.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddMarten(opts =>
{
    opts.Connection("Host=localhost;Database=container;Username=admin;Password=admin");
    opts.Schema.For<ContainerBase>()
        .AddSubClass<CompilerContainer>()
        .AddSubClass<DocumentContainer>()
        .Identity(x => x.ID)
        .Duplicate(x => x.cID)
        .Duplicate(x => x.shortID)
        .Duplicate(x => x.ContainerName)
        .Duplicate(x => x.Description)
        .Duplicate(x => x.Type);
})
    .UseLightweightSessions();

/*
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ContainerBaseJsonConverter());
});
*/

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new ContainerBaseJsonConverter());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
    await store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

    // Warm up query system
    using var session = store.LightweightSession();
    _ = await session.Query<ContainerBase>().Take(1).ToListAsync();
}


// app.MapControllers();  

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();