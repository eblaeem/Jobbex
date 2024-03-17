using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class SmsOutboxType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
