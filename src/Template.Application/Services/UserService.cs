using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.DTO.Role;
using Template.Application.DTO.User;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Core.Interfaces;
using Template.Core.Interfaces.Repositories;
using Template.Core.Models;

namespace Template.Application.Services;

public class UserService : BaseService<UserModel>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    public UserService(IUnitOfWork unitOfWork,
                       IErrorNotificator errorNotificator,
                       ICacheService cache,
                       ILogger<UserService> logger,
                       IGlobalizer globalizer,
                       IUserRepository userRepository,
                       IRoleRepository roleRepository) : base(unitOfWork, errorNotificator, cache, logger, globalizer)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IServiceResult<UserModel>> GetAsync(Guid id)
    {
        var user = await _cache.RememberModelAsync(id, _userRepository.FindAsync);
        if (user == null) return NotFoundResult(_g["User not found"]);

        var model = MapToUserModel(user);
        return OkResult(model);
    }

    public async Task<IServiceResult<DetailedUserModel>> GetDetailedAsync(Guid id)
    {
        var user = await _cache.RememberModelAsync(id, _userRepository.FindAsync);
        if (user == null) return NotFoundResult<DetailedUserModel>(_g["User not found"]);

        var roles = await _roleRepository.GetAsync(user);

        var model = MapToDetailedUserModel(user, roles);
        return OkResult(model);
    }

    public async Task<IServiceResult<IEnumerable<UserModel>>> GetAllAsync()
    {
        var users = await _userRepository.FindAllAsync();
        var models = users.Select(MapToUserModel);

        return OkResult(models);
    }

    public async Task<IServiceResult<IEnumerable<DetailedUserModel>>> GetAllDetailedAsync()
    {
        var users = await _userRepository.FindAllAsync();

        var roles = Enumerable.Empty<Role>();

        var models = users.Select(u => MapToDetailedUserModel(u, roles));
        return OkResult(models);
    }

    private static UserModel MapToUserModel(User user) => new() { Id = user.Id, Name = user.Name };
    private static RoleModel MapToRoleModel(Role role) => new() { Id = role.Id, Name = role.Name };
    private static DetailedUserModel MapToDetailedUserModel(User user, IEnumerable<Role> roles) 
        => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Verified = user.Verified,
            Roles = roles.Select(MapToRoleModel)
        };

}
