using EveryNode.Api.DataStore;
using EveryNode.Api.Models;
using EveryNode.Api.Models.ActionModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>
    (options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

var app = builder.Build();

app.MapPost("/AddUser", async (CreateUserRequest createUserRequest, AppDbContext db) =>
{
    User newUser = new User
    {
        Username = createUserRequest.Username,
        NodesBudget = 3,
        EdgesBudget = 5
    };
    newUser.AdditionalData = createUserRequest.AdditionalData;

    await db.Users.AddAsync(newUser);
    await db.SaveChangesAsync();
});

app.MapGet("/Field", async (AppDbContext db) =>
{
    List<EdgeResponse> nodes = await db.Edges.Select(edge => new EdgeResponse
    {
        Id = edge.Id,
        StartId = edge.Start.Id,
        EndId = edge.End.Id
    }).ToListAsync();
    
    List<NodeResponse> edges = await db.Nodes.Select(node => new NodeResponse
    {
        Id = node.Id,
        OwnerID = node.Owner.Id,
        X = node.X,
        Y = node.Y
    }).ToListAsync();
    
    FieldResponse field = new FieldResponse
    {
        Edges = nodes,
        Nodes = edges
    };
    
    return field;
});

app.MapGet("/users", async (AppDbContext db) => await db.Users.ToListAsync());

app.MapPost("/AddEdge", async (CreateEdgeRequest req, AppDbContext db) =>
{
    //Временное получение пользователя, далее будет ID с браузера
    List<User> users = await db.Users.ToListAsync();
    
    User placingUser = users.First(u => u.Id == 1);
    
    if (!db.Nodes.Select(n => n.Id).Contains(req.StartId))
        return Results.BadRequest($"Can't find start node {req.StartId}");
    
    if (!db.Nodes.Select(n => n.Id).Contains(req.EndId))
        return Results.BadRequest($"Can't find end node {req.EndId}");
    
    Node startNode = await db.Nodes.FirstAsync(n => n.Id == req.StartId);
    Node endNode = await db.Nodes.FirstAsync(n => n.Id == req.EndId);
    
    if (placingUser.EdgesBudget == 0)
        return Results.BadRequest($"{placingUser.Username} Insufficient budget");
    
    if (req.StartId == req.EndId)
        return Results.BadRequest($"{placingUser.Username} Cannot connect node to itself");
    
    if (startNode.Owner.Id != placingUser.Id && endNode.Owner.Id != placingUser.Id)
        return Results.BadRequest($"{placingUser.Username} Cannot connect two not owned nodes");
    
    if (db.Edges.Where(edge => edge.Start.Id == req.StartId && edge.End.Id == req.EndId).ToList().Count != 0 ||
        db.Edges.Where(edge => edge.Start.Id == req.EndId && edge.End.Id == req.StartId).ToList().Count != 0)
        return Results.BadRequest($"{placingUser.Username} Cannot connect two connected nodes");

    Edge edgeToAdd = new Edge
    {
        Start = startNode,
        End = endNode,
    };
    
    placingUser.EdgesBudget -= 1;
    
    await db.Edges.AddAsync(edgeToAdd);
    await db.SaveChangesAsync();
    
    return Results.Ok(edgeToAdd);
});

app.MapPost("/addNode", async (CreateNodeRequest pos, AppDbContext db) =>
{   
    //Временное получение пользователя, далее будет ID с браузера
    List<User> users = await db.Users.ToListAsync();
    
    User placingUser = users.First();
    
    if (placingUser.NodesBudget == 0)
        return Results.BadRequest($"{placingUser.Username} Insufficient budget");
    
    Node nodeToAdd = new Node
    {
        Owner = placingUser,
        X = pos.X,
        Y = pos.Y
    };
    
    placingUser.NodesBudget -= 1;
    
    await db.Nodes.AddAsync(nodeToAdd);
    await db.SaveChangesAsync();
    
    return Results.Ok(nodeToAdd);

});

app.Run();
