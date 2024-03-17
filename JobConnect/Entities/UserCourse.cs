namespace Entities
{
    public class UserCourse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CourseName { get; set; }
        public string InstituteName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Score { get; set; }
    }
}
