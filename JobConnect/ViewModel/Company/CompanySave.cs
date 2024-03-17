using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ViewModel.States;

namespace ViewModel.Company
{
    public class CompanySave
    {

        public int Id { get; set; }
        [Display(Name = "عنوان شرکت")]
        public string Title { get; set; }

        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }

        [Display(Name = "عنوان شرکت به انگلیسی")]
        public string EnglishName { get; set; }

        [Display(Name = "گروه شغلی")]
        public int? JobGroupId { get; set; }
        public List<LabelValue> JobGroups { get; set; }

        [Display(Name = "استان")]
        public int? StateId { get; set; }

       

        [Display(Name = "شهر")]
        public int? CityId { get; set; }
        public List<GroupStateResponse> Cities { get; set; }


        [Display(Name = "شماره تماس")]
        public string Phone { get; set; }

        [Display(Name = "وب سایت")]
        public string WebSite { get; set; }

        [Display(Name = "محله")]
        public string ZoneId { get; set; }
        public List<LabelValue> Zones { get; set; }


        [Display(Name = "سال تاسیس")]
        public int? CreatedYear { get; set; }

        [Display(Name = "اندازه سازمان")]
        public int? OrganizationSizeId { get; set; }
        public List<LabelValue> OrganizationSizes { get; set; }


        [Display(Name = "توضیحات شرکت")]
        public string Description { get; set; }

        [Display(Name = "خدمات و سرویس ها")]
        public string ServiceAndProducs { get; set; }

        [Display(Name = "لوگو شرکت")]
        public int? AttachmentLogoId { get; set; }
        public IFormFile AttachmentLogo { get; set; }
        public string AttachmentLogoString { get; set; }

        [Display(Name = "تصویر شرکت")]
        public int? AttachmentBackgroundId { get; set; }
        public IFormFile AttachmentBackground { get; set; }
        public string AttachmentBackgroundString { get; set; }

        public List<LabelValue> Attachments { get; set;}
    }
}
