using EP.Application.Services.Account.Role;
using EP.Application.Services.Account.Role.DTO.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers.Account
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,SuperAdmin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRoles();

            if (roles.IsSuccess)
            {
                return Ok(roles);
            }

            return BadRequest(roles.Error);
        }

        [HttpPost]
        [Authorize(Policy = "RoleManagementPolicy")]
        public async Task<IActionResult> CreateRole(string name)
        {
            var result = await _roleService.CreateRole(name);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpDelete]
        [Authorize(Policy = "RoleManagementPolicy")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var result = await _roleService.DeleteRole(roleName);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersFilterDto filterQuery)
        {
            var users = await _roleService.GetAllUsers(filterQuery);

            if (users.IsSuccess)
            {
                return Ok(users);
            }

            return BadRequest(users.Error);
        }

        [HttpPost]
        [Authorize(Policy = "RoleManagementPolicy")]
        public async Task<IActionResult> AddUserToRole(AddUserToRoleRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }
            var result = await _roleService.AddUserToRole(requestDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Error);

        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var roles = await _roleService.GetUserRoles(email);

            if (roles.IsSuccess)
            {
                return Ok(roles);
            }

            return BadRequest(roles.Error);
        }

        [HttpDelete]
        [Authorize(Policy = "RoleManagementPolicy")]
        public async Task<IActionResult> RemoveUserFromRole(RemoveUserFromRoleRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var result = await _roleService.RemoveUserFromRole(requestDto);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        private IActionResult BadRequestModelState()
        {
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

            return BadRequest(new { error = errorMessages });
        }
    }
}
