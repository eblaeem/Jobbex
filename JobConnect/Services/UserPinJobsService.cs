using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.UserPinJob;

namespace Services
{
    public interface IUserPinJobsService
    {
        Task<IEnumerable<UserPinJobsResponse>> Get(PagingModel filter, CancellationToken cancellationToken);
        Task<ResponseBase> Save(int id, CancellationToken cancellationToken);
        Task<ResponseBase> Delete(int id, CancellationToken cancellationToken);
    }
    public class UserPinJobsService : IUserPinJobsService
    {
        private readonly ICommonService _commonService;
        private readonly IUnitOfWork _unitOfWork;

        private int _userId { get; set; }

        public UserPinJobsService(ICommonService commonService, IUnitOfWork unitOfWork)
        {
            _commonService = commonService;
            _unitOfWork = unitOfWork;
            _userId = _commonService.GetCurrentUserId();
        }

        public async Task<IEnumerable<UserPinJobsResponse>> Get(PagingModel filter,CancellationToken cancellationToken)
        {
           

            IQueryable<UserPinJobsResponse> query = from userPinJobs in _unitOfWork.Set<UserPinJob>().Where(w => w.UserId == _userId)
                                                    from job in _unitOfWork.Set<Job>().Where(w => w.Id == userPinJobs.JobId && w.Status == true)
                                                    from company in _unitOfWork.Set<Company>().Where(w => w.Id == job.CompanyId)
                                                    from contract in _unitOfWork.Set<Entities.ContractType>().Where(w => w.Id == job.ContractTypeId)
                                                    from salaryType in _unitOfWork.Set<SalaryRequestedType>().Where(w => w.Id == job.SalaryTypeId)
                                                    from city in _unitOfWork.Set<City>().Where(w => w.Id == job.CityId)
                                                    from state in _unitOfWork.Set<StateModel>().Where(w => w.Id == job.StateId)
                                                    from jobGroup in _unitOfWork.Set<Group>().Where(w=>w.Id == job.JobGroupId)
                                                    select new UserPinJobsResponse
                                                    {
                                                        JobId = userPinJobs.JobId,
                                                        UserId = userPinJobs.UserId,
                                                        JobTitle = job.Title,
                                                        CompanyId = company.Id,
                                                        Status = job.Status,
                                                        CityId = city.Id,
                                                        CityName = city.PersianName,
                                                        CompanyName = company.Title,
                                                        ContractId = job.ContractTypeId.Value,
                                                        ContractName = contract.Name,
                                                        StateId = state.Id,
                                                        StatePersianName = state.PersianName,
                                                        SalaryTypeId = job.SalaryTypeId.Value,
                                                        SalaryTypeName = salaryType.Name,
                                                        Description = job.Description,
                                                        ExpireDateTime = job.ExpiredDateTime,
                                                        DateTime = job.DateTime,
                                                        AttachmentLogoId = company.AttachmentLogoId,
                                                        StatusName = "فعال",
                                                        JobGroupId = jobGroup.Id,
                                                        JobGroupName = jobGroup.Name
                                                    };

            var totalRowCount = await query.CountAsync(cancellationToken);
            if (totalRowCount <= 0)
            {
                return null;
            }

            var response = await query.Skip(filter.PageNumber * filter.PageSize)
                .Take(filter.PageSize).ToListAsync(cancellationToken);
            if (response.Any() == false)
            {
                return null;
            }
            response.FirstOrDefault().TotalRowCount = totalRowCount;

            foreach (var item in response)
            {
                if (item.AttachmentLogoId > 0)
                {
                    var attachment = _unitOfWork.Set<Attachment>().FirstOrDefault(w => w.Id == item.AttachmentLogoId);
                    if (attachment != null)
                    {
                        item.CompanyLogo = "data:image/gif;base64," + Convert.ToBase64String(attachment.FileData);
                    }
                }
                item.DateTimeString = item.DateTime.ToShortPersianDateString();
            }


            return response;
        }

        public async Task<ResponseBase> Delete(int id, CancellationToken cancellationToken)
        {
            var userPinJobsResponse = await _unitOfWork.Set<UserPinJob>().FindAsync(_userId,id);

            if (userPinJobsResponse == null) { return new ResponseBase(false); };

            _unitOfWork.Set<UserPinJob>().Remove(userPinJobsResponse);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseBase(true);

        }

        public async Task<ResponseBase> Save(int id, CancellationToken cancellationToken)
        {
            
            var userPinJobsResponse = await _unitOfWork.Set<UserPinJob>().FindAsync(_userId, id);
            
            if (userPinJobsResponse != null) { return new ResponseBase(false,"این آگهی قبلا برای شما نشان شده است."); };


            await _unitOfWork.Set<UserPinJob>().AddAsync(new UserPinJob
            {
                Date = DateTime.Now,
                JobId = id,
                UserId = _userId
            });

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseBase(true);
        }
    }
}
