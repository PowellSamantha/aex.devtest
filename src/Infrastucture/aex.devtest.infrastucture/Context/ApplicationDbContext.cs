using aex.devtest.infrastucture.Entities;
using Microsoft.EntityFrameworkCore;

namespace aex.devtest.infrastucture.Context
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Vehicle> Vehicle { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }
    }
}