namespace ViewModel.User
{
    public class RegisterEmailConfirmation
    {
        public string EmailSignature { set; get; }
        public string MessageDateTime { set; get; }
        public Entities.User User { set; get; }
        public string EmailConfirmationToken { set; get; }
        public string ApplicatioWebUrl { get; set; }
    }
}
