using EP.Application.DTO_General.Generic;
using EP.Application.Services.Account.Role.DTO.Request;
using EP.Application.Services.Account.Role.DTO.Response;
using EP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace EP.Application.Services.Account.Role
{
    public interface IRoleService
    {
        Task<Result<string>> DeleteRole(string roleName);
        Task<Result<List<IdentityRole>>> GetAllRoles();
        Task<Result<RoleResult>> CreateRole(string name);
        Task<PagedResult<ApplicationUser>> GetAllUsers(GetUsersFilterDto filterQuery);
        Task<Result<RoleResult>> AddUserToRole(AddUserToRoleRequestDto requestDto);
        Task<Result<IList<string>>> GetUserRoles(string email);
        Task<Result<string>> RemoveUserFromRole(RemoveUserFromRoleRequestDto requestDto);
    }
}
