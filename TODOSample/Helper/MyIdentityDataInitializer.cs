using DAL;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TODOSample
{
    public class MyIdentityDataInitializer
    {
        public static void SeedData(UserManager<User> userManager,RoleManager<Role> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByEmailAsync("sa@mail.com").Result == null)
            {
                User user = new User();
                user.UserName = "sa@mail.com";
                user.Email = "sa@mail.com";
                user.FirstName = "Super";
                user.LastName = "Admin";

                IdentityResult result = userManager.CreateAsync
                (user, "Super@123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Helper.EnumRoles.SA.ToString()).Wait();
                }
            }

            if (userManager.FindByEmailAsync("demo@mail.com").Result == null)
            {
                User user = new User();
                user.UserName = "demo@mail.com";
                user.Email = "demo@mail.com";
                user.FirstName = "Demo";
                user.LastName = "User";

                IdentityResult result = userManager.CreateAsync
                (user, "Demo@123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Helper.EnumRoles.User.ToString()).Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<Role> roleManager)
        {
            if (!roleManager.RoleExistsAsync(Helper.EnumRoles.SA.ToString()).Result)
            {
                Role role = new Role();
                role.Name = Helper.EnumRoles.SA.ToString();
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync(Helper.EnumRoles.User.ToString()).Result)
            {
                Role role = new Role();
                role.Name = Helper.EnumRoles.User.ToString();
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
    }
}
