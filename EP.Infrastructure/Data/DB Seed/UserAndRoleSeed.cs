using System.Security.Claims;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.AspNetCore.Identity;

namespace EP.Infrastructure.Data.DB_Seed
{
    public class UserAndRoleSeed : ISeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserAndRoleSeed(RoleManager<
                IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        public int Index { get; set; } = 1;

        public async Task Seed()
        {
            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(Role.AppUser.ToString()).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Role.SuperAdmin.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Role.AppUser.ToString())).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "LukaLom",
                    Email = "lukalomiashvili@gmail.com",
                    PhoneNumber = "111111123",
                    EmailConfirmed = true
                }, "String123!").GetAwaiter().GetResult();

                var user = await _userManager.FindByEmailAsync("lukalomiashvili@gmail.com");
                _userManager.AddToRoleAsync(user, Role.SuperAdmin.ToString()).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, Role.AppUser.ToString()).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, Role.Admin.ToString()).GetAwaiter().GetResult();

                var claimList = new List<Claim>()
                {
                    new("Role Management", "Role Management"),
                    new("Show Management", "Show Management")
                };
                await _userManager.AddClaimsAsync(user, claimList);
            }
        }
    }
}
