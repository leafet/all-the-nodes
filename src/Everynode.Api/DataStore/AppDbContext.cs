using EveryNode.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EveryNode.Api.DataStore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Edge> Edges => Set<Edge>();
    public DbSet<Node> Nodes => Set<Node>();
    public DbSet<User> Users => Set<User>();
}