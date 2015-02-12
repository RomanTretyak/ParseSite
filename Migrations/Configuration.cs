using System.Data.Entity.Migrations;

namespace ParseSites.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<BaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "ParseSites.BaseContext";
        }

        protected override void Seed(BaseContext context)
        {
            context.Film.AddOrUpdate(
                p => p.Name,
                new Film { Name = "test"}
                );
        }
    }
}
