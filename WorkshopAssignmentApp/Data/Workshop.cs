namespace WorkshopAssignmentApp.Data
{
    public class Workshop
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MinParticipants { get; set; } = 10;

        public int MaxParticipants { get; set; } = 12;
    }
}
