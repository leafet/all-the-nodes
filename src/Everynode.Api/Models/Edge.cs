using Microsoft.EntityFrameworkCore;

namespace EveryNode.Api.Models;

[Index(nameof(Start.Id), nameof(End.Id), IsUnique = true)]
public class Edge
{
    public int Id { get; set; }
    
    public required User Owner { get; set; }
    public required Node Start { get; set; }
    public required Node End { get; set; }
}