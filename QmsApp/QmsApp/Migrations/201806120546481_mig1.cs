namespace QmsApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        AuditLogId = c.Guid(nullable: false),
                        EventType = c.String(),
                        TableName = c.String(nullable: false),
                        PrimaryKeyName = c.String(),
                        PrimaryKeyValue = c.String(),
                        ColumnName = c.String(),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        CreatedUser = c.Int(),
                        UpdatedDate = c.DateTime(),
                        CompanyId = c.Int(),
                    })
                .PrimaryKey(t => t.AuditLogId)
                .ForeignKey("dbo.CompanyProfiles", t => t.CompanyId)
                .ForeignKey("dbo.Users", t => t.CreatedUser)
                .Index(t => t.CompanyId)
                .Index(t => t.CreatedUser);
            
            CreateTable(
                "dbo.CompanyProfiles",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false),
                        CompanyAddress = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Tin = c.String(),
                        VatRegNo = c.String(),
                        WebSite = c.String(),
                        CompanyLogo = c.String(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        UserName = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        LastPassChangeDate = c.DateTime(),
                        PasswordChangedCount = c.Int(),
                        SupUser = c.Boolean(nullable: false),
                        Status = c.Byte(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        CompanyId = c.Int(),
                        Role_RoleId = c.Int(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.CompanyProfiles", t => t.CompanyId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Roles", t => t.Role_RoleId)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.CompanyId)
                .Index(t => t.CreatedBy)
                .Index(t => t.Role_RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.UpdatedBy)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false),
                        Status = c.Byte(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.UpdatedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.UpdatedBy);
            
            CreateTable(
                "dbo.RoleTasks",
                c => new
                    {
                        RoleTaskId = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        Task = c.String(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleTaskId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .Index(t => t.CreatedBy)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Counters",
                c => new
                    {
                        CounterId = c.Int(nullable: false, identity: true),
                        CountNo = c.String(),
                        UserId = c.Int(),
                        Status = c.Int(),
                        CreateBy = c.Int(),
                        CreateTime = c.DateTime(),
                        UpdateBy = c.Int(),
                        UpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.CounterId)
                .ForeignKey("dbo.Users", t => t.CreateBy)
                .ForeignKey("dbo.Users", t => t.UpdateBy)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.CreateBy)
                .Index(t => t.UpdateBy)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Phone = c.String(),
                        ServiceId = c.Int(),
                        Status = c.Int(),
                        CreateBy = c.Int(),
                        CreateTime = c.DateTime(),
                        UpdateBy = c.Int(),
                        UpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.CustomerId)
                .ForeignKey("dbo.Users", t => t.CreateBy)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .ForeignKey("dbo.Users", t => t.UpdateBy)
                .Index(t => t.CreateBy)
                .Index(t => t.ServiceId)
                .Index(t => t.UpdateBy);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        ServiceId = c.Int(nullable: false, identity: true),
                        ServiceName = c.String(),
                        PossibleServiceTime = c.Time(precision: 7),
                        Status = c.Int(),
                        CreateBy = c.Int(),
                        CreateTime = c.DateTime(),
                        UpdateBy = c.Int(),
                        UpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ServiceId)
                .ForeignKey("dbo.Users", t => t.CreateBy)
                .ForeignKey("dbo.Users", t => t.UpdateBy)
                .Index(t => t.CreateBy)
                .Index(t => t.UpdateBy);
            
            CreateTable(
                "dbo.Logins",
                c => new
                    {
                        LoginsId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        SessionId = c.String(),
                        LoggedIn = c.Boolean(nullable: false),
                        LoggedInDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.LoginsId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customers", "UpdateBy", "dbo.Users");
            DropForeignKey("dbo.Customers", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Services", "UpdateBy", "dbo.Users");
            DropForeignKey("dbo.Services", "CreateBy", "dbo.Users");
            DropForeignKey("dbo.Customers", "CreateBy", "dbo.Users");
            DropForeignKey("dbo.Counters", "UserId", "dbo.Users");
            DropForeignKey("dbo.Counters", "UpdateBy", "dbo.Users");
            DropForeignKey("dbo.Counters", "CreateBy", "dbo.Users");
            DropForeignKey("dbo.AuditLogs", "CreatedUser", "dbo.Users");
            DropForeignKey("dbo.AuditLogs", "CompanyId", "dbo.CompanyProfiles");
            DropForeignKey("dbo.CompanyProfiles", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.CompanyProfiles", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Users", "Role_RoleId", "dbo.Roles");
            DropForeignKey("dbo.Roles", "UpdatedBy", "dbo.Users");
            DropForeignKey("dbo.RoleTasks", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.RoleTasks", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Roles", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Users", "CompanyId", "dbo.CompanyProfiles");
            DropIndex("dbo.Customers", new[] { "UpdateBy" });
            DropIndex("dbo.Customers", new[] { "ServiceId" });
            DropIndex("dbo.Services", new[] { "UpdateBy" });
            DropIndex("dbo.Services", new[] { "CreateBy" });
            DropIndex("dbo.Customers", new[] { "CreateBy" });
            DropIndex("dbo.Counters", new[] { "UserId" });
            DropIndex("dbo.Counters", new[] { "UpdateBy" });
            DropIndex("dbo.Counters", new[] { "CreateBy" });
            DropIndex("dbo.AuditLogs", new[] { "CreatedUser" });
            DropIndex("dbo.AuditLogs", new[] { "CompanyId" });
            DropIndex("dbo.CompanyProfiles", new[] { "UpdatedBy" });
            DropIndex("dbo.CompanyProfiles", new[] { "CreatedBy" });
            DropIndex("dbo.Users", new[] { "User_UserId" });
            DropIndex("dbo.Users", new[] { "UpdatedBy" });
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropIndex("dbo.Users", new[] { "Role_RoleId" });
            DropIndex("dbo.Roles", new[] { "UpdatedBy" });
            DropIndex("dbo.RoleTasks", new[] { "RoleId" });
            DropIndex("dbo.RoleTasks", new[] { "CreatedBy" });
            DropIndex("dbo.Roles", new[] { "CreatedBy" });
            DropIndex("dbo.Users", new[] { "CreatedBy" });
            DropIndex("dbo.Users", new[] { "CompanyId" });
            DropTable("dbo.Logins");
            DropTable("dbo.Services");
            DropTable("dbo.Customers");
            DropTable("dbo.Counters");
            DropTable("dbo.RoleTasks");
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
            DropTable("dbo.CompanyProfiles");
            DropTable("dbo.AuditLogs");
        }
    }
}
