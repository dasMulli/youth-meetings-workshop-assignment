using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }

    public class ConfigurationValue
    {
        public static class KnownKeys
        {
            public static string FirstChoiceAssignmentCostDefault = nameof(FirstChoiceAssignmentCostDefault);
            public static string SecondChoiceAssignmentCostDefault = nameof(SecondChoiceAssignmentCostDefault);
            public static string ThirdChoiceAssignmentCostDefault = nameof(ThirdChoiceAssignmentCostDefault);
        }

        [Key]
        public string Key { get; set; } = string.Empty;

        public float? FloatValue { get; set; }
    }

    public class Workshop
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MinParticipants { get; set; } = 10;

        public int MaxParticipants { get; set; } = 12;
    }

    public class Participant
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Workshop? FirstChoice { get; set; }

        public Workshop? SecondChoice { get; set; }

        public Workshop? ThirdChoice { get; set; }

        public float? FirstChoiceAssignmentCostOverride { get; set; }

        public float? SecondChoiceAssignmentCostOverride { get; set; }

        public float? ThirdChoiceAssignmentCostOverride { get; set; }
    }
}
