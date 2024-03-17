using DataLayer.Context;
using Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using ViewModel;
using ViewModel.Attachment;
namespace Services
{
    public interface ICommonService
    {
        int GetCurrentUserId();
        int GetCurrentCompanyId();
        ValueTask<User> GetCurrentUser();
        AttachmentFileType GetFileType(AttachmentResponse response);
        bool CheckFileExtension(string base64String, List<string> allowedExtension);
        string GetFileExtension(string data);
        bool IsImage(string fileType, string mimType);
        bool IsImage(IFormFile postedFile);
        bool IsPdf(IFormFile postedFile);
        bool IsPdf(string fileType, string mimType);
        bool IsVideo(string fileType, string mimType);
        bool IsAudio(string fileType, string mimType);
        string GetColumns(Type model, bool isOperation = false);
        List<Column> ListColumns(Type model, bool isOperation = false);
        string ToXml<T>(T dataToSerialize);
        public byte[] ConvertToPdf(IFormFile file);
        public byte[] ConvertToPdf(byte[] bytes);
        public bool IsValidPdf(byte[] bytes);
        public byte[] GetBytes(IFormFile file);
        string GenerateToken();


        IFormFile Base64ToImage(string filedata, string name, string type);
        string ReplaceAgentBrowserName(string agentName);
        string ReplcaeMobileNumberZero(string mobileNumber);
        string ReplcaeMobileNumber98(string mobileNumber);
        string DownloadImageHtmlResponse(string fileName, int fileSize, string result);
        string DownloadVideoHtmlResponse(string fileName, int fileSize, string result);
        string DownloadAudioHtmlResponse(string fileName, int fileSize, string result);
        string DownloadFileHtmlResponse(string fileName, int fileSize, int? id);

