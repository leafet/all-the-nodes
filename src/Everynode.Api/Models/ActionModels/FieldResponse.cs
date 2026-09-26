namespace EveryNode.Api.Models.ActionModels;

public class FieldResponse
{
    public required List<NodeResponse> Nodes { get; set; }
    public required List<EdgeResponse> Edges { get; set; }
}