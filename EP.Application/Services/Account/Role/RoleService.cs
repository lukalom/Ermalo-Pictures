using EP.Application.DTO_General.Errors;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Extensions;
using EP.Application.Services.Account.Role.DTO.Request;
using EP.Application.Services.Account.Role.DTO.Response;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Shared.Exceptions.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EP.Application.Services.Account.Role
{

    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RoleService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        public async Task<Result<List<IdentityRole>>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var result = new Result<List<IdentityRole>>();
            if (roles.Any())
            {
                result.Content = roles;
                return result;
            }

            result.Error = new Error()
            {
                Code = (int)StatusCode.NotFound,
                Message = "No roles in Db",
                Type = ErrorMessages.Generic.TypeBadRequest
            };

            return result;
        }

        public async Task<Result<RoleResult>> CreateRole(string name)
        {
            var result = new Result<RoleResult>();
            var roleExist = await _roleManager.RoleExistsAsync(name);

            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole() { Name = name });

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"The Role {name} has been added successfully");

                    result.Content = new RoleResult()
                    {
                        Success = roleResult.Succeeded,
                        Role = name
                    };
                    return result;
                }
            }

            _logger.LogInformation($"The Role {name} has not been added");
            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.BadRequest,
                ErrorMessages.Generic.InvalidPayload,
                ErrorMessages.Generic.TypeBadRequest);

            return result;
        }

        public async Task<Result<string>> DeleteRole(string roleName)
        {
            var result = new Result<string>();
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var roleResult = await _roleManager.DeleteAsync(role);

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"The Role {roleName} has been added Deleted");
                    result.Content = $"The Role {roleName} has been added Deleted";
                    return result;
                }

            }

            _logger.LogInformation($"The Role {roleName} has not been added");
            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.BadRequest,
                ErrorMessages.Generic.InvalidPayload,
                ErrorMessages.Generic.TypeBadRequest);

            return result;
        }

        public async Task<PagedResult<ApplicationUser>> GetAllUsers(GetUsersFilterDto filterQuery)
        {
            var result = new PagedResult<ApplicationUser>();
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filterQuery.UserId) || !string.IsNullOrEmpty(filterQuery.Email))
            {
                var users = await usersQuery.Where(u =>
                    u.Email == filterQuery.Email || u.Id == filterQuery.UserId).ToListAsync();
                result.Items = users;
                return result;
            }

            if (!string.IsNullOrEmpty(filterQuery.UserName))
            {
                var filteredUsers = await _userManager.Users.AsQueryable()
                    .Where(u => u.UserName.Contains(filterQuery.UserName.ToLowerInvariant()))
                    .ToListAsync();

                result.Items = filteredUsers;
                return result;
            }

            result = await _userManager.Users
                .PaginateAsync(filterQuery.Page, filterQuery.PageSize);

            if (result.IsSuccess) return result;

            result.Error = new Error()
            {
                Code = (int)StatusCode.NotFound,
                Message = ErrorMessages.Users.UsersNotFound,
                Type = ErrorMessages.Generic.TypeBadRequest
            };

            return result;
        }

        public async Task<Result<RoleResult>> AddUserToRole(AddUserToRoleRequestDto requestDto)
        {
            var result = new Result<RoleResult>();
            var user = await _userManager.FindByEmailAsync(requestDto.Email);

            if (user == null)
            {
                _logger.LogInformation($"The user with the {requestDto.Email} does not exists");
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Users.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var roleExist = await _roleManager.RoleExistsAsync(requestDto.RoleName);

            if (!roleExist)
            {
                _logger.LogInformation($"The role {requestDto.RoleName} does not exists");
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.NotFound,
                    ErrorMessages.Users.RoleDoesNotExist,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var resultTask = await _userManager.AddToRoleAsync(user, requestDto.RoleName);

            if (resultTask.Succeeded)
            {
                _logger.LogInformation($"The Role {requestDto.RoleName} has been added successfully");
                result.Content = new RoleResult()
                {
                    Role = $"{user.UserName} added role {requestDto.RoleName}",
                    Success = resultTask.Succeeded
                };

                return result;
            }
            _logger.LogInformation($"The User was not able to be added to the role");
            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.NotFound,
                "The User was not able to be added to the role",
                ErrorMessages.Generic.TypeBadRequest);
            return result;
        }

        public async Task<Result<IList<string>>> GetUserRoles(string email)
        {
            var result = new Result<IList<string>>();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The user with the {email} does not exists");
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Users.UsersNotFound,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            result.Content = userRoles;

            return result;
        }

        public async Task<Result<string>> RemoveUserFromRole(RemoveUserFromRoleRequestDto requestDto)
        {
            var result = new Result<string>();

            var user = await _userManager.FindByEmailAsync(requestDto.Email);
            if (user == null)
            {
                _logger.LogInformation($"The user with the {requestDto.Email} does not exists");
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.NotFound,
                    ErrorMessages.Users.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var roleExist = await _roleManager.RoleExistsAsync(requestDto.RoleName);

            if (!roleExist)
            {
                _logger.LogInformation($"The user with the {requestDto.Email} does not exists");
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.NotFound,
                    ErrorMessages.Users.RoleDoesNotExist,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var removeResult = await _userManager.RemoveFromRoleAsync(user, requestDto.RoleName);

            if (removeResult.Succeeded)
            {
                result.Content = $"user {requestDto.Email} has been removed from role {requestDto.RoleName}";
                return result;
            }

            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.InternalServerError,
                $"Unable to  remove user {requestDto.Email} from role {requestDto.RoleName}",
                ErrorMessages.Generic.UnableToProcess);

            return result;

        }
    }
}
