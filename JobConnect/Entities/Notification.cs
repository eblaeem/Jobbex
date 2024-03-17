namespace Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public int InsertedUserId { get; set; }
    }
}
