namespace ViewModel.Job
{
    public class JobFilter : PagingModel
    {
        public int? Id { get; set; }
        public string Term { get; set; }
        public List<int?> JobGroups { get; set; }
        public List<int?> Cities { get; set; }
        public List<int?> ContractTypes { get; set; }
        public List<int?> SalaryRequests { get; set; }
        public List<int?> WorkExperienceYears { get; set; }
    }
}
