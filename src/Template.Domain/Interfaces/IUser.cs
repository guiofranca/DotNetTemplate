using System.Security.Claims;

namespace Template.Domain.Interfaces;

public interface IUser
{
    Guid Id { get; }
    bool IsAuthenticated { get; }
    string? Token {  get; }
    bool IsInRole(string role);
}
