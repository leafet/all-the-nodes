namespace EveryNode.Api.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    
    public required int NodesBudget { get; set; }
    public required int EdgesBudget { get; set; }
    
    public string? AdditionalData { get; set; }
}