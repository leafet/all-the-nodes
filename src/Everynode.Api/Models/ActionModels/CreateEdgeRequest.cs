namespace EveryNode.Api.Models.ActionModels;

public class CreateEdgeRequest
{
    public required int StartId { get; set; }
    public required int EndId { get; set; }
}