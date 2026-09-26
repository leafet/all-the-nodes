using System.Numerics;

namespace EveryNode.Api.Models;

public class Node
{
    public required int Id { get; set; }
    public required User Owner { get; set; }
    public required float X { get; set; } 
    public required float Y { get; set; }
}