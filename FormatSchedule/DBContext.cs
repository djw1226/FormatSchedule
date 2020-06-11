using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace FormatSchedule
{
    public class DatabaseContext : DbContext
    {
        
        public DbSet<Event> Events { get; set; }
        public DbSet<Team> Teams { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=GBADB;Trusted_Connection=True;");
        }
    }
}
