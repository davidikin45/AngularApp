using AspNetCore.ApiBase.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class AppContext : DbContextBase
    {
        public DbSet<Value> Values { get; set; }

        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if(optionsBuilder.IsConfigured == false)
            {

            }
        }

        public override void BuildQueries(ModelBuilder builder)
        {

        }

        public override void Seed()
        {
            DbSeed.Seed(this);
        }
    }
}
