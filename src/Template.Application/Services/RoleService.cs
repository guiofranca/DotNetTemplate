using Microsoft.Extensions.Logging;
using Template.Application.DTO.Role;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;

namespace Template.Application.Services;

public class RoleService : BaseService<RoleModel>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    public RoleService(IUnitOfWork unitOfWork,
                       IErrorNotificator errorNotificator,
                       ICacheService cache,
                       ILogger<RoleService> logger,
                       IGlobalizer globalizer,
                       IUserRepository userRepository,
                       IRoleRepository roleRepository) : base(unitOfWork, errorNotificator, cache, logger, globalizer)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IServiceResult<IEnumerable<RoleModel>>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.FindAllAsync();
        var models = roles.Select(r => new RoleModel {
            Id = r.Id,
            Name = r.Name,
        });

        return OkResult<IEnumerable<RoleModel>>(models);
    }

    public async Task<IServiceResult<RoleModel>> AddRoleToUserAsync(RoleRequest roleRequest)
    {
        var user = await _userRepository.FindAsync(roleRequest.UserId);
        if (user == null) return NotFoundResult(_g["User not found"]);

        var userRoles = await _roleRepository.GetAsync(user);
        if (userRoles.Any(r => r.Id == roleRequest.RoleId)) return FailureResult(_g["User already has the role"]);

        var role = await _roleRepository.FindAsync(roleRequest.RoleId);
        if (role == null) return NotFoundResult(_g["Role not found"]);

        var success = await _roleRepository.AddAsync(user, role);

        if (success)
        {
            var model = new RoleModel { Id = role.Id, Name = role.Name };
            return OkResult(model, _g["Role {0} added to user", role.Name]);
        }

        return FailureResult(_g["Failed to set role"]);
    }

    public async Task<IServiceResult<RoleModel>> RemoveRoleFromUserAsync(RoleRequest roleRequest)
    {
        var user = await _userRepository.FindAsync(roleRequest.UserId);
        if (user == null) return NotFoundResult(_g["User not found"]);

        var role = await _roleRepository.FindAsync(roleRequest.RoleId);
        if (role == null) return NotFoundResult(_g["Role not found"]);

        var success = await _roleRepository.RemoveAsync(user, role);

        if (success)
        {
            var model = new RoleModel { Id = role.Id, Name = role.Name };
            return OkResult(model, _g["Role {0} removed fom user", role.Name]);
        }

        return FailureResult(_g["Failed to remove role"]);
    }
}
