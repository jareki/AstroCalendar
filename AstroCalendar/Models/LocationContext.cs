using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroCalendar.Models
{
    public class LocationContext:DbContext
    {
        public DbSet<Location> Locations { get; set; }

        public LocationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=sunmoon.db");
        }
        
    }
}
