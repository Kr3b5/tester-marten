using Marten;
using TesterMarten.Converters;
using TesterMarten.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ContainerBaseJsonConverter());
});

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
});

var app = builder.Build();
app.MapControllers();
app.Run();