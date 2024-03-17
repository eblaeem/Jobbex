namespace Entities
{
    public class AccessCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string? Url { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateTime { get; set; }

    }
}
