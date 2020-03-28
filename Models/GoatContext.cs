using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _21GoatBackend.Models
{
    public class GoatContext : DbContext    
    {
        public GoatContext(DbContextOptions<GoatContext> options)
           : base(options)
        {
        }

        public DbSet<Statement> Statements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
           => options.UseSqlite("Data Source=Data\\statements.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Statement>().HasData(
            new Statement() { Id = 1, Content= "Erstes Statement", Language="DE"  }
            );
        }
    }
}
