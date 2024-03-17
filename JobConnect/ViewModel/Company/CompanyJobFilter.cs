﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.States;

namespace ViewModel.Company
{
    public class CompanyJobFilter : PagingModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Display(Name = "عنوان آگهی")]
        public string JobTitle { get; set; }

        [Display(Name = " نوع قرارداد")]
        public int? ContractTypeId { get; set; }
        public List<LabelValue> ContractTypes { get; set; }


        [Display(Name = "گروه شغلی")]
        public int? JobGroupId { get; set; }
        public List<LabelValue> JobGroups { get; set; }


        [Display(Name = "شهر")]
        public int? CityId { get; set; }

        [Display(Name = "استان")]
        public int? StateId { get; set; }
        public List<GroupStateResponse> Cities { get; set; }

        [Display(Name = "سابقه کار")]
        public int? WorkExperienceYearId { get; set; }
        public List<LabelValue> WorkExperienceYears { get; set; }

        [Display(Name = "عنوان شغل")]
        public string Term { get; set; }

        [Display(Name = "حقوق درخواستی")]
        public int? SalaryRequestedId { get; set; }
        public List<LabelValue> SalaryRequesteds { get; set; }

        public List<CompanyJobResponse> Latest { get; set; }

        public int? JobId { get; set; }

        public List<LabelValue> PageLengths { get; set; }
        public int? TotalRowNumber { get; set; }

    }
}
