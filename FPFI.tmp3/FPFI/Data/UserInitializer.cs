using FPFI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Data
{
    public class UserInitializer
    {
        public static Task Initialize(ApplicationDbContext context)
        {
            var roleStore = new RoleStore<ApplicationRole>(context);
            var userStore = new UserStore<ApplicationUser>(context);

            if (!context.ApplicationUserRoles.Any())
            {
                if (!context.Users.Any())
                {
                    if (!context.Roles.Any())
                    {
                        List<ApplicationRole> applicationRoles = new List<ApplicationRole>() { };
                        foreach (var role in RoleData.ApplicationRoles)
                        {
                            applicationRoles.Add(new ApplicationRole
                            {
                                CreatedDate = DateTime.Now,
                                Name = role,
                                Description = role,
                                NormalizedName = role.ToUpper(),
                                IPAddress = "::1"
                            });
                        }

                        foreach (var role in applicationRoles)
                        {
                            context.Roles.Add(role);
                        }
                    }

                    var user = new ApplicationUser
                    {
                        Name = "Admin",
                        Last = "Superuser",
                        UserName = "superuser@fpfi.cl",
                        NormalizedUserName = "superuser@fpfi.cl",
                        Email = "guille.arp@gmail.com",
                        NormalizedEmail = "superuser@fpfi.cl",
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        MemberSince = DateTime.Now
                    };

                    var hasher = new PasswordHasher<ApplicationUser>();
                    var hashedPassword = hasher.HashPassword(user, "4dm1n");
                    user.PasswordHash = hashedPassword;

                    foreach (var claim in ClaimData.UserClaims)
                    {
                        user.Claims.Add(new IdentityUserClaim<string>
                        {
                            ClaimType = claim,
                            ClaimValue = claim
                        });
                    }
                    //userStore.Create(user);

                    context.Users.Add(user);

                    context.SaveChanges();
                }

                var rol = context.Roles.SingleOrDefault(m => m.Name == "Administrator");

                var usuario = context.Users.SingleOrDefault(m => m.UserName == "superuser@fpfi.cl");

                var userRole = new ApplicationUserRole
                {
                    UserId = usuario.Id,
                    RoleId = rol.Id
                };

                context.ApplicationUserRoles.Add(userRole);

                context.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}
