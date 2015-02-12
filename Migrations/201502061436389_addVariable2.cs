namespace ParseSites.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVariable2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Films", "Variable2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Films", "Variable2");
        }
    }
}
