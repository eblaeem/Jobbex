namespace ViewModel.UserEducation
{
    public class UserEducationResponse
    {
        public int? Id { get; set; }
        public string Field { get; set; }
        public string UniversityName { get; set; }
        public int? DegreeId { get; set; }
        public string Degree { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }


        public decimal? Score { get; set; }
        public string Description { get; set; }
    }
}
