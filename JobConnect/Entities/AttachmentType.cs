using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class AttachmentType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? EnglishName { get; set; }
    }
}
