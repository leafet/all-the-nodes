using EveryNode.Api.DataStore;
using EveryNode.Api.Models;
using EveryNode.Api.Models.ActionModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>
    (options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Everynode.Browser";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/session", async (HttpContext http, AppDbContext db) =>
{
    var idText = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (int.TryParse(idText, out var id))
    {
        var existingUser = await db.Users.FindAsync(id);
        if (existingUser is not null)
        {
            return Results.Ok(new
            {
                existingUser.Id,
                existingUser.NodesBudget,
                existingUser.EdgesBudget
            });
        }
    }

    var user = new User
    {
        Username = $"Guest-{Guid.NewGuid():N}",
        NodesBudget = 3,
        EdgesBudget = 6
    };
    
    db.Users.Add(user);
    
    await db.SaveChangesAsync();
    
    var identity = new ClaimsIdentity(
        [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())],
        CookieAuthenticationDefaults.AuthenticationScheme);
    
    await http.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity),
        new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        });
    
    return Results.Ok(new { user.Id, user.NodesBudget, user.EdgesBudget });
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

app.MapPost("/AddEdge", async (HttpContext http, CreateEdgeRequest req, AppDbContext db) =>
{
    var idText = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    if (!int.TryParse(idText, out var userId))
        return Results.Unauthorized();
    
    var placingUser = await db.Users.FindAsync(userId);

    if (placingUser is null)
        return Results.Unauthorized();
    
    if (!db.Nodes.Select(n => n.Id).Contains(req.StartId))
        return Results.BadRequest($"Can't find start node {req.StartId}");
    
    if (!db.Nodes.Select(n => n.Id).Contains(req.EndId))
        return Results.BadRequest($"Can't find end node {req.EndId}");
    
    Node startNode = await db.Nodes.Include(node => node.Owner).FirstAsync(n => n.Id == req.StartId);
    Node endNode = await db.Nodes.Include(node => node.Owner).FirstAsync(n => n.Id == req.EndId);
    
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
        Owner = placingUser,
        Start = startNode,
        End = endNode,
    };
    
    placingUser.EdgesBudget -= 1;
    
    await db.Edges.AddAsync(edgeToAdd);
    await db.SaveChangesAsync();
    
    return Results.Ok(edgeToAdd);
}).RequireAuthorization();

app.MapPost("/addNode", async (HttpContext http, CreateNodeRequest pos, AppDbContext db) =>
{   
    var idText = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    if (!int.TryParse(idText, out var userId))
        return Results.Unauthorized();
    
    var placingUser = await db.Users.FindAsync(userId);

    if (placingUser is null)
        return Results.Unauthorized();

    
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

}).RequireAuthorization();

app.Run();
