namespace ViewModel.User
{
    public class LoginResponse
    {
        public LoginResponse()
        {

        }
        public LoginResponse(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string NationalCode { get; set; }
        public string RedirectUrl { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string AntiForgeryToken { get; set; }
        public bool UserMustBeChanged { get; set; }
    }
}
