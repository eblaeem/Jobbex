using Azure;
using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.Extentions;
using ViewModel;
using ViewModel.Attachment;
using ViewModel.Company;
using ViewModel.Job;
using ViewModel.User;
using static iTextSharp.text.pdf.AcroFields;

namespace Services
{
    public interface ICompanyService
    {
        Task<List<CompaniesResponse>> Get(CompanyFilter filter);
        Task<CompanySave> GetByUserId(int userId);
        Task<List<CompanyJobResponse>> GetJobs(CompanyJobFilter filter);
        Task<List<CompanyResponse>> GetSummery(CompanyFilter filter);
        Task<List<GroupsCompanyResponse>> GetGroupsCompany();
        Task<CompanyDetailResponse> Detail(int id);
        Task<ResponseBase> Save(CompanySave request);
        Task<ResponseBase> Upload(int type, IFormFile file);

    }
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Company> _companies;
        private readonly IAttachmentService _attachmentService;
        private readonly ICommonService _commonService;

        public CompanyService(IUnitOfWork uow,
            IAttachmentService attachmentService,
            ICommonService commonService)
        {
            _uow = uow;
            _companies = _uow.Set<Company>();
            _attachmentService = attachmentService;
            _commonService = commonService;
        }
        public async Task<List<CompaniesResponse>> Get(CompanyFilter filter)
        {
            var query = (from company in _uow.Set<Company>()
                         from user in _uow.Set<User>().Where(w => w.Id == company.UserId)
                         from jobGroup in _uow.Set<Group>().Where(w => w.Id == company.JobGroupId)
                         from organizationSize in _uow.Set<OrganizationSize>().Where(w => w.Id == company.OrganizationSizeId)
                         let activeJobCount = _uow.Set<Job>().Count(w => w.CompanyId == company.Id && w.Status == true && w.ExpiredDateTime > DateTime.Now)
                         select new CompaniesResponse
                         {
                             Id = company.Id,
                             Name = company.Title,
                             Description = company.Description,
                             WebSite = company.WebSite,
                             OrganizationSizeName = organizationSize.Name,
                             JobGroupName = jobGroup.Name,
                             ActiveJobCount = activeJobCount,
                             UserId = user.Id
                         });
            if (filter.Id > 0)
            {
                query = query.Where(w => w.Id == filter.Id);
            }
            if (filter.UserId > 0)
            {
                query = query.Where(w => w.UserId == filter.UserId);
            }
            var totalRowCount = await query.CountAsync();
            if (totalRowCount <= 0)
            {
                return null;
            }
            var response = await query.Skip(filter.PageNumber * filter.PageSize)
            .Take(filter.PageSize).ToListAsync();
            if (response.Any() == false)
            {
                return null;
            }
            response.FirstOrDefault().TotalRowCount = totalRowCount;
            foreach (var item in response)
            {
                if (item.AttachmentLogoId > 0)
                {
                    var attachment = _uow.Set<Attachment>().FirstOrDefault(w => w.Id == item.AttachmentLogoId);
                    if (attachment != null)
                    {
                        item.CompanyLogo = "data:image/jpeg;base64," + Convert.ToBase64String(attachment.FileData);
                    }
                }
            }
            return response;
        }
        public async Task<CompanySave> GetByUserId(int userId)
        {
            var result = new CompanySave();
            var company = await _uow.Set<Company>().FirstOrDefaultAsync(w => w.UserId == userId);

            if (company == null)
            {
                return result;
            }

            result.Id = company.Id;
            result.Title = company.Title;
            result.Description = company.Description;
            result.AttachmentLogoId = company.AttachmentLogoId;
            result.AttachmentBackgroundId = company.AttachmentBackgroundId;
            result.JobGroupId = company.JobGroupId;
            result.StateId = company.StateId;
            result.CityId = company.CityId;
            result.PhoneNumber = company.Phone;
            result.CreatedYear = company.CreatedYear;
            result.EnglishName = company.EnglishName;
            result.OrganizationSizeId = company.OrganizationSizeId;
            result.ServiceAndProducs = company.ServiceAndProducs;
            result.ZoneId = company.ZoneId;
            result.WebSite = company.WebSite;

            if (company.AttachmentLogoId > 0)
            {
                var attachment = await _uow.Set<Attachment>().FirstOrDefaultAsync(w => w.Id == company.AttachmentLogoId);
                if (attachment != null)
                {
                    result.AttachmentLogoString = "data:image/jpeg;base64," + Convert.ToBase64String(attachment.FileData);
                }
            }

            if (company.AttachmentBackgroundId > 0)
            {
                var attachment = await _uow.Set<Attachment>().FirstOrDefaultAsync(w => w.Id == company.AttachmentBackgroundId);
                if (attachment != null)
                {
                    result.AttachmentBackgroundString = "data:image/jpeg;base64," + Convert.ToBase64String(attachment.FileData);
                }
            }
            var attachments = await _uow.Set<Attachment>().Where(w => w.AttachmentTypeId == 5 && w.RecordId == company.Id).ToListAsync();
            if (attachments.Any())
            {
                result.Attachments = attachments.Select(w => new LabelValue
                {
                    Value = w.Id,
                    Label = "data:image/jpeg;base64," + Convert.ToBase64String(w.FileData)
                }).ToList();
            }

            result.OrganizationSizes = await _uow.Set<OrganizationSize>().Select(item => new LabelValue
            {
                Label = item.Name,
                Value = item.Id
            }).ToListAsync();

            return result;
        }
        public async Task<List<CompanyJobResponse>> GetJobs(CompanyJobFilter filter)
        {
            var companyId = _commonService.GetCurrentCompanyId();
            var company = await _uow.Set<Company>().FirstOrDefaultAsync(w => w.Id == companyId);
            if (company is null)
            {
                return new List<CompanyJobResponse>();
            }
            var query = (from job in _uow.Set<Job>().Where(w => w.CompanyId == company.Id)
                         from city in _uow.Set<Entities.City>().Where(w => w.Id == job.CityId)
                         from jobGroup in _uow.Set<Group>().Where(w => w.Id == job.JobGroupId)
                         from workExperienceYear in _uow.Set<WorkExperienceYear>().Where(w => w.Id == job.WorkExperienceYearId)
                         from educationLevel in _uow.Set<EducationLevel>().Where(w => w.Id == job.EducationLevelRequired)
                         from militaryState in _uow.Set<MilitaryStatus>().Where(w => w.Id == job.MilitaryStateRequired)
                         from salaryRequested in _uow.Set<SalaryRequestedType>().Where(w => w.Id == job.SalaryTypeId)
                         from contractType in _uow.Set<ContractType>().Where(w => w.Id == job.ContractTypeId)
                         let jobRequestedCount = _uow.Set<JobRequested>().Count(w => w.JobId == job.Id)
                         select new CompanyJobResponse
                         {
                             JobId = job.Id,
                             CompanyName = company.Title,
                             CityId = company.CityId,
                             CityName = city.PersianName,
                             ContractTypeId = contractType.Id,
                             ContractTypeName = contractType.Name,
                             DateTime = job.DateTime,
                             JobTitle = job.Title,
                             SalaryRequestedId = salaryRequested.Id,
                             SalaryRequestedName = salaryRequested.Name,
                             JobGroupId = jobGroup.Id,
                             JobGroupName = jobGroup.Name,
                             WorkExperienceYearId = workExperienceYear.Id,
                             WorkExperienceYearName = workExperienceYear.Name,
                             JobDescription = job.Description,
                             JobRequestedCount = jobRequestedCount,
                             Status = job.Status,
                             DateTimeString = job.DateTime.RelativeDate()
                         });
            if (filter.JobId > 0)
            {
                query = query.Where(w => w.JobId == filter.JobId);
            }
            if (filter.CityId > 0)
            {
                query = query.Where(w => w.CityId == filter.CityId);
            }
            if (filter.JobGroupId > 0)
            {
                query = query.Where(w => w.JobGroupId == filter.JobGroupId);
            }
            if (filter.SalaryRequestedId > 0)
            {
                query = query.Where(w => w.SalaryRequestedId == filter.SalaryRequestedId);
            }
            if (filter.WorkExperienceYearId > 0)
            {
                query = query.Where(w => w.WorkExperienceYearId == filter.WorkExperienceYearId);
            }

            if (string.IsNullOrEmpty(filter.Sort))
            {
                filter.Sort = "JobId desc";
            }
            query = query.OrderBy(filter.Sort);

            var totalRowCount = await query.CountAsync();
            if (totalRowCount <= 0)
            {
                return new List<CompanyJobResponse>();
            }

            var response = await query.Skip(filter.PageNumber * filter.PageSize)
                .Take(filter.PageSize).ToListAsync();
            if (response.Any() == false)
            {
                return new List<CompanyJobResponse>();
            }
            var firstOrDefault = response.FirstOrDefault();
            if (firstOrDefault != null)
            {
                firstOrDefault.TotalRowCount = totalRowCount;
                firstOrDefault.PageNumber = filter.PageNumber;
                firstOrDefault.PageSize = filter.PageSize;
            }

            return response;
        }

