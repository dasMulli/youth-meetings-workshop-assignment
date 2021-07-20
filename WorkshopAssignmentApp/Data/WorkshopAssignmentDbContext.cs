using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WorkshopAssignmentApp.Data
{
    public class WorkshopAssignmentDbContext : DbContext
    {
        public WorkshopAssignmentDbContext(DbContextOptions<WorkshopAssignmentDbContext> dbContextOptions)
            : base(dbContextOptions)
        {}

        public DbSet<ConfigurationValue> ConfigurationValues { get; set; } = null!;

        public DbSet<Workshop> Workshops { get; set; } = null!;

        public DbSet<Participant> Participants { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var participantBuilder = modelBuilder.Entity<Participant>();

            participantBuilder.Navigation(p => p.FirstChoice).AutoInclude();
            participantBuilder.Navigation(p => p.SecondChoice).AutoInclude();
            participantBuilder.Navigation(p => p.ThirdChoice).AutoInclude();
        }
    }
}
