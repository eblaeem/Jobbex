namespace Entities
{
    public class UserPriority
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Cities { get; set; }
        public string Groups { get; set; }
        public string ContractTypes { get; set; }
        public string Benefits { get; set; }
        public int? SalaryRequestedId { get; set; }
        public virtual User User { get; set; }
    }
}
