using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ViewModel;

namespace Services
{
    public interface IDbInitializerService
    {
        void Initialize();
        void SeedData();
    }

    public class DbInitializerService : IDbInitializerService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISecurityService _securityService;

        public DbInitializerService(
            IServiceScopeFactory scopeFactory,
            ISecurityService securityService)
        {
            _scopeFactory = scopeFactory;
            _securityService = securityService;
        }

        public void Initialize()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            //context.Database.Migrate();
        }

        public void SeedData()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

            var adminRole = new Role { Id = 2, Name = CustomRoles.Admin };
            var userRole = new Role { Id = 1, Name = CustomRoles.User };
            if (!context.Roles.Any())
            {
                context.Add(adminRole);
                context.Add(userRole);
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    Username = "Hossein",
                    DisplayName = "حسین",
                    IsActive = true,
                    LastLoggedIn = null,
                    Password = _securityService.GetSha256Hash("admin101211"),
                    SerialNumber = Guid.NewGuid().ToString("N"),
                    Email = "Hossein@gmail.com",
                    FirstName = "حسین",
                    LastName = "حقیان",
                    NationalCode = "0493399445",
                    PhoneNumber = "989366493383"
                };
                context.Add(adminUser);
                context.SaveChanges();

                context.Add(new UserRole { Role = adminRole, User = adminUser });
                context.Add(new UserRole { Role = userRole, User = adminUser });
                context.SaveChanges();
            }
            if (!context.UserRequestedStatus.Any())
            {
                var list = new List<UserRequestedStatus>()
                {
                    new UserRequestedStatus { Name="بررسی نشده"},
                    new UserRequestedStatus { Name=" در انتظار تعیین وضعیت" },
                    new UserRequestedStatus { Name="تایید برای مصاحبه" },
                    new UserRequestedStatus { Name="استخدام شده" },
                    new UserRequestedStatus { Name="رد شده" },
                    new UserRequestedStatus { Name="منقضی‌شده" },
                    new UserRequestedStatus { Name="درخواست لغو‌شده" }
                };
                foreach (var item in list)
                {
                    context.UserRequestedStatus.Add(item);
                }
                context.SaveChanges();
            }
            if (!context.OrganizationSizes.Any())
            {
                var list = new List<OrganizationSize>()
                {
                    new OrganizationSize { Name="یک نفر"},
                    new OrganizationSize { Name="2-10 نفر" },
                    new OrganizationSize { Name="11-50 نفر" },
                    new OrganizationSize { Name="51-200 نفر" },
                    new OrganizationSize { Name="201-500 نفر" },
                    new OrganizationSize { Name="بیش از 500 نفر" }
                };
                foreach (var item in list)
                {
                    context.OrganizationSizes.Add(item);
                }
                context.SaveChanges();
            }
            if (!context.WorkExperienceYears.Any())
            {
                var list = new List<WorkExperienceYear>()
                {
                    new WorkExperienceYear { Name="مهم نیست"},
                    new WorkExperienceYear { Name="کمتر از سه سال" },
                    new WorkExperienceYear { Name="سه تا شش سال" },
                    new WorkExperienceYear { Name="بیش از شش سال" },
                };
                foreach (var item in list)
                {
                    context.WorkExperienceYears.Add(item);
                }
                context.SaveChanges();
            }

            if (!context.Benefit.Any())
            {
                var list = new List<Benefit>()
                {
                    new Benefit { Name="بیمه"},
                    new Benefit { Name="بیمه تکمیلی" },
                    new Benefit { Name="تهیه غذا" },
                    new Benefit { Name="دوره های آموزشی" },
                    new Benefit { Name="سرویس رفت آمد" },
                    new Benefit { Name="ساعت کاری منعطف" },
                };
                foreach (var item in list)
                {
                    context.Benefit.Add(item);
                }
                context.SaveChanges();
            }
            if (!context.ContractTypes.Any())
            {
                var list = new List<ContractType>()
                {
                    new ContractType { Name="تمام ‌وقت"},
                    new ContractType { Name="پاره ‌وقت" },
                    new ContractType { Name="تهیه غذا" },
                    new ContractType { Name="دورکاری" },
                    new ContractType { Name="کارآموزی" }
                };
                foreach (var item in list)
                {
                    context.ContractTypes.Add(item);
                }
                context.SaveChanges();
            }
            if (!context.SkillLevels.Any())
            {
                var list = new List<SkillLevel>()
                {
                    new SkillLevel { Name="آشنا"},
                    new SkillLevel { Name="کم" },
                    new SkillLevel { Name="متوسط" },
                    new SkillLevel { Name="خوب" },
                    new SkillLevel { Name="عالی" }
                };
                foreach (var item in list)
                {
                    context.SkillLevels.Add(item);
                }
                context.SaveChanges();
            }
            if (!context.EducationLevels.Any())
            {
                var list = new List<EducationLevel>()
                {
                    new EducationLevel {Id=1, Name="بی سواد"},
                    new EducationLevel {Id=2, Name="زیر دیپلم" },
                    new EducationLevel {Id=3, Name="دیپلم" },
                    new EducationLevel {Id=4, Name="کاردانی" },
                    new EducationLevel {Id=5, Name="کارشناسی" },
                    new EducationLevel {Id=6, Name="کارشناسی ارشد" },
                    new EducationLevel {Id=7, Name="دکتری" },
                    new EducationLevel {Id=8, Name="فوق دکتری" },
                    new EducationLevel {Id=9, Name="سایر" }
                };
                foreach (var item in list)
                {
                    context.EducationLevels.Add(item);
                }
                context.SaveChanges();
            }

            if (!context.MaritalStatus.Any())
            {
                var list = new List<MaritalStatus>()
                {
                    new MaritalStatus { Name="مجرد"},
                    new MaritalStatus { Name="متاهل" }
                };
                foreach (var item in list)
                {
                    context.MaritalStatus.Add(item);
                }
                context.SaveChanges();
            }
            if (!context.MilitaryStatus.Any())
            {
                var list = new List<MilitaryStatus>()
                {
                    new MilitaryStatus { Name="مشمول"},
                    new MilitaryStatus { Name="كارت پايان خدمت" },
                    new MilitaryStatus { Name="كارت‌ معافيت‌ دائم"},
                    new MilitaryStatus { Name="معافيت موقت بدون غيبت" },
                    new MilitaryStatus { Name="سرباز در حال خدمت"}
                };
                foreach (var item in list)
                {
                    context.MilitaryStatus.Add(item);
                }
                context.SaveChanges();
            }
            if (!context.SalaryRequestedTypes.Any())
            {
                var list = new List<SalaryRequestedType>()
                {
                    new SalaryRequestedType { Name="توافقی"},
                    new SalaryRequestedType { Name="حقوق وزارت کار"},
                    new SalaryRequestedType { Name="بین 2 تا 3 میلیون تومان"},
                    new SalaryRequestedType { Name="بین 3 تا 5 میلیون تومان" },
                    new SalaryRequestedType { Name="بین 5 تا 8 میلیون تومان"},
                    new SalaryRequestedType { Name="بین 8 تا 10 میلیون تومان" },
                    new SalaryRequestedType { Name="بین 10 تا 13 میلیون تومان"},
                    new SalaryRequestedType { Name="بین 13 تا 18 میلیون تومان"},
                    new SalaryRequestedType { Name="بین 18 تا 22 میلیون تومان"},
                    new SalaryRequestedType { Name="بین 22 تا 30 میلیون تومان"},
                    new SalaryRequestedType { Name="بیشتر از 30 میلیون تومان"}
                };
                foreach (var item in list)
                {
                    context.SalaryRequestedTypes.Add(item);
                }
                context.SaveChanges();
            }

        }
    }
}