namespace ViewModel.User
{
    public class ResetPasswordModel
    {
        public string UserName { get; set; }
        public string ApplicatioWebUrl { get; set; }
        public string ApplicationName { get; set; }
        public string EmailConfirmationToken { get; set; }
        public string MessageDateTime { get; set; }
    }
}
