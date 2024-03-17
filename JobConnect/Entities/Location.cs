using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Location
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? LocationTypeId { get; set; }
        public int? ParentId { get; set; }
        public bool? IsCenter { get; set; }
        public string? Code { get; set; }
    }
}
