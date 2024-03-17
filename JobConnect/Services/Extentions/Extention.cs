using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Job;
using ViewModel.UserRecommended;

namespace Services.Extentions
{
    public static class Extention
    {
        public static IQueryable<UserRecommendedResponse> AddRank(this IQueryable<UserRecommendedResponse> responses, int rank)
        {
            return responses.Select(s => new UserRecommendedResponse
            {
                Id = s.Id,
                CompanyName = s.CompanyName,
                CityId = s.CityId,
                CityName = s.CityName,
                CompanyRate = s.CompanyRate,
                AttachmentLogoId = s.AttachmentLogoId,
                ContractTypeName = s.ContractTypeName,
                DateTime = s.DateTime,
                JobTitle = s.JobTitle,
                SalaryTypeName = s.SalaryTypeName,
                JobGroupId = s.JobGroupId,
                ContractTypeId = s.ContractTypeId,
                //JobBenefits = s.JobBenefits,
                RankScore = rank,
                CompanyCreatedYear = s.CompanyCreatedYear,
                CompanyDescription = s.CompanyDescription,
                CompanyOrganizationSize = s.CompanyOrganizationSize,
                EducationLevelRequiredTitle = s.EducationLevelRequiredTitle,
                ExpiredDateTime = s.ExpiredDateTime,
                //GenderRequiredTitle = y.Min(m => m.GenderRequiredTitle),
                //JobSkill = s.JobSkill,
                JobStatus = s.JobStatus,

                MilitaryStateRequiredTitle = s.MilitaryStateRequiredTitle,

                WorkExperienceYearTitle = s.WorkExperienceYearTitle,
                WorkingDays = s.WorkingDays,
                ZoneName = "میرداماد",
                SalaryTypeId = s.SalaryTypeId
            });
        }

        public static string RelativeDate(this DateTime theDate)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - theDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * minute)
                return ts.Seconds == 1 ? "یک ثانیه قبل" : ts.Seconds + " ثانیه قبل";

            if (delta < 2 * minute)
                return "یک دقیقه قبل";

            if (delta < 45 * minute)
                return ts.Minutes + " دقیقه قبل";

            if (delta < 90 * minute)
                return "یک ساعت قبل";

            if (delta < 24 * hour)
                return ts.Hours + " ساعت قبل";

            if (delta < 48 * hour)
                return "دیروز";

            if (delta < 30 * day)
                return ts.Days + " روز قبل";

            if (delta < 12 * month)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "یک ماه قبل" : months + " ماه قبل";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "یک سال قبل" : years + " سال قبل";
            }
        }
    }
}