        public async Task<List<CompanyResponse>> GetSummery(CompanyFilter filter)
        {
            var query = (from company in _uow.Set<Company>()
                         from job in _uow.Set<Job>().Where(w => w.CompanyId == company.Id)
                         group company by company.Id into g
                         orderby g.Count() descending
                         select new CompanyResponse
                         {
                             JobCount = g.Count(),
                             Name = g.Min(w => w.Title),
                             Id = g.Key,
                             Description = g.Min(w => w.Description),
                             AttachmentLogoId = g.Min(w => w.AttachmentLogoId)
                         });
            var response = await query.Skip(filter.PageNumber * filter.PageSize)
                .Take(filter.PageSize).ToListAsync();
            if (response.Any() == false)
            {
                return null;
            }
            foreach (var item in response)
            {
                if (item.AttachmentLogoId > 0)
                {
                    var attachment = _uow.Set<Attachment>().FirstOrDefault(w => w.Id == item.AttachmentLogoId);
                    if (attachment != null)
                    {
                        item.CompanyLogo = "data:image/gif;base64," + Convert.ToBase64String(attachment.FileData);
                    }
                }
            }
            return response;
        }
        public async Task<CompanyDetailResponse> Detail(int id)
        {
            var result = new CompanyDetailResponse();
            var company = _uow.Set<Company>().FirstOrDefault(w => w.Id == id);

            if (company == null)
            {
                return result;
            }

            result.Id = company.Id;
            result.Name = company.Title;
            result.Description = company.Description;
            company.AttachmentLogoId = company.AttachmentLogoId ?? 1;
            company.AttachmentBackgroundId = company.AttachmentBackgroundId ?? 3;


            var attachment = _uow.Set<Attachment>().FirstOrDefault(w => w.Id == company.AttachmentLogoId);
            if (attachment != null)
            {
                result.CompanyLogo = "data:image/gif;base64," + Convert.ToBase64String(attachment.FileData);
            }

            //attachment = _uow.Set<Attachment>().FirstOrDefault(w => w.Id == company.AttachmentBackgroundId);
            //if (attachment != null)
            //{
            //    result.CompanyBackground = Convert.ToBase64String(attachment.FileData);
            //}

            var jobs = await (from job in _uow.Set<Job>().Where(w => w.CompanyId == company.Id)
                              from city in _uow.Set<City>().Where(w => w.Id == job.CityId)
                              from jobGroup in _uow.Set<Group>().Where(w => w.Id == job.JobGroupId)
                              from workExperienceYear in _uow.Set<WorkExperienceYear>().Where(w => w.Id == job.WorkExperienceYearId)
                              from educationLevel in _uow.Set<EducationLevel>().Where(w => w.Id == job.EducationLevelRequired)
                              from militaryState in _uow.Set<MilitaryStatus>().Where(w => w.Id == job.MilitaryStateRequired)
                              from salaryRequested in _uow.Set<SalaryRequestedType>().Where(w => w.Id == job.SalaryTypeId)
                              from contractType in _uow.Set<ContractType>().Where(w => w.Id == job.ContractTypeId)
                              select new JobResponse
                              {
                                  Id = job.Id,
                                  Status = job.Status,
                                  ExpireDateTime = job.ExpiredDateTime,
                                  CompanyName = company.Title,
                                  AttachmentLogoId = company.AttachmentLogoId,
                                  CityId = company.CityId,
                                  CityName = city.PersianName,
                                  CompanyRate = company.Rate,
                                  ContractTypeName = contractType.Name,
                                  DateTime = job.DateTime,
                                  JobTitle = job.Title,
                                  SalaryRequestedName = salaryRequested.Name,
                                  JobGroupId = jobGroup.Id,
                                  JobGroupName = jobGroup.Name,
                              }).ToListAsync();
            if (jobs.Any())
            {
                var currentData = DateTime.Now;
                foreach (var item in jobs)
                {
                    if (item.Status == true && item.ExpireDateTime > currentData)
                    {
                        item.StatusName = "فعال";
                        result.ActiveJobs.Add(item);
                    }
                    if (item.Status == false && item.ExpireDateTime < currentData)
                    {
                        item.StatusName = "منقضی شده";
                        result.ExpiredJobs.Add(item);
                    }
                    item.DateTimeString = item.DateTime.ToShortPersianDateString();
                }
            }
            return result;
        }

