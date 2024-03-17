namespace ViewModel.User
{
    public class AddUserTokenRequest
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshTokenSerial { get; set; }
        public string RefreshTokenSourceSerial { get; set; }
        public string IpAddress { get; set; }
        public string BrowserName { get; set; }
    }
}
