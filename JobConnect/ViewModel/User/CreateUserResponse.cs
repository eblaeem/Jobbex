namespace ViewModel.User
{
    public class CreateUserResponse : ResponseBase
    {
        public CreateUserResponse()
        {
        }
        public CreateUserResponse(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
        public string RedirectUrl { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
