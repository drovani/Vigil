namespace Vigil.Data.Modeling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatronPersonInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "vigil.ChangeLogs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        SourceId = c.Guid(nullable: false),
                        ModelName = c.String(nullable: false),
                        PropertyName = c.String(nullable: false),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        CreatedBy_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("vigil.VigilUser", t => t.CreatedBy_Id)
                .Index(t => t.CreatedBy_Id);
            
            CreateTable(
                "vigil.Patron",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountNumber = c.String(),
                        DisplayName = c.String(nullable: false, maxLength: 250),
                        IsAnonymous = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        DeletedOn = c.DateTime(),
                        CreatedBy_Id = c.Guid(nullable: false),
                        DeletedBy_Id = c.Guid(),
                        ModifiedBy_Id = c.Guid(),
                        PatronType_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("vigil.VigilUser", t => t.CreatedBy_Id)
                .ForeignKey("vigil.VigilUser", t => t.DeletedBy_Id)
                .ForeignKey("vigil.VigilUser", t => t.ModifiedBy_Id)
                .ForeignKey("vigil.PatronType", t => t.PatronType_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.DeletedBy_Id)
                .Index(t => t.ModifiedBy_Id)
                .Index(t => t.PatronType_Id);
            
            CreateTable(
                "vigil.PatronType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IsOrganization = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        DeletedOn = c.DateTime(),
                        TypeName = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Ordinal = c.Int(nullable: false),
                        CreatedBy_Id = c.Guid(nullable: false),
                        DeletedBy_Id = c.Guid(),
                        ModifiedBy_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("vigil.VigilUser", t => t.CreatedBy_Id)
                .ForeignKey("vigil.VigilUser", t => t.DeletedBy_Id)
                .ForeignKey("vigil.VigilUser", t => t.ModifiedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.DeletedBy_Id)
                .Index(t => t.ModifiedBy_Id);
            
            CreateTable(
                "vigil.Person",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FullName_Title = c.String(maxLength: 250),
                        FullName_GivenName = c.String(maxLength: 250),
                        FullName_MiddleName = c.String(maxLength: 250),
                        FullName_FamilyName = c.String(maxLength: 250),
                        FullName_Suffix = c.String(maxLength: 250),
                        DateOfBirth = c.DateTime(),
                        DateOfBirthAccuracy_Accuracy = c.String(maxLength: 3, fixedLength: true, unicode: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        DeletedOn = c.DateTime(),
                        CreatedBy_Id = c.Guid(nullable: false),
                        DeletedBy_Id = c.Guid(),
                        ModifiedBy_Id = c.Guid(),
                        Patron_Id = c.Guid(nullable: false),
                        PersonType_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("vigil.VigilUser", t => t.CreatedBy_Id)
                .ForeignKey("vigil.VigilUser", t => t.DeletedBy_Id)
                .ForeignKey("vigil.VigilUser", t => t.ModifiedBy_Id)
                .ForeignKey("vigil.Patron", t => t.Patron_Id)
                .ForeignKey("vigil.PersonType", t => t.PersonType_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.DeletedBy_Id)
                .Index(t => t.ModifiedBy_Id)
                .Index(t => t.Patron_Id)
                .Index(t => t.PersonType_Id);
            
            CreateTable(
                "vigil.PersonType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AllowMultiplePerPatron = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                        DeletedOn = c.DateTime(),
                        TypeName = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Ordinal = c.Int(nullable: false),
                        CreatedBy_Id = c.Guid(nullable: false),
                        DeletedBy_Id = c.Guid(),
                        ModifiedBy_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("vigil.VigilUser", t => t.CreatedBy_Id)
                .ForeignKey("vigil.VigilUser", t => t.DeletedBy_Id)
                .ForeignKey("vigil.VigilUser", t => t.ModifiedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.DeletedBy_Id)
                .Index(t => t.ModifiedBy_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("vigil.Person", "PersonType_Id", "vigil.PersonType");
            DropForeignKey("vigil.PersonType", "ModifiedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PersonType", "DeletedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PersonType", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Person", "Patron_Id", "vigil.Patron");
            DropForeignKey("vigil.Person", "ModifiedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Person", "DeletedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Person", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Patron", "PatronType_Id", "vigil.PatronType");
            DropForeignKey("vigil.PatronType", "ModifiedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PatronType", "DeletedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.PatronType", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Patron", "ModifiedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Patron", "DeletedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.Patron", "CreatedBy_Id", "vigil.VigilUser");
            DropForeignKey("vigil.ChangeLogs", "CreatedBy_Id", "vigil.VigilUser");
            DropIndex("vigil.PersonType", new[] { "ModifiedBy_Id" });
            DropIndex("vigil.PersonType", new[] { "DeletedBy_Id" });
            DropIndex("vigil.PersonType", new[] { "CreatedBy_Id" });
            DropIndex("vigil.Person", new[] { "PersonType_Id" });
            DropIndex("vigil.Person", new[] { "Patron_Id" });
            DropIndex("vigil.Person", new[] { "ModifiedBy_Id" });
            DropIndex("vigil.Person", new[] { "DeletedBy_Id" });
            DropIndex("vigil.Person", new[] { "CreatedBy_Id" });
            DropIndex("vigil.PatronType", new[] { "ModifiedBy_Id" });
            DropIndex("vigil.PatronType", new[] { "DeletedBy_Id" });
            DropIndex("vigil.PatronType", new[] { "CreatedBy_Id" });
            DropIndex("vigil.Patron", new[] { "PatronType_Id" });
            DropIndex("vigil.Patron", new[] { "ModifiedBy_Id" });
            DropIndex("vigil.Patron", new[] { "DeletedBy_Id" });
            DropIndex("vigil.Patron", new[] { "CreatedBy_Id" });
            DropIndex("vigil.ChangeLogs", new[] { "CreatedBy_Id" });
            DropTable("vigil.PersonType");
            DropTable("vigil.Person");
            DropTable("vigil.PatronType");
            DropTable("vigil.Patron");
            DropTable("vigil.ChangeLogs");
        }
    }
}
