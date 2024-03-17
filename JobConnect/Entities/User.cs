namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string NationalCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public int Age { get; set; }
        public byte Gender { get; set; }
        public byte MilitrayStateId { get; set; }
        public byte MaritalStateId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public bool IsActive { get; set; }
        public int? SalaryRequestedId { get; set; }
        public int? ExpectedJobId { get; set; }

        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public string TwoFactorCode { get; set; }
        public bool EnableTwoStepVerification { get; set; }
        public string ResetPasswordToken { get; set; }
        public bool Hidden { get; set; }

        public DateTime? LastLoggedIn { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? ChangePasswordDateTime { get; set; }
        public DateTime? BirthDate { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }
        public virtual ICollection<UserPinJob> UserPinJobs { get; set; }
    }
}
