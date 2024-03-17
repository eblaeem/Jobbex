namespace ViewModel.UserAccessCode
{
    public class UserAccessCodeResponse
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? AccessCodeId { get; set; }
        public string AccessCodeName { get; set; }
        public int? AccessCodeNumber { get; set; }
        public bool Granted { get; set; }
    }
}
