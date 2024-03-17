namespace Entities
{
    public class UserEducation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Field { get; set; }
        public string UniversityName { get; set; }
        public int? DegreeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Score { get; set; }
        public string Description { get; set; }

    }
}
