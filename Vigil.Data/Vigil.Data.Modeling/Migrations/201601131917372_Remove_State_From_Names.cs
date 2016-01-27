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
            AddColumn("vigil.ChangeLog", "EntityId", c => c.Guid(nullable: false));
            AlterColumn("vigil.ApplicationSetting", "SettingName", c => c.String(nullable: false));
            AlterColumn("vigil.ApplicationSetting", "SettingValue", c => c.String(nullable: false));
            AlterColumn("vigil.Comment", "CommentText", c => c.String(nullable: false));
            DropColumn("vigil.ChangeLog", "SourceId");
        }
        
        public override void Down()
        {
            AddColumn("vigil.ChangeLog", "SourceId", c => c.Guid(nullable: false));
            AlterColumn("vigil.Comment", "CommentText", c => c.String());
            AlterColumn("vigil.ApplicationSetting", "SettingValue", c => c.String());
            AlterColumn("vigil.ApplicationSetting", "SettingName", c => c.String());
            DropColumn("vigil.ChangeLog", "EntityId");
            RenameIndex(table: "vigil.Comment", name: "IX_Patron_Id", newName: "IX_PatronState_Id");
            RenameColumn(table: "vigil.Comment", name: "Patron_Id", newName: "PatronState_Id");
        }
    }
}
