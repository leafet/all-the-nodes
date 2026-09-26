namespace EveryNode.Api.Models.ActionModels;

public class CreateUserRequest
{
    public required string Username { get; set; }
    public string? AdditionalData { get; set; }
}