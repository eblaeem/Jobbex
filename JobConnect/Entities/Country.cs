using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Country
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string PersianName { get; set; }
        public string? EnglishName { get; set; }
    }
}
