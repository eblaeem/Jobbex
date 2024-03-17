using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class EducationLevel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set;}
        public string Name { get; set;}
    }
}
