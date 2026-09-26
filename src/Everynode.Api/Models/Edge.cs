namespace EveryNode.Api.Models;

public class Edge
{
    public required int Id { get; set; }
    public required Node Start { get; set; }
    public required Node End { get; set; }
}