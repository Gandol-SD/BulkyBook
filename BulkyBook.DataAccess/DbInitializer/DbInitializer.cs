using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbx;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext dbx)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbx = dbx;
        }

        public void init()
        {
            // Apply migrations if they are not applied
            try
            {
                if(_dbx.Database.GetPendingMigrations().Count() > 0)
                {
                    _dbx.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }

            // create roles if not created
            if (!_roleManager.RoleExistsAsync(StaticDetailes.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(StaticDetailes.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetailes.Role_User_Indie)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetailes.Role_User_Comp)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetailes.Role_Employee)).GetAwaiter().GetResult();

                // if roles are created, then we will create admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "SuperBulky",
                    Email = "dev.djvan@gmail.com",
                    Name = "SuperBulky",
                    PhoneNumber = "09564585324",
                    StreetAddress = "Str33t",
                    State = "St4t3",
                    City = "CeTi",
                    PostalCode = "11111",
                }, "123A123a@").GetAwaiter().GetResult();

                ApplicationUser user = _dbx.ApplicationUsers.FirstOrDefault(u => u.Email == "dev.djvan@gmail.com");
                _userManager.AddToRoleAsync(user, StaticDetailes.Role_Admin).GetAwaiter().GetResult();
            }



        }
    }
}











