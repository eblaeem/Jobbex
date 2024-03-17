using DataLayer.Context;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.Attachment;
using ViewModel.JobRequest;

namespace Services
{
    public interface IJobRequestService
    {
        Task<ResponseBase> Save(JobRequestSave request);
        Task<List<JobRequestResponse>> Get(JobRequestFilter request);
    }
    public class JobRequestService : IJobRequestService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICommonService _commonService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUserProfileService _userProfileService;
        public JobRequestService(IUnitOfWork uow,
            ICommonService commonService,
            IAttachmentService attachmentService,
            IUserProfileService userProfileService)
        {
            _uow = uow;
            _commonService = commonService;
            _attachmentService = attachmentService;
            _userProfileService = userProfileService;
        }
        public async Task<ResponseBase> Save(JobRequestSave request)
        {
            var currentUserId = _commonService.GetCurrentUserId();
            if (currentUserId == 0 && string.IsNullOrEmpty(request.PhoneNumber))
            {
                return new ResponseBase(false, "لطفا وارد سایت شوید یا شماره تماس خود را وارد نمایید");
            }

            var job = _uow.Set<Job>().Find(request.JobId);
            if (job == null)
            {
                return new ResponseBase(false, "موقعیت شغلی مورد نظر وجود ندارد");
            }

            if (job.Status == false || job.ExpiredDateTime < DateTime.Now)
            {
                return new ResponseBase(false, "موقعیت شغلی مورد نظر فعال نمی باشد");
            }

            if (job.RequiredLoginToSite == true && currentUserId == 0)
            {
                return new ResponseBase(false, "لطفا وارد سایت شوید و اطلاعات مورد نیاز شغل را از پروفایل کاربری وارد نمایید");
            }

            if (currentUserId > 0)
            {
                var userProfile = await _userProfileService.GetByUserId(currentUserId);
                if (job.RequiredInformation == true)
                {
                    if (string.IsNullOrEmpty(userProfile.FirstName))
                    {
                        return new ResponseBase(false, "لطفا نام را در قسمت پروفایل کاربری بخش مشخصات تکمیلی وارد نمایید");
                    }
                    if (string.IsNullOrEmpty(userProfile.LastName))
                    {
                        return new ResponseBase(false, "لطفا نام خانوادگی را در قسمت پروفایل کاربری بخش مشخصات تکمیلی وارد نمایید");
                    }
                    if (string.IsNullOrEmpty(userProfile.BirthDate))
                    {
                        return new ResponseBase(false, "لطفا تاریخ تولد را در قسمت پروفایل کاربری بخش مشخصات تکمیلی وارد نمایید");
                    }
                    if (string.IsNullOrEmpty(userProfile.PhoneNumber))
                    {
                        return new ResponseBase(false, "لطفا شماره تماس را در قسمت پروفایل کاربری بخش مشخصات تکمیلی وارد نمایید");
                    }
                }

                if (job.RequiredEducation == true && userProfile.UserEducations.Any() == false)
                {
                    return new ResponseBase(false, "لطفا حداقل یک مورد سوابق تحصیلی/آموزشی وارد نمایید ");
                }

                if (job.RequiredJob == true && userProfile.UserJobs.Any() == false)
                {
                    return new ResponseBase(false, "لطفا حداقل یک مورد سوابق شغلی وارد نمایید ");
                }

                if (job.RequiredLanguage == true && userProfile.UserLanguages.Any() == false)
                {
                    return new ResponseBase(false, "لطفا حداقل یک مورد اطلاعات زبان ها وارد نمایید ");
                }

                if (job.RequiredSkills == true && userProfile.UserSkills.Any() == false)
                {
                    return new ResponseBase(false, "لطفا حداقل یک مورد اطلاعات مهارت های شغلی وارد نمایید ");
                }
                //if (job.RequiredPriorities == true && userProfile.UserPriorities)
                //{
                //    return new ResponseBase(false, "لطفا حداقل یک مورد اطلاعات الویت های شغلی وارد نمایید ");
                //}
            }

            var find = _uow.Set<JobRequested>().FirstOrDefault(w => w.JobId == request.JobId &&
                (currentUserId > 0 ? w.UserId == currentUserId : w.PhoneNumber == request.PhoneNumber));
            if (find != null)
            {
                return new ResponseBase(false, "درخواست شما قبلا ثبت شده است و در حال پیگیری می باشد");
            }
            int? attachmentResumehId = null;
            if (request.File != null)
            {
                var response = await _attachmentService.Save(new AttachmentSave()
                {
                    AttachmentTypeId = 6,//ارسال رزومه
                    FormFile = request.File,
                    ImageOrPdf = true,
                    FileName = request.File.FileName,
                    FileType = request.File.ContentType,
                    FileSize = Convert.ToInt32(request.File.Length),
                });
                if (response.IsValid)
                {
                    attachmentResumehId = response.Id;
                }
            }

            _uow.Set<JobRequested>().Add(new JobRequested
            {
                DateTime = DateTime.Now,
                JobId = request.JobId,
                UserId = currentUserId > 0 ? currentUserId : null,
                AttachmentResumehId = attachmentResumehId,
                PhoneNumber = request.PhoneNumber,
                DisplayName = request.DisplayName,
            });
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
        public async Task<List<JobRequestResponse>> Get(JobRequestFilter request)
        {
            var currentCompany = _commonService.GetCurrentCompanyId();
            var job = _uow.Set<Job>().Find(request.JobId);
            if (job == null)
            {
                return new List<JobRequestResponse>();
            }
            if (job.CompanyId != currentCompany)
            {
                return new List<JobRequestResponse>();
            }
            var query = (from jobRequest in _uow.Set<JobRequested>().Where(w => w.JobId == job.Id)
                         from user in _uow.Set<User>().Where(w => w.Id == jobRequest.UserId).DefaultIfEmpty()
                         from city in _uow.Set<City>().Where(w => w.Id == user.CityId).DefaultIfEmpty()
                         from jobRequestState in _uow.Set<JobRequestState>().Where(w => w.JobRequestId == jobRequest.Id).DefaultIfEmpty()
                         from userRequestedStatus in _uow.Set<UserRequestedStatus>().Where(w => w.Id == jobRequestState.TypeId).DefaultIfEmpty()
                         from attachment in _uow.Set<Attachment>().Where(w => w.RecordId == user.Id && w.AttachmentTypeId == 1).DefaultIfEmpty()
                         select new JobRequestResponse
                         {
                             Id = jobRequest.Id,
                             UserId = user.Id,
                             DisplayName = user != null ? user.DisplayName : jobRequest.DisplayName,
                             PhoneNumber = user != null ? user.PhoneNumber : jobRequest.PhoneNumber,
                             UserCityName = city.PersianName,
                             StatusName = userRequestedStatus != null ? userRequestedStatus.Name : "بررسی نشده",
                             StateId = userRequestedStatus != null ? userRequestedStatus.Id : 1,
                             DateTimeString = jobRequest.DateTime.RelativeDate(),
                             UserAvatar = attachment != null ? "data:image/gif;base64," + Convert.ToBase64String(attachment.FileData) : "",
                             AttachmentResumehId = jobRequest.AttachmentResumehId
                         });
            if (string.IsNullOrEmpty(request.Sort))
            {
                request.Sort = "id desc";
            }
            query = query.OrderBy(request.Sort);

            var totalRowCount = await query.CountAsync();
            if (totalRowCount <= 0)
            {
                return null;
            }

            var response = await query.Skip(request.PageNumber * request.PageSize)
                .Take(request.PageSize).ToListAsync();
            if (response.Any() == false)
            {
                return null;
            }
            var firstOrDefault = response.FirstOrDefault();
            if (firstOrDefault != null)
            {
                firstOrDefault.TotalRowCount = totalRowCount;
                firstOrDefault.PageNumber = request.PageNumber;
                firstOrDefault.PageSize = request.PageSize;
            }

            return response;
        }

    }
}
