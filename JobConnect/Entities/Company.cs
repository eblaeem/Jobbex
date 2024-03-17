namespace Entities
{
    public class Company
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string EnglishName { get; set; }

        public int? JobGroupId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Phone { get; set; }
        public string WebSite { get; set; }
        public string ZoneId { get; set; }

        public int? CreatedYear { get; set; }
        public int? OrganizationSizeId { get; set; }
        public string Description { get; set; }
        public string ServiceAndProducs { get; set; }

        public int? AttachmentLogoId { get; set; }
        public int? AttachmentBackgroundId { get; set; }
        public decimal? Rate { get; set; }
    }
}
