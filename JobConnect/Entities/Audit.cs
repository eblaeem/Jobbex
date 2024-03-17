namespace Entities
{
    public class Audit
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string? TypeName { get; set; }
        public string? KeyValues { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime DateTime { get; set; }
        public string? IpAddress { get; set; }
        public int? UserId { get; set; }
    }
}
