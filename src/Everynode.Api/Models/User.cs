namespace EveryNode.Api;

public class User
{
    public required int Id { get; set; }
    public required string Username { get; set; }
    
    public required int NodesBudget { get; set; }
    
    public string? AdditionalData { get; set; }
}