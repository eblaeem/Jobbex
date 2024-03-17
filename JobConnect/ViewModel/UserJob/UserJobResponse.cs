namespace ViewModel.UserJob
{
    public class UserJobResponse
    {
        public int? Id { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public int? JobGroupId { get; set; }
        public int? PositionId { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public bool? IsCurrentJob { get; set; }

        public string City { get; set; }
        public string Position { get; set; }
        public string JobGroupName { get; set; }    
    }
}
