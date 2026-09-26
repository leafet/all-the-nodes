using EveryNode.Api.Models;

namespace EveryNode.Api;

public class Field
{
    public required List<Node> Nodes { get; set; }
    public required List<Edge> Edges { get; set; }
}