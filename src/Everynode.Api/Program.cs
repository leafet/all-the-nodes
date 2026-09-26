using EveryNode.Api;
using EveryNode.Api.MockData;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => CreateRandomField.Create(3, 5));

app.Run();
