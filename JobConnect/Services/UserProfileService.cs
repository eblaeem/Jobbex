using Azure;
using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ViewModel;
using ViewModel.Attachment;
using ViewModel.User;
using ViewModel.UserEducation;

namespace Services
{
    public interface IUserProfileService
    {
        Task<UserProfile> Get();
        Task<UserProfile> GetByUserId(int userId);
        Task<ResponseBase> Save(UserProfile request);
        Task<ResponseBase> UploadProfile(int? id, IFormFile file);
    }
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<User> _users;
        private readonly ICommonService _commonService;
        private readonly IAttachmentService _attachmentService;
        private readonly IStateService _stateService;
        private readonly ILanguageService _languageService;
        private readonly ILanguageLevelService _languageLevelService;
        private readonly IJobPositionsService _jobPositionsService;
        private readonly IJobGroupsService _jobGroupsService;
        private readonly ISoftWareSkillsService _softWareSkillsService;
        private readonly IUserEducationService _userEducationService;
        private readonly IUserJobService _userJobService;
        private readonly IUserLanguageService _userLanguageService;
        private readonly IUserSoftWareSkillsService _userSoftWareSkillsService;
        private readonly IUserPriorityService _userPriorityService;
        public UserProfileService(IUnitOfWork unitOfWork,
            ICommonService commonService,
            IAttachmentService attachmentService,
            IStateService stateService,
            ILanguageService languageService,
            ILanguageLevelService languageLevelService,
            IJobPositionsService jobPositionsService,
            IJobGroupsService jobGroupsService,
            ISoftWareSkillsService softWareSkillsService,
            IUserEducationService userEducationService,
            IUserJobService userJobService,
            IUserLanguageService userLanguageService,
            IUserSoftWareSkillsService userSoftWareSkillsService,
            IUserPriorityService userPriorityService)
        {
            _uow = unitOfWork;
            _users = _uow.Set<User>();
            _commonService = commonService;
            _attachmentService = attachmentService;
            _stateService = stateService;
            _languageService = languageService;
            _languageLevelService = languageLevelService;
            _jobPositionsService = jobPositionsService;
            _jobGroupsService = jobGroupsService;
            _softWareSkillsService = softWareSkillsService;

            _userEducationService = userEducationService;
            _userJobService = userJobService;
            _userLanguageService = userLanguageService;
            _userSoftWareSkillsService = userSoftWareSkillsService;
            _userPriorityService = userPriorityService;
        }
        public async Task<UserProfile> Get()
        {
            var user = await _commonService.GetCurrentUser();
            var response = await GetProfile(user);
            return response;
        }
        public async Task<UserProfile> GetByUserId(int userId)
        {
            var user = await _users.FindAsync(userId); //todo check company access user
            var response = await GetProfile(user);
            return response;
        }

