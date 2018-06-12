using System.Collections.Generic;
using QmsApp.Models;

namespace QmsApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<QmsApp.Models.QmsDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(QmsApp.Models.QmsDbContext context)
        {
            //var companyProfile = new List<CompanyProfile>
            //{
            //    new CompanyProfile {CompanyName = "Encoders Infotech Ltd.",CompanyAddress = "Mohakhali DOHS, Bangladesh"}

            //};
            //companyProfile.ForEach(s => context.CompanyProfiles.AddOrUpdate(c => c.CompanyName, s));
            //context.SaveChanges("");

            //var roles = new List<Role>
            //{
            //    new Role {RoleName = "SuperAdmin", Status=2}
            //};
            //roles.ForEach(s => context.Roles.AddOrUpdate(r => r.RoleName, s));
            //context.SaveChanges("");

            //var deptRoles = new List<RoleTask>
            //{
            //    new RoleTask {RoleId = 1, Task = "Global_SupAdmin"}
            //};
            //deptRoles.ForEach(s => context.RoleTasks.AddOrUpdate(r => r.Task, s));
            //context.SaveChanges("");

            //var users = new List<User>
            //{
            //    new User {RoleId = 1, UserName = "administrator",Password = "72b9c9b28b41a635e93cab3ab558ee15",SupUser = true, CompanyId = 1,Status=2},//p@werp
            //    new User {RoleId = 1, UserName = "dev",Password = "90f2c9c53f66540e67349e0ab83d8cd0",SupUser = true,CompanyId = 1, Status=2}, //p@ssword
            //      };
            //users.ForEach(s => context.Users.AddOrUpdate(u => u.UserName, s));
            //context.SaveChanges("");
        }
    }
}
