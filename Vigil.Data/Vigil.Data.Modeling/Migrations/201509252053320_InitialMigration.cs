namespace Vigil.Data.Modeling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "vigil.VigilRole",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(),
                        RoleType = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "vigil.VigilUserRole",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("vigil.VigilRole", t => t.RoleId)
                .ForeignKey("vigil.VigilUser", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "vigil.VigilUser",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "vigil.VigilUserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("vigil.VigilUser", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "vigil.VigilUserLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("vigil.VigilUser", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("vigil.VigilUserRole", "UserId", "vigil.VigilUser");
            DropForeignKey("vigil.VigilUserLogin", "UserId", "vigil.VigilUser");
            DropForeignKey("vigil.VigilUserClaim", "UserId", "vigil.VigilUser");
            DropForeignKey("vigil.VigilUserRole", "RoleId", "vigil.VigilRole");
            DropIndex("vigil.VigilUserLogin", new[] { "UserId" });
            DropIndex("vigil.VigilUserClaim", new[] { "UserId" });
            DropIndex("vigil.VigilUser", "UserNameIndex");
            DropIndex("vigil.VigilUserRole", new[] { "RoleId" });
            DropIndex("vigil.VigilUserRole", new[] { "UserId" });
            DropIndex("vigil.VigilRole", "RoleNameIndex");
            DropTable("vigil.VigilUserLogin");
            DropTable("vigil.VigilUserClaim");
            DropTable("vigil.VigilUser");
            DropTable("vigil.VigilUserRole");
            DropTable("vigil.VigilRole");
        }
    }
}
