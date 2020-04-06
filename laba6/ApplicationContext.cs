using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace laba6
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> context)
            : base(context)
        {
            Database.EnsureCreated();
        }

        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=students;Username=me;Password=1234");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Student>()
                .Property(p => p.FirstName)
                .HasMaxLength(30);

            builder.Entity<Student>()
                .Property(p => p.LastName)
                .HasMaxLength(30);

            builder.Entity<Student>()
                .Property(p => p.Group)
                .HasMaxLength(10);

            builder.Entity<Student>()
                .Property(p => p.CreatedAt)
                .HasDefaultValue(null);

            builder.Entity<Student>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValue(null);
        }
    }
}
