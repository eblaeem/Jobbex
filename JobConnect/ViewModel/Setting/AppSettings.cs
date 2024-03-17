namespace ViewModel.Setting
{
    public class AppSettings
    {
        public string Output { get; set; }
        public string OutputApiAddress { get; set; }
        public string OutputApiKey { get; set; }

        public string ApplicationName { get; set; }
        public string ApplicationPersianName { get; set; }
        public string ApplicationAdminUrl { get; set; }
        public string ApplicationUrl { get; set; }


        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public string RefreshTokenPath { get; set; }
        public string AccessTokenObjectKey { get; set; }
        public string RefreshTokenObjectKey { get; set; }
        public string AdminRoleName { get; set; }
        public bool EnableEmailConfirmation { get; set; }
        public bool EnableTwoStepVerification { get; set; }
        public bool EnableTwoStepSmsVerification { get; set; }
        public int NotAllowedPreviouslyUsedPasswords { get; set; }
        public int ChangePasswordReminderDays { get; set; }

        public string SmsNumber { get; set; }
        public string SmsUserName { get; set; }
        public string SmsPassword { get; set; }
        public string SmsProvider { get; set; }

        public string ApplicatioWebUrl { get; set; }
        public string ApplicationSite { get; set; }
        public string ApplicatioApiUrl { get; set; }

        public int? CurrentBrokerId { get; set; }

        public List<string> UserBlockList { get; set; }
    }
    public class SecretAppSetting
    {
        public string Password { get; set; }
        public string Scope { get; set; }
    }
}