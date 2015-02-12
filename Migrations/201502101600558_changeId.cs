namespace ParseSites.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeId : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Roles", new[] { "Film_FilmID" });
            CreateIndex("dbo.Roles", "Film_FilmId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Roles", new[] { "Film_FilmId" });
            CreateIndex("dbo.Roles", "Film_FilmID");
        }
    }
}
