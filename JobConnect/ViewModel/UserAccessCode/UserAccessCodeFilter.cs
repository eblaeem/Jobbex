namespace ViewModel.UserAccessCode
{
    public class UserAccessCodeFilter
    {
        public int UserId { get; set; }
        public int? AccessCodeId { get; set; }
        public bool? Granted { get; set; }
    }
}
