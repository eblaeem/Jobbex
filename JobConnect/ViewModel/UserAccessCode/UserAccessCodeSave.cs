using FluentValidation;
public class UserAccessCodeSave
{
    public int UserId { get; set; }
    public int AccessCodeId { get; set; }
    public bool Type { get; set; }
}
public class UserAccessCodeSaveValidator : AbstractValidator<UserAccessCodeSave>
{
    public UserAccessCodeSaveValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().NotNull();
        RuleFor(x => x.AccessCodeId).NotEmpty().NotNull();
    }
}
