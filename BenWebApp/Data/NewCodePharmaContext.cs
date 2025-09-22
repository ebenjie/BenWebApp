using BenWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BenWebApp.Data
{
    public class NewCodePharmaContext: DbContext
    {
        public NewCodePharmaContext(DbContextOptions<NewCodePharmaContext> options) : base(options) { }

        public DbSet<NewItemPharmaModel> NewItemPharma {  get; set; }
        public DbSet<CodePerDeptModel> CodePerDepts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CodePerDeptModel>()
                .ToTable("NewCodePerDept");  // Maps entity to your existing table
        }

    }
}
