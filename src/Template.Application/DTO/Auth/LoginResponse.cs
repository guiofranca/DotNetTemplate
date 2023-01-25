using Template.Application.DTO.User;

namespace Template.Application.DTO.Auth;

public record LoginResponse
{
    public string Token { get; set; }
    public UserModel User { get; set; }
}

