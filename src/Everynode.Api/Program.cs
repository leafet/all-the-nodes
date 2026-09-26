using System.Numerics;
using EveryNode.Api;
using EveryNode.Api.Models;
using EveryNode.Api.Models.DTOs;
using static EveryNode.Api.MockData.MockDataUtils;

PopulateRandomUsers();

int GlobalNodesIdStore = 0;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<User> appUsers = GetRandomUsers();

Field globalField = new Field{Nodes = new List<Node>(), Edges = new List<Edge>()};

User testPlacingUser = appUsers[0];

//PopulateField(10, 15, globalField);

app.MapGet("/", () => globalField);

app.MapGet("/users", () => appUsers);

app.MapGet("/reloadTestData", () =>
{
    testPlacingUser = GetRandomUser(appUsers);
});

app.MapPost("/addNode", (NodePositionDTO pos) =>
{
    User placingUser = testPlacingUser;
    
    if (placingUser.NodesBudget == 0)
        return Results.BadRequest($"{placingUser.Username} Insufficient budget");
    
    Node nodeToAdd = new Node
    {
        Id = GlobalNodesIdStore++,
        Owner = placingUser,
        X = pos.X,
        Y = pos.Y
    };
    
    placingUser.NodesBudget -= 1;
    
    globalField.Nodes.Add(nodeToAdd);
    
    return Results.Ok(nodeToAdd);

});

app.Run();