        public async Task<List<GroupsCompanyResponse>> GetGroupsCompany()
        {
            var query = (from company in _uow.Set<Company>()
                         from job in _uow.Set<Job>().Where(w => w.CompanyId == company.Id)
                         from jobGroup in _uow.Set<Group>().Where(w => w.Id == job.JobGroupId)
                         group jobGroup by jobGroup.Id into g
                         orderby g.Count() descending
                         select new GroupsCompanyResponse
                         {
                             GroupCount = g.Count(),
                             GroupName = g.Min(w => w.Name),
                             Id = g.Key
                         });
            return await query.ToListAsync();
        }
        public async Task<ResponseBase> Save(CompanySave request)
        {
            var find = _companies.Find(request.Id);
            if (find == null)
            {
                return new ResponseBase(false, "شرکت مورد نظر وجود ندارد");
            }
            if (request.AttachmentLogo != null)
            {
                var response = await _attachmentService.Save(new AttachmentSave()
                {
                    AttachmentTypeId = 4,
                    FormFile = request.AttachmentLogo,
                    ImageOrPdf = true,
                    FileName = request.AttachmentLogo.FileName,
                    FileType = request.AttachmentLogo.ContentType,
                    FileSize = Convert.ToInt32(request.AttachmentLogo.Length),
                });
                if (response.IsValid)
                {
                    find.AttachmentLogoId = response.Id;
                }
            }
            if (request.AttachmentBackgroundId != null)
            {
                var response = await _attachmentService.Save(new AttachmentSave()
                {
                    AttachmentTypeId = 5,
                    FormFile = request.AttachmentBackground,
                    ImageOrPdf = true,
                    FileName = request.AttachmentBackground.FileName,
                    FileType = request.AttachmentBackground.ContentType,
                    FileSize = Convert.ToInt32(request.AttachmentBackground.Length),
                });
                if (response.IsValid)
                {
                    find.AttachmentBackgroundId = response.Id;
                }
            }
            if (request.Title != find.Title)
            {
                var user = _uow.Set<User>().FirstOrDefault(w => w.Id == find.UserId);
                if (user != null)
                {
                    user.DisplayName = find.Title;
                }
            }
            find.Title = request.Title;
            find.StateId = request.StateId;
            find.CityId = request.CityId;
            find.CreatedYear = request.CreatedYear;
            find.Description = request.Description;
            find.EnglishName = request.EnglishName;
            find.JobGroupId = request.JobGroupId;
            find.ZoneId = request.ZoneId;
            find.OrganizationSizeId = request.OrganizationSizeId;
            find.Phone = request.PhoneNumber;
            find.ServiceAndProducs = request.ServiceAndProducs;
            find.WebSite = request.WebSite;



            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }

        public async Task<ResponseBase> Upload(int type, IFormFile file)
        {
            if (file.Length > 5 * 1024 * 1024)
            {
                return new ResponseBase(false, "حداکثر سایز آپلودی 5 مگابایت می باشد");
            }
            var currentUserId = _commonService.GetCurrentUserId();

            var company = await _uow.Set<Company>().FirstOrDefaultAsync(w => w.UserId == currentUserId);
            if (company is null)
            {
                return new ResponseBase(false);
            }

            var attachment = _uow.Set<Attachment>().FirstOrDefault(w => w.AttachmentTypeId == type && type != 5 && w.RecordId == company.Id);

            var response = await _attachmentService.Save(new AttachmentSave()
            {
                AttachmentTypeId = type,
                FormFile = file,
                ImageOrPdf = true,
                FileName = file.FileName,
                FileType = file.ContentType,
                FileSize = Convert.ToInt32(file.Length),
                RecordId = company.Id,
                Id = attachment?.Id
            });
            if (response.IsValid)
            {
                switch (type)
                {
                    case 3:
                        company.AttachmentLogoId = response.Id;
                        break;
                    case 4:
                        company.AttachmentBackgroundId = response.Id;
                        break;
                }
                await _uow.SaveChangesAsync();
            }

            return response;
        }
    }
}
