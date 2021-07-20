using System.ComponentModel.DataAnnotations;

namespace WorkshopAssignmentApp.Data
{
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
}