        List<LabelValue> GetSalaryRequesteds();
        List<LabelValue> GetWorkExperienceYears();
        List<LabelValue> GetGenders();
        List<LabelValue> GetMilitaryStatus();
        List<LabelValue> GetMaritalStatus();
        List<LabelValue> GetDegrees();
        List<LabelValue> GetSkillLevels();
        List<LabelValue> GetContractTypes();
        List<LabelValue> GetJobBenefits();
        List<LabelValue> GetPageLengths();
        List<LabelValue> GetSortingTypes();
    }
    public class CommonService : ICommonService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        public CommonService(IHttpContextAccessor contextAccessor,
            IUnitOfWork unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
        }
        public int GetCurrentUserId()
        {
            var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
            var userId = userDataClaim?.Value;
            return string.IsNullOrWhiteSpace(userId) ? 0 : int.Parse(userId);
        }
        public int GetCurrentCompanyId()
        {
            var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var companyId = claimsIdentity?.FindFirst("companyId")?.Value;
            return string.IsNullOrWhiteSpace(companyId) ? 0 : int.Parse(companyId);
        }
        public async ValueTask<User> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            return await _unitOfWork.Set<User>().FindAsync(userId);
        }

        public AttachmentFileType GetFileType(AttachmentResponse attachment)
        {
            if (IsImage(attachment.FileType, attachment.MimeType))
            {
                return new AttachmentFileType { IsImage = true };
            }
            if (IsPdf(attachment.FileType, attachment.MimeType))
            {
                return new AttachmentFileType { IsPdf = true };
            }
            if (IsVideo(attachment.FileType, attachment.MimeType))
            {
                return new AttachmentFileType { IsVideo = true };
            }
            if (IsAudio(attachment.FileType, attachment.MimeType))
            {
                return new AttachmentFileType { IsAudio = true };
            }

            return new AttachmentFileType
            {
                OtherFiles = true
            };
        }
        public bool CheckFileExtension(string base64String, List<string> allowedExtension)
        {
            var data = base64String.Substring(0, 5);
            var extension = GetFileExtension(data);
            return allowedExtension.Contains(extension);
        }
        public string GetFileExtension(string data)
        {
            var extension = string.Empty;
            switch (data.ToUpper())
            {
                case "IVBOR":
                    extension = ".png";
                    break;
                case "/9J/4":
                    extension = ".jpg";
                    break;
                case "JVBER":
                    extension = ".pdf";
                    break;
                case "AAAAF":
                    extension = ".mp4";
                    break;
                case "AAABA":
                    extension = ".ico";
                    break;
                case "UMFYI":
                    extension = ".rar";
                    break;
                case "E1XYD":
                    extension = ".rtf";
                    break;
                case "U1PKC":
                    extension = ".txt";
                    break;
                case "UESDB":
                case "0M8R4":
                    extension = ".xlsx";
                    break;
                case "MQOWM":
                case "77U/M":
                    extension = ".txt";
                    break;
            }
            return extension;
        }
        public bool IsImage(string fileType, string mimType)
        {
            var imageList = new List<string>
            {
                ".jpg", ".png",".gif",".jpeg"
            };
            if (imageList.Any(a => string.Equals(a, fileType, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            if (fileType.StartsWith("image/"))
            {
                return true;
            }

            if (mimType == "ivbor" || mimType == "/9j/4")
            {
                return true;
            }
            return false;
        }
        public bool IsImage(IFormFile postedFile)
        {
            var ImageMinimumBytes = 512;
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }

                if (postedFile.Length < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                using System.Drawing.Bitmap bitmap = new(postedFile.OpenReadStream());
                var imageHeight = bitmap.Height;
                var imageWidth = bitmap.Width;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.OpenReadStream().Position = 0;
            }

            return true;
        }
        public bool IsPdf(string fileType, string mimType)
        {
            if (string.Equals(fileType, ".pdf", StringComparison.OrdinalIgnoreCase) || mimType == "jvber")
            {
                return true;
            }
            if (fileType.StartsWith("application/pdf"))
            {
                return true;
            }
            return false;
        }
        public bool IsPdf(IFormFile postedFile)
        {
            var supportedTypes = new[] { "pdf" };
            var fileExt = Path.GetExtension(postedFile.FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
            {
                return false;
            }
            return true;
        }

        public bool IsVideo(string fileType, string mimType)
        {
            var list = new List<string>
            {
                ".mp4",".avi"
            };
            if (list.Any(a => string.Equals(a, fileType, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            if (fileType.StartsWith("video/"))
            {
                return true;
            }

            //if (mimType == "ivbor" || mimType == "/9j/4")
            //{
            //    return true;
            //}
            return false;
        }
        public bool IsAudio(string fileType, string mimType)
        {
            var list = new List<string>
            {
                ".mp3",".wma"
            };
            if (list.Any(a => string.Equals(a, fileType, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            if (fileType.StartsWith("audio/"))
            {
                return true;
            }
            return false;
        }

        public string GetColumns(Type model, bool isOperation = false)
        {
            var instance = Activator.CreateInstance(model);
            if (instance is not IDataTable table)
            {
                return string.Empty;
            }
            var columns = table.GetColumns();
            if (isOperation == true)
            {
                columns.Add(new Column("Operation", "عملیات") { HideInExcel = true });
            }
            if (columns.Any() == false)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(columns, new JsonSerializerSettings
            {
                //ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
        public List<Column> ListColumns(Type model, bool isOperation = false)
        {
            var instance = Activator.CreateInstance(model);
            if (instance is not IDataTable table)
            {
                return new List<Column>();
            }
            var columns = table.GetColumns();
            return columns;
        }

        public string ToXml<T>(T dataToSerialize)
        {
            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, dataToSerialize);
            return stringwriter.ToString();
        }

        public byte[] ConvertToPdf(byte[] bytes)
        {
            try
            {
                return GetMemoryStream(bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public byte[] ConvertToPdf(IFormFile file)
        {
            try
            {
                var bytes = GetBytes(file);
                return GetMemoryStream(bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private byte[] GetMemoryStream(byte[] bytes)
        {
            var document = new Document(PageSize.A4, 188f, 88f, 5f, 10f);
            using var stream = new MemoryStream();
            PdfWriter.GetInstance(document, stream);
            document.Open();
            var image = Image.GetInstance(bytes);
            image.ScaleAbsolute(600, 600);
            image.SetAbsolutePosition((PageSize.A4.Width - image.ScaledWidth) / 2, (PageSize.A4.Height - image.ScaledHeight) / 2);

            document.Add(image);
            document.Close();

            return stream.ToArray();
        }
        public bool IsValidPdf(byte[] bytes)
        {
            try
            {
                new PdfReader(bytes);
                return true;
            }
            catch (iTextSharp.text.exceptions.InvalidPdfException)
            {
                return false;
            }
        }
        public byte[] GetBytes(IFormFile file)
        {
            using var target = new MemoryStream();
            file.CopyTo(target);
            return target.ToArray();

        }
        public string GenerateToken()
        {
            return Convert.ToBase64String(
                 BitConverter.GetBytes(DateTime.UtcNow.ToBinary()).
                 Concat(Guid.NewGuid().ToByteArray()).ToArray());
        }
        public IFormFile Base64ToImage(string filedata, string name, string type)
        {
            var bytes = Convert.FromBase64String(filedata);
            var stream = new MemoryStream(bytes);
            var result = new FormFile(stream, 0, bytes.Length, name, name)
            {
                Headers = new HeaderDictionary(),
                ContentType = type
            };
            return result;
        }
        public string ReplaceAgentBrowserName(string agentName)
        {
            var agentReplace = new List<string>()
            {
                "(",")","Mozilla/5.0","Windows NT 10.0","Win64; x64",";","AppleWebKit/537.36 KHTML, like Gecko",
                "rv:101.0 Gecko/20100101"
            };
            foreach (var agent in agentReplace)
            {
                agentName = agentName.Replace(agent, "");
            }
            return agentName;
        }

        public string ReplcaeMobileNumberZero(string mobileNumber)
        {
            var mobile = mobileNumber.ToString();
            if (mobile != null && mobile.StartsWith("98"))
            {
                mobile = mobile.Trim().Replace(" ", "");
                mobile = mobile.Substring(0, 2).Replace("98", "0") + mobile.Substring(2, 10);
            }
            return mobile;
        }
        public string ReplcaeMobileNumber98(string mobileNumber)
        {
            var mobile = mobileNumber.ToString();
            if (mobile != null && mobile.StartsWith("0"))
            {
                mobile = mobile.Trim().Replace(" ", "");
                mobile = mobile.Substring(0, 1).Replace("0", "98") + mobile.Substring(1, 10);
            }
            return mobile;
        }

        public string DownloadImageHtmlResponse(string fileName, int fileSize, string result)
        {
            var fileSizeString = SizeSuffix(fileSize);
            return $"<img  alt='{fileName}' class='img-responsive img-thumbnail chat-thumbnail cursor-pointer' src='data:image/jpeg;base64,{result}' />" +
                    $"{fileSizeString}";
        }

        public string DownloadFileHtmlResponse(string fileName, int fileSize, int? id)
        {
            var fileSizeString = SizeSuffix(fileSize);
            return $"<a data-id='{id}' dir='ltr' class='attachment-download btn btn-default margin-top-5'> " +
                   $"<i class='fa fa-file'></i> {fileName} {fileSizeString}</a>";
        }
        public string DownloadVideoHtmlResponse(string fileName, int fileSize, string result)
        {
            var fileSizeString = SizeSuffix(fileSize);
            return $"<video controls='' style='width:100%;'><source type='video/webm' src='data:video/webm;base64,{result}' /></video>" +
                   $"{fileSizeString}";
        }
        public string DownloadAudioHtmlResponse(string fileName, int fileSize, string result)
        {
            var fileSizeString = SizeSuffix(fileSize);
            return $"<audio controls='' style='width:100%;'><source type='audio/mp3' src='data:audio/mp3;base64,{result}' /></audio>" +
                   $"{fileSizeString}";
        }

        private string SizeSuffix(int value, int decimalPlaces = 1)
        {
            string[] SizeSuffixes =
            { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (decimalPlaces < 0)
            {
                throw new ArgumentOutOfRangeException("decimalPlaces");
            }
            if (value < 0)
            {
                return "-" + SizeSuffix(-value, decimalPlaces);
            }
            if (value == 0)
            {
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);
            }

            int mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }
            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        public List<LabelValue> GetSalaryRequesteds()
        {
            return _unitOfWork.Set<SalaryRequestedType>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }
        public List<LabelValue> GetWorkExperienceYears()
        {
            return _unitOfWork.Set<WorkExperienceYear>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }
        public List<LabelValue> GetGenders()
        {
            return _unitOfWork.Set<Gender>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }
        public List<LabelValue> GetMilitaryStatus()
        {
            return _unitOfWork.Set<MilitaryStatus>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }
        public List<LabelValue> GetMaritalStatus()
        {
            return _unitOfWork.Set<MaritalStatus>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }
        public List<LabelValue> GetDegrees()
        {
            return _unitOfWork.Set<EducationLevel>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }
        public List<LabelValue> GetSkillLevels()
        {
            return _unitOfWork.Set<SkillLevel>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }
        public List<LabelValue> GetContractTypes()
        {
            return _unitOfWork.Set<ContractType>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }
        public List<LabelValue> GetJobBenefits()
        {
            return _unitOfWork.Set<Benefit>().Select(item => new LabelValue()
            {
                Label = item.Name,
                Value = item.Id
            }).ToList();
        }

        public List<LabelValue> GetPageLengths()
        {
            return new List<LabelValue>{
                new LabelValue(5,"5"),
                new LabelValue(10,"10"),
                new LabelValue(20,"20"),
                new LabelValue(50,"50"),
                new LabelValue(100,"100"),
            };
        }

        public List<LabelValue> GetSortingTypes()
        {
            return new List<LabelValue>{
                new LabelValue(1,"مرتبط ترین"),
                new LabelValue(2,"جدیدترین"),
                new LabelValue(3,"بیشترین امتیاز")
            };
        }
    }
}
