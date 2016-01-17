namespace Vigil.Data.Modeling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_State_From_Names : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "vigil.Comment", name: "PatronState_Id", newName: "Patron_Id");
            RenameIndex(table: "vigil.Comment", name: "IX_PatronState_Id", newName: "IX_Patron_Id");
            CreateTable(
                "vigil.PersonType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AllowMultiplePerPatron = c.Boolean(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        TypeName = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Ordinal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("vigil.ChangeLog", "EntityId", c => c.Guid(nullable: false));
            AlterColumn("vigil.ApplicationSetting", "SettingName", c => c.String(nullable: false));
            AlterColumn("vigil.ApplicationSetting", "SettingValue", c => c.String(nullable: false));
            AlterColumn("vigil.Comment", "CommentText", c => c.String(nullable: false));
            DropColumn("vigil.ChangeLog", "SourceId");
            DropTable("vigil.PersonType");
        }
        
        public override void Down()
        {
            CreateTable(
                "vigil.PersonType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AllowMultiplePerPatron = c.Boolean(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        TypeName = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Ordinal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("vigil.ChangeLog", "SourceId", c => c.Guid(nullable: false));
            AlterColumn("vigil.Comment", "CommentText", c => c.String());
            AlterColumn("vigil.ApplicationSetting", "SettingValue", c => c.String());
            AlterColumn("vigil.ApplicationSetting", "SettingName", c => c.String());
            DropColumn("vigil.ChangeLog", "EntityId");
            DropTable("vigil.PersonType");
            RenameIndex(table: "vigil.Comment", name: "IX_Patron_Id", newName: "IX_PatronState_Id");
            RenameColumn(table: "vigil.Comment", name: "Patron_Id", newName: "PatronState_Id");
        }
    }
}