        public async Task<ResponseBase> Save(UserProfile request)
        {

            var user = await _commonService.GetCurrentUser();

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Age = (int)request.Age;
            user.DisplayName = $"{request.FirstName} {request.LastName}";
            user.Gender = request.GenderId.GetValueOrDefault();
            user.MaritalStateId = request.MaritalStateId.GetValueOrDefault();
            user.MilitrayStateId = request.MilitaryStateId.GetValueOrDefault();
            user.CityId = request.CityId.GetValueOrDefault();
            user.StateId = request.StateId;
            user.SalaryRequestedId = request.SalaryRequestedId;
            user.ExpectedJobId = request.ExpectedJobId;
            if (string.IsNullOrEmpty(request.BirthDate) == false)
            {
                user.BirthDate = request.BirthDate.ToGregorianDateTime();
            }
            user.Description = request.Description;

            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
        public async Task<ResponseBase> UploadProfile(int? id, IFormFile file)
        {
            var user = await _commonService.GetCurrentUser();
            if (user == null)
            {
                return new ResponseBase(false, "لطفا مجددا وارد شوید");
            }

            var random = new Random();
            var response = await _attachmentService.Save(new AttachmentSave()
            {
                AttachmentTypeId = 1,
                ImageOrPdf = true,
                FormFile = file,
                FileName = file.FileName,
                FileType = file.ContentType,
                FileSize = Convert.ToInt32(file.Length),
                UserId = user.Id,
                RecordId = user.Id,
                Id = id
            });

            return response;
        }

        private async Task<UserProfile> GetProfile(User user)
        {
            if (user == null)
            {
                return null;
            }
            var response = new UserProfile()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DisplayName = user.DisplayName,
                LastLoggedIn = user.LastLoggedIn.ToShortPersianDateString(),
                NationalCode = user.NationalCode,
                UserName = user.Username,
                ChangePasswordDateTimeString = user.ChangePasswordDateTime.ToShortPersianDateTimeString(),
                GenderId = user.Gender,

                MilitaryStateId = user.MilitrayStateId,
                MilitaryStateStateName = "",
                MaritalStateId = user.MaritalStateId,
                CityId = user.CityId,
                StateId = user.StateId,
                BirthDate = user.BirthDate.ToShortPersianDateString(),
                ExpectedJobId = user.ExpectedJobId,
                SalaryRequestedId = user.SalaryRequestedId,
                Description = user.Description
            };
            switch (user.Gender)
            {
                case 1:
                    response.GenderName = "مرد";
                    break;
                case 2:
                    response.GenderName = "زن";
                    break;
                default:
                    response.GenderName = "نامشخص";
                    break;
            }

            var userImages = await _attachmentService.Get(new AttachmentFilter()
            {
                AttachmentTypes = new List<int> { 1 },
                WithData = true,
                UserId = user.Id,
                RecordId = user.Id
            });
            if (userImages.Any())
            {
                response.UserProfileImage = "data:image/png;base64," + userImages.FirstOrDefault().FileData;
                response.UserProfileImageId = userImages.FirstOrDefault().Id;
            }

            response.Cites = await _stateService.GetGroupStates();
            response.SalaryRequesteds = _commonService.GetSalaryRequesteds();
            response.Genders = _commonService.GetGenders();
            response.MilitaryStatus = _commonService.GetMilitaryStatus();
            response.MaritalStatus = _commonService.GetMaritalStatus();
            response.Degrees = _commonService.GetDegrees();
            response.Languages = await _languageService.Get();
            response.LanguageLevels = await _languageLevelService.Get();
            response.JobPositions = await _jobPositionsService.Get();
            response.JobGroups = await _jobGroupsService.Get();
            response.Skills = await _softWareSkillsService.Get();
            response.SkillLevels = _commonService.GetSkillLevels();
            response.ContractTypes = _commonService.GetContractTypes();
            response.JobBenefits = _commonService.GetJobBenefits();

            if (response.MaritalStateId.GetValueOrDefault() > 0)
            {
                response.MaritalStateName = response.MaritalStatus.FirstOrDefault(w => w.Value == response.MaritalStateId)?.Label;
            }
            else
            {
                response.MaritalStateName = "-";
            }
            if (response.MilitaryStateId.GetValueOrDefault() > 0)
            {
                response.MilitaryStateStateName = response.MilitaryStatus.FirstOrDefault(w => w.Value == response.MilitaryStateId)?.Label;
            }
            else
            {
                response.MilitaryStateStateName = "-";
            }

            if (response.CityId.GetValueOrDefault() > 0)
            {
                var city = response.Cites.SelectMany(w => w.Options).FirstOrDefault(w => w.Value == response.CityId);
                if (city != null)
                {
                    response.CityName = city.Label;
                }
            }
            else
            {
                response.CityName = "-";
            }
            if (response.SalaryRequestedId.GetValueOrDefault() > 0)
            {
                response.SalaryRequestedName = response.SalaryRequesteds.FirstOrDefault(w => w.Value == response.SalaryRequestedId)?.Label;
            }
            else
            {
                response.SalaryRequestedName = "-";
            }

            //if (string.IsNullOrEmpty(response.BirthDate) == false)
            //{
            //    response.Age = response.BirthDate.ToGregorianDateTime().Value.GetAge();
            //}

            response.UserEducations = await _userEducationService.Get(user.Id);
            response.UserJobs = await _userJobService.Get(user.Id);
            response.UserLanguages = await _userLanguageService.Get(user.Id);
            response.UserSkills = await _userSoftWareSkillsService.Get(user.Id);
            response.UserPriorities = await _userPriorityService.Get(user.Id);
            if (response.UserPriorities != null)
            {
                response.UserPriorities.GroupsName = response.JobGroups.Where(w => response.UserPriorities.Groups.Any(a => a == w.Value)).Select(w => w.Label).ToList();
            }

            return response;
        }
    }
}
