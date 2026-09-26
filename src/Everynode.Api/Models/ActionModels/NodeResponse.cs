namespace EveryNode.Api.Models.ActionModels;

public class NodeResponse
{
    public required int Id { get; set; }
    public required int OwnerID { get; set; }
    public required float X { get; set; } 
    public required float Y { get; set; }
}