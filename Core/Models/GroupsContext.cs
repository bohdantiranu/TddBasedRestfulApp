using Microsoft.EntityFrameworkCore;

namespace Core.Models
{
    public class GroupsContext : DbContext
    {
        public DbSet<Group> Groups { get; set; }

        //public GroupsContext(DbContextOptions<GroupsContext> options) : base(options) //Work with config
        //{
        //    Database.EnsureCreated();
        //}
        public GroupsContext()//For tests
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=myTestdb;Trusted_Connection=True;");
        }
    }
}