using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.EntityFrameworkCore;
using Services.Extentions;
using ViewModel;
using ViewModel.Job;
using ViewModel.UserPriority;
using ViewModel.UserRecommended;

namespace Services
{
    public interface IJobService
    {
        Task<List<JobResponse>> Get(JobFilter filter);
        Task<List<UserRecommendedResponse>> GetUserJobRecomended(UserPriorityResponse filter);
        Task<int> Count();
        Task<JobDetailResponse> Detail(int id);
        Task<ResponseBase> Save(JobSave save);
    }
    public class JobService : IJobService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Job> _jobs;

        public JobService(IUnitOfWork uow)
        {
            _uow = uow;
            _jobs = _uow.Set<Job>();
        }


        public async Task<List<JobResponse>> Get(JobFilter filter)
        {
            var currentData = DateTime.Now;

            var query = (from job in _jobs.Where(w => w.Status == true && w.ExpiredDateTime > currentData)
                         from city in _uow.Set<Entities.City>().Where(w => w.Id == job.CityId)
                         from company in _uow.Set<Company>().Where(w => w.Id == job.CompanyId)
                         from jobGroup in _uow.Set<Group>().Where(w => w.Id == job.JobGroupId)
                         from workExperienceYear in _uow.Set<WorkExperienceYear>().Where(w => w.Id == job.WorkExperienceYearId)
                         from educationLevel in _uow.Set<EducationLevel>().Where(w => w.Id == job.EducationLevelRequired)
                         from militaryState in _uow.Set<MilitaryStatus>().Where(w => w.Id == job.MilitaryStateRequired)
                         from salaryRequested in _uow.Set<SalaryRequestedType>().Where(w => w.Id == job.SalaryTypeId)
                         from contractType in _uow.Set<ContractType>().Where(w => w.Id == job.ContractTypeId)
                         select new JobResponse
                         {
                             Id = job.Id,
                             CompanyName = company.Title,
                             AttachmentLogoId = company.AttachmentLogoId,
                             CityId = job.CityId,
                             CityName = city.PersianName,
                             CompanyRate = company.Rate,
                             ContractTypeId = contractType.Id,
                             ContractTypeName = contractType.Name,
                             DateTime = job.DateTime,
                             JobTitle = job.Title,
                             SalaryRequestedId = salaryRequested.Id,
                             SalaryRequestedName = salaryRequested.Name,
                             JobGroupId = jobGroup.Id,
                             JobGroupName = jobGroup.Name,
                             WorkExperienceYearId = workExperienceYear.Id,
                             //WorkExperienceYearName = workExperienceYear.Name
                         });
            if (filter.Cities != null && filter.Cities.Any())
            {
                query = query.Where(w => filter.Cities.Any(a => a == w.CityId));
            }
            if (filter.JobGroups != null && filter.JobGroups.Any())
            {
                query = query.Where(w => filter.JobGroups.Any(a => a == w.JobGroupId));
            }
            if (filter.ContractTypes != null && filter.ContractTypes.Any())
            {
                query = query.Where(w => filter.ContractTypes.Any(a => a == w.ContractTypeId));
            }
            if (filter.SalaryRequests != null && filter.SalaryRequests.Any())
            {
                query = query.Where(w => filter.SalaryRequests.Any(a => a == w.SalaryRequestedId));
            }
            if (filter.WorkExperienceYears != null && filter.WorkExperienceYears.Any())
            {
                query = query.Where(w => filter.WorkExperienceYears.Any(a => a == w.WorkExperienceYearId));
            }

            if (string.IsNullOrEmpty(filter.Term) == false)
            {
                filter.Term = filter.Term.Trim().ApplyCorrectYeKe();
                query = query.Where(w => w.JobTitle.Contains(filter.Term) || w.CompanyName.Contains(filter.Term));
            }

            if (string.IsNullOrEmpty(filter.Sort))
            {
                filter.Sort = "id desc";
            }
            query = query.OrderBy(filter.Sort);

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
                        item.CompanyLogo = "data:image/gif;base64," + Convert.ToBase64String(attachment.FileData);
                    }
                }
                item.DateTimeString = item.DateTime.ToShortPersianDateString();
            }
            return response;
        }

        public async Task<int> Count()
        {
            return await _jobs.CountAsync();
        }

        public async Task<List<UserRecommendedResponse>> GetUserJobRecomended(UserPriorityResponse filter)
        {
            var currentData = DateTime.Now;

            var finalQuery = new List<Tuple<IQueryable<UserRecommendedResponse>, int>>();
            var response = new List<UserRecommendedResponse>();
            var totalCount = 0;
            var query = (from job in _jobs.Where(w => w.Status == true && w.ExpiredDateTime > DateTime.Now)
                         from city in _uow.Set<Entities.City>().Where(w => w.Id == job.CityId)
                         from company in _uow.Set<Company>().Where(w => w.Id == job.CompanyId)
                         from organizationSize in _uow.Set<OrganizationSize>().Where(w => w.Id == company.OrganizationSizeId)
                         from jobGroup in _uow.Set<Group>().Where(w => w.Id == job.JobGroupId)
                         from workExperienceYear in _uow.Set<WorkExperienceYear>().Where(w => w.Id == job.WorkExperienceYearId)
                         from educationLevel in _uow.Set<EducationLevel>().Where(w => w.Id == job.EducationLevelRequired)
                         from militaryState in _uow.Set<MilitaryStatus>().Where(w => w.Id == job.MilitaryStateRequired)
                         from salaryType in _uow.Set<SalaryRequestedType>().Where(w => w.Id == job.SalaryTypeId)
                         from contractType in _uow.Set<ContractType>().Where(w => w.Id == job.ContractTypeId)
                         from gender in _uow.Set<Gender>().Where(w => w.Id == job.GenderRequired)
                         let jobBenefit = _uow.Set<JobBenefit>().Where(w => w.JobId == job.Id).DefaultIfEmpty().AsEnumerable().ToList()
                         //let jobSkills =(from jobSkill in _uow.Set<JobSkill>().Where(w => w.JobId == job.Id)
                         //                from softwareSkill in _uow.Set<SoftwareSkill>().Where(w=>w.Id == jobSkill.SoftwareSkillId)
                         //                select new LabelValue
                         //                {
                         //                    Label = softwareSkill.Name,
                         //                    Value = softwareSkill.Id
                         //                }).DefaultIfEmpty().ToList()
                         select new UserRecommendedResponse
                         {
                             Id = job.Id,
                             CompanyName = company.Title,
                             CompanyCreatedYear = company.CreatedYear,
                             CompanyDescription = company.Description,
                             AttachmentLogoId = company.AttachmentLogoId,
                             CompanyOrganizationSize = organizationSize.Name,
                             ContractTypeName = contractType.Name,
                             DateTime = job.DateTime,
                             EducationLevelRequiredTitle = educationLevel.Name,
                             ExpiredDateTime = job.ExpiredDateTime,
                             JobStatus = true,
                             JobTitle = job.Title,
                             MilitaryStateRequiredTitle = militaryState.Name,
                             SalaryTypeName = salaryType.Name,
                             WorkExperienceYearTitle = workExperienceYear.Name,
                             WorkingDays = job.WorkingDays,
                             ZoneName = "میرداماد",
                             CompanyRate = company.Rate,
                             CityName = city.PersianName,
                             CityId = job.CityId,
                             JobGroupId = jobGroup.Id,
                             ContractTypeId = contractType.Id,
                             SalaryTypeId = salaryType.Id,
                             JobBenefits = jobBenefit,
                             GenderRequiredTitle = gender.Name,
                             //JobSkill = jobSkills,
                         });

            var t = query;
            if (filter.Cities.Count > 0)
            {
                finalQuery.Add(Tuple.Create(query.Where(w => filter.Cities.Any(a => a == w.CityId.Value)).AddRank(1), 1));
            }

            if (filter.Groups.Count > 0)
            {
                finalQuery.Add(Tuple.Create(query.Where(w => filter.Groups.Any(a => a == w.JobGroupId.Value)).AddRank(2), 2));
            }

            if (filter.ContractTypes.Count > 0)
            {
                finalQuery.Add(Tuple.Create(query.Where(w => filter.ContractTypes.Any(a => a == w.ContractTypeId.Value)).AddRank(3), 3));
            }

            //if (filter.Benefits.Count > 0)
            //{
            //    finalQuery.Add(Tuple.Create(query.Where(w => filter.Benefits.Any(a => w.JobBenefits.Any(x => x.Id == a))).AddRank(4), 4));
            //}

            if (filter.SalaryRequestedId is not null)
            {
                finalQuery.Add(Tuple.Create(query.Where(w => w.SalaryTypeId == filter.SalaryRequestedId.Value).AddRank(5), 5));
            }

            if (finalQuery.Count == 0)
            {
                response = await query.AsNoTracking().Skip(filter.PageNumber * filter.PageSize)
                                          .Take(filter.PageSize)
                                          .ToListAsync();
                totalCount = response.Count();
            }
            else
            {
                var q = finalQuery.OrderByDescending(d => d.Item2).FirstOrDefault().Item1;
                if (q != null) { finalQuery.Remove(finalQuery.OrderByDescending(d => d.Item2).FirstOrDefault()); }
                foreach (var item in finalQuery)
                {
                    q = q.Union(item.Item1);
                }
                totalCount = await q.GroupBy(g => g.Id).CountAsync();

                response = await q.GroupBy(g => g.Id, (x, y) => new UserRecommendedResponse
                {
                    Id = x,
                    CompanyName = y.Min(m => m.CompanyName),
                    CompanyCreatedYear = y.Min(m => m.CompanyCreatedYear),
                    CompanyDescription = y.Min(m => m.CompanyDescription),
                    AttachmentLogoId = y.Min(m => m.AttachmentLogoId),
                    CompanyOrganizationSize = y.Min(m => m.CompanyOrganizationSize),
                    ContractTypeName = y.Min(m => m.ContractTypeName),
                    DateTime = y.Min(m => m.DateTime),
                    EducationLevelRequiredTitle = y.Min(m => m.EducationLevelRequiredTitle),
                    ExpiredDateTime = y.Min(m => m.ExpiredDateTime),
                    //GenderRequiredTitle = y.Min(m => m.GenderRequiredTitle),
                    JobSkill = y.FirstOrDefault().JobSkill,
                    JobStatus = true,
                    JobTitle = y.Min(m => m.JobTitle),
                    MilitaryStateRequiredTitle = y.Min(m => m.MilitaryStateRequiredTitle),
                    SalaryTypeName = y.Min(m => m.SalaryTypeName),
                    WorkExperienceYearTitle = y.Min(m => m.WorkExperienceYearTitle),
                    WorkingDays = y.Min(m => m.WorkingDays),
                    ZoneName = "میرداماد",
                    CompanyRate = y.Min(m => m.CompanyRate),
                    CityName = y.Min(m => m.CityName),
                    CityId = y.Min(m => m.CityId),
                    JobGroupId = y.Min(m => m.JobGroupId),
                    ContractTypeId = y.Min(m => m.ContractTypeId),
                    SalaryTypeId = y.Min(m => m.SalaryTypeId),
                    //JobBenefits = y.FirstOrDefault().JobBenefits,
                    RankScore = y.Sum(s => s.RankScore)
                }).OrderByDescending(o => o.RankScore).Skip(filter.PageNumber * filter.PageSize)
                                       .Take(filter.PageSize)
                                       .AsNoTracking()
                                       .ToListAsync();
            }
            if (response.Any())
            {
                response.FirstOrDefault().TotalRowCount = totalCount;
                var attachmentIds = response.Select(w => w.AttachmentLogoId).ToList();

                var attachments = _uow.Set<Attachment>().Where(w => attachmentIds.Any(a => a == w.Id)).ToList();
                foreach (var item in response)
                {
                    var find = attachments.FirstOrDefault(w => w.Id == item.AttachmentLogoId);
                    if (find != null)
                    {
                        item.AttachmentLogoString = "data:image/png;base64," + Convert.ToBase64String(find.FileData);
                    }
                    item.DateTimeString = item.DateTime.RelativeDate();
                }
            }
            return response;
        }
        
        public async Task<JobDetailResponse> Detail(int id)
        {
            if (id < 0)
            {
                return null;
            }

            var response = await (from job in _jobs.Where(w => w.Id == id)
                                  from city in _uow.Set<Entities.City>().Where(w => w.Id == job.CityId)
                                  from company in _uow.Set<Company>().Where(w => w.Id == job.CompanyId)
                                  from jobGroup in _uow.Set<Group>().Where(w => w.Id == job.JobGroupId)
                                  from workExperienceYear in _uow.Set<WorkExperienceYear>().Where(w => w.Id == job.WorkExperienceYearId)
                                  from educationLevel in _uow.Set<EducationLevel>().Where(w => w.Id == job.EducationLevelRequired)
                                  from genderLevel in _uow.Set<Gender>().Where(w => w.Id == job.GenderRequired)
                                  from militaryState in _uow.Set<MilitaryStatus>().Where(w => w.Id == job.MilitaryStateRequired)
                                  from salaryRequested in _uow.Set<SalaryRequestedType>().Where(w => w.Id == job.SalaryTypeId)
                                  from contractType in _uow.Set<ContractType>().Where(w => w.Id == job.ContractTypeId)
                                  let jobSkills = (from jobSkill in _uow.Set<JobSkill>().Where(w => w.JobId == job.Id)
                                                   from softwareSkill in _uow.Set<SoftwareSkill>().Where(w => w.Id == jobSkill.SoftwareSkillId)
                                                   select new LabelValue
                                                   {
                                                       Label = softwareSkill.Name,
                                                       Value = softwareSkill.Id
                                                   }).DefaultIfEmpty().ToList()
                                  select new JobDetailResponse
                                  {
                                      Id = job.Id,
                                      CityId = city.Id,
                                      CityName = city.PersianName,
                                      CompanyId = company.Id,
                                      CompanyCreatedYear = company.CreatedYear,
                                      CompanyDescription = company.Description,
                                      CompanyName = company.Title,
                                      CompanyOrganizationSize = company.OrganizationSizeId,
                                      WebSite = company.WebSite,
                                      AttachmentLogoId = company.AttachmentLogoId,
                                      CompanyRate = company.Rate,
                                      CompanyServiceAndProducs = company.ServiceAndProducs,

                                      DateTime = job.DateTime,
                                      JobSkill = jobSkills,
                                      JobTitle = job.Title,
                                      WorkingDays = job.WorkingDays,
                                      Description = job.Description,

                                      ContractTypeName = contractType.Name,
                                      ContractTypeId = contractType.Id,
                                      EducationLevelRequiredTitle = educationLevel.Name,
                                      EducationLevelRequiredId = (byte)educationLevel.Id,
                                      GenderRequiredTitle = genderLevel.Name,
                                      GenderRequiredId = (byte)genderLevel.Id,
                                      MilitaryStateRequiredTitle = militaryState.Name,
                                      MilitaryStateRequiredId = (byte)militaryState.Id,
                                      SalaryRequestedName = salaryRequested.Name,
                                      SalaryTypeId = salaryRequested.Id,
                                      WorkExperienceYearTitle = workExperienceYear.Name,
                                      WorkExperienceYearId = (byte)workExperienceYear.Id,
                                      JobGroupId = jobGroup.Id,
                                      JobGroupName = jobGroup.Name,
                                      Status = job.Status,
                                      RequiredLoginToSite = job.RequiredLoginToSite.GetValueOrDefault(),
                                      RequiredInformation = job.RequiredInformation.GetValueOrDefault(),
                                      RequiredEducation = job.RequiredEducation.GetValueOrDefault(),
                                      RequiredJob = job.RequiredJob.GetValueOrDefault(),
                                      RequiredLanguage = job.RequiredLanguage.GetValueOrDefault(),
                                      RequiredSkills = job.RequiredSkills.GetValueOrDefault(),
                                      RequiredPriorities = job.RequiredPriorities.GetValueOrDefault()

                                  }).FirstOrDefaultAsync();
            if (response != null)
            {
                if (response.AttachmentLogoId > 0)
                {
                    var attachment = _uow.Set<Attachment>().FirstOrDefault(w => w.Id == response.AttachmentLogoId);
                    if (attachment != null)
                    {
                        response.CompanyLogo = "data:image/png;base64," + Convert.ToBase64String(attachment.FileData);
                    }
                }
                var attachments = _uow.Set<Attachment>().Where(w => w.RecordId == response.CompanyId && w.AttachmentTypeId == 2);
                if (attachments.Any())
                {
                    foreach (var item in attachments)
                    {
                        response.CompanyAttachments.Add("data:image/png;base64," + Convert.ToBase64String(item.FileData));
                    }
                }
                response.DateTimeString = response.DateTime.ToShortPersianDateString();
            }
            return response;
        }
        
        public async Task<ResponseBase> Save(JobSave request)
        {
            var company = await _uow.Set<Company>().FirstOrDefaultAsync(w => w.Id == request.CompanyId);
            if (company is null)
            {
                return new ResponseBase(false);
            }

            if (request.RequiredInformation == true || request.RequiredEducation == true
                || request.RequiredJob == true || request.RequiredLanguage == true
                 || request.RequiredSkills == true)
            {
                request.RequiredLoginToSite = true;
            }

            if (request.Id > 0)
            {
                var find = await _jobs.FirstOrDefaultAsync(w => w.Id == request.Id && w.CompanyId == company.Id);
                if (find is null)
                {
                    return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
                }
                find.CityId = request.CityId;
                find.StateId = request.StateId;
                find.JobGroupId = request.JobGroupId;
                find.SalaryTypeId = request.SalaryTypeId;
                find.Status = request.Status;
                find.ContractTypeId = request.ContractTypeId;
                find.Description = request.Description;
                find.EducationLevelRequired = request.EducationLevelRequired;
                find.GenderRequired = request.GenderRequired;
                find.MilitaryStateRequired = request.MilitaryStateRequired;
                find.Title = request.Title;
                find.Description = request.Description;
                find.WorkingDays = request.WorkingDays;
                find.WorkExperienceYearId = request.WorkExperienceYearId;

                find.RequiredLoginToSite = request.RequiredLoginToSite;
                find.RequiredInformation = request.RequiredInformation;
                find.RequiredEducation = request.RequiredEducation;
                find.RequiredJob = request.RequiredJob;
                find.RequiredLanguage = request.RequiredLanguage;
                find.RequiredSkills = request.RequiredSkills;
                find.RequiredPriorities = request.RequiredPriorities;


                await _uow.SaveChangesAsync();
                return new ResponseBase(true);
            }
            _jobs.Add(new Job
            {
                CityId = request.CityId,
                CompanyId = company.Id,
                ContractTypeId = request.ContractTypeId,
                DateTime = DateTime.Now,
                ExpiredDateTime = DateTime.Now.AddDays(60),
                Description = request.Description,
                EducationLevelRequired = request.EducationLevelRequired,
                GenderRequired = request.GenderRequired,
                JobGroupId = request.JobGroupId,
                MilitaryStateRequired = request.MilitaryStateRequired,
                Title = request.Title,
                SalaryTypeId = request.SalaryTypeId,
                WorkingDays = request.WorkingDays,
                StateId = request.StateId,
                WorkExperienceYearId = request.WorkExperienceYearId,
                Status = request.Status,

                RequiredLoginToSite = request.RequiredLoginToSite,
                RequiredInformation = request.RequiredInformation,
                RequiredEducation = request.RequiredEducation,
                RequiredJob = request.RequiredJob,
                RequiredLanguage = request.RequiredLanguage,
                RequiredSkills = request.RequiredSkills,
                RequiredPriorities = request.RequiredPriorities,

            });
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
    }
}
