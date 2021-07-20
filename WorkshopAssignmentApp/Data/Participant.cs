namespace WorkshopAssignmentApp.Data
{
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
