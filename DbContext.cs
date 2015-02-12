using System.Data.Entity;

namespace ParseSites
{
    class BaseContext : DbContext
    {
        public BaseContext()
            : base("DbContext")
        {

        }
        public DbSet<Film> Film { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}
