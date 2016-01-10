namespace Vigil.Data.Modeling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_CreatedBy_ModifedBy_DeletedBy_To_String : DbMigration
    {
        public override void Up()
        {
            DropIndex("vigil.ChangeLogs", new[] { "CreatedBy_Id" });
            DropIndex("vigil.Patron", new[] { "CreatedBy_Id" });
            DropIndex("vigil.Patron", new[] { "DeletedBy_Id" });
            DropIndex("vigil.Patron", new[] { "ModifiedBy_Id" });
            DropIndex("vigil.PatronType", new[] { "CreatedBy_Id" });
            DropIndex("vigil.PatronType", new[] { "DeletedBy_Id" });
            DropIndex("vigil.PatronType", new[] { "ModifiedBy_Id" });
            DropIndex("vigil.Person", new[] { "CreatedBy_Id" });
            DropIndex("vigil.Person", new[] { "DeletedBy_Id" });
            DropIndex("vigil.Person", new[] { "ModifiedBy_Id" });
            DropIndex("vigil.PersonType", new[] { "CreatedBy_Id" });
            DropIndex("vigil.PersonType", new[] { "DeletedBy_Id" });
            DropIndex("vigil.PersonType", new[] { "ModifiedBy_Id" });
            DropForeignKey("vigil.ChangeLogs", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Patron", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Patron", "DeletedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Patron", "ModifiedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PatronType", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PatronType", "DeletedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PatronType", "ModifiedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Person", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Person", "DeletedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Person", "ModifiedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PersonType", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PersonType", "DeletedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PersonType", "ModifiedBy_Id", "vigil.VigilUser");
            DropColumn("vigil.ChangeLogs", "CreatedBy_Id");
            DropColumn("vigil.Patron", "CreatedBy_Id");
            DropColumn("vigil.Patron", "DeletedBy_Id");
            DropColumn("vigil.Patron", "ModifiedBy_Id");
            DropColumn("vigil.PatronType", "CreatedBy_Id");
            DropColumn("vigil.PatronType", "DeletedBy_Id");
            DropColumn("vigil.PatronType", "ModifiedBy_Id");
            DropColumn("vigil.Person", "CreatedBy_Id");
            DropColumn("vigil.Person", "DeletedBy_Id");
            DropColumn("vigil.Person", "ModifiedBy_Id");
            DropColumn("vigil.PersonType", "CreatedBy_Id");
            DropColumn("vigil.PersonType", "DeletedBy_Id");
            DropColumn("vigil.PersonType", "ModifiedBy_Id");
            CreateTable(
                "vigil.ApplicationSetting",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SettingName = c.String(),
                        SettingValue = c.String(),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "vigil.Comment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EntityId = c.Guid(nullable: false),
                        CommentText = c.String(),
                        CreatedBy = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        PatronState_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("vigil.Patron", t => t.PatronState_Id)
                .Index(t => t.PatronState_Id);

            RenameTable(name: "vigil.ChangeLogs", newName: "ChangeLog");
            AddColumn("vigil.ChangeLog", "CreatedBy", c => c.String(nullable: false));
            AddColumn("vigil.Patron", "CreatedBy", c => c.String(nullable: false));
            AddColumn("vigil.Patron", "ModifiedBy", c => c.String());
            AddColumn("vigil.Patron", "DeletedBy", c => c.String());
            AddColumn("vigil.PatronType", "CreatedBy", c => c.String(nullable: false));
            AddColumn("vigil.PatronType", "ModifiedBy", c => c.String());
            AddColumn("vigil.PatronType", "DeletedBy", c => c.String());
            AddColumn("vigil.Person", "CreatedBy", c => c.String(nullable: false));
            AddColumn("vigil.Person", "ModifiedBy", c => c.String());
            AddColumn("vigil.Person", "DeletedBy", c => c.String());
            AddColumn("vigil.PersonType", "CreatedBy", c => c.String(nullable: false));
            AddColumn("vigil.PersonType", "ModifiedBy", c => c.String());
            AddColumn("vigil.PersonType", "DeletedBy", c => c.String());
        }
        
        public override void Down()
        {
            AddColumn("vigil.PersonType", "ModifiedBy_Id", c => c.Guid());
            AddColumn("vigil.PersonType", "DeletedBy_Id", c => c.Guid());
            AddColumn("vigil.PersonType", "CreatedBy_Id", c => c.Guid(nullable: false));
            AddColumn("vigil.Person", "ModifiedBy_Id", c => c.Guid());
            AddColumn("vigil.Person", "DeletedBy_Id", c => c.Guid());
            AddColumn("vigil.Person", "CreatedBy_Id", c => c.Guid(nullable: false));
            AddColumn("vigil.PatronType", "ModifiedBy_Id", c => c.Guid());
            AddColumn("vigil.PatronType", "DeletedBy_Id", c => c.Guid());
            AddColumn("vigil.PatronType", "CreatedBy_Id", c => c.Guid(nullable: false));
            AddColumn("vigil.Patron", "ModifiedBy_Id", c => c.Guid());
            AddColumn("vigil.Patron", "DeletedBy_Id", c => c.Guid());
            AddColumn("vigil.Patron", "CreatedBy_Id", c => c.Guid(nullable: false));
            AddColumn("vigil.ChangeLog", "CreatedBy_Id", c => c.Guid(nullable: false));
            DropForeignKey("vigil.Comment", "PatronState_Id", "vigil.Patron");
            DropIndex("vigil.Comment", new[] { "PatronState_Id" });
            DropColumn("vigil.PersonType", "DeletedBy");
            DropColumn("vigil.PersonType", "ModifiedBy");
            DropColumn("vigil.PersonType", "CreatedBy");
            DropColumn("vigil.Person", "DeletedBy");
            DropColumn("vigil.Person", "ModifiedBy");
            DropColumn("vigil.Person", "CreatedBy");
            DropColumn("vigil.PatronType", "DeletedBy");
            DropColumn("vigil.PatronType", "ModifiedBy");
            DropColumn("vigil.PatronType", "CreatedBy");
            DropColumn("vigil.Patron", "DeletedBy");
            DropColumn("vigil.Patron", "ModifiedBy");
            DropColumn("vigil.Patron", "CreatedBy");
            DropColumn("vigil.ChangeLog", "CreatedBy");
            DropTable("vigil.Comment");
            DropTable("vigil.ApplicationSetting");
            CreateIndex("vigil.PersonType", "ModifiedBy_Id");
            CreateIndex("vigil.PersonType", "DeletedBy_Id");
            CreateIndex("vigil.PersonType", "CreatedBy_Id");
            CreateIndex("vigil.Person", "ModifiedBy_Id");
            CreateIndex("vigil.Person", "DeletedBy_Id");
            CreateIndex("vigil.Person", "CreatedBy_Id");
            CreateIndex("vigil.PatronType", "ModifiedBy_Id");
            CreateIndex("vigil.PatronType", "DeletedBy_Id");
            CreateIndex("vigil.PatronType", "CreatedBy_Id");
            CreateIndex("vigil.Patron", "ModifiedBy_Id");
            CreateIndex("vigil.Patron", "DeletedBy_Id");
            CreateIndex("vigil.Patron", "CreatedBy_Id");
            CreateIndex("vigil.ChangeLog", "CreatedBy_Id");
            AddForeignKey("vigil.PersonType", "ModifiedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.PersonType", "DeletedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.PersonType", "CreatedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.Person", "ModifiedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.Person", "DeletedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.Person", "CreatedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.PatronType", "ModifiedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.PatronType", "DeletedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.PatronType", "CreatedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.Patron", "ModifiedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.Patron", "DeletedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.Patron", "CreatedBy_Id", "vigil.VigilUser", "Id");
            AddForeignKey("vigil.ChangeLogs", "CreatedBy_Id", "vigil.VigilUser", "Id");
            RenameTable(name: "vigil.ChangeLog", newName: "ChangeLogs");
        }
    }
}
