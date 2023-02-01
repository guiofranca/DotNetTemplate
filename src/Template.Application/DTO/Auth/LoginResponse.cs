using Template.Application.DTO.User;

namespace Template.Application.DTO.Auth;

public record LoginResponse
{
    public required string Token { get; set; }
    public required UserModel User { get; set; }
}

