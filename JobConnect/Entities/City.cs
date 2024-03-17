using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class City
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string PersianName { get; set; }
        public string? EnglishName { get; set; }
        public int? StateId { get; set; }
        public int? LocationTypeId { get; set; }
    }
}
