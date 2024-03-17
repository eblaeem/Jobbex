namespace Entities
{
    public class UserJob
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public int? JobGroupId { get; set; }
        public int? PositionId { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public bool? IsCurrentJob { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
