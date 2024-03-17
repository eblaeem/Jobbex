using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using ViewModel;
using ViewModel.Attachment;

namespace Services
{
    public interface IAttachmentService
    {
        Task<List<AttachmentResponse>> Get(AttachmentFilter filter);
        Task<AttachmentResponse> GetById(int id, bool? needFileDataByte = false);
        Task<List<LabelValue>> GetTypes(List<int> attachmentTypes = null);
        Task<ResponseBase> Save(AttachmentSave attachment);
        Task<ResponseBase> Delete(int id);
        Task<AttachmentResponse> Download(AttachmentDownloadRequest request);
        Task<AttachmentResponse> StaticDownload(StaticDownloadFilter request);
        Task<bool> UserHaveSignatureSampleForm(int userId);
        Task<ResponseBase> ConfirmAttachment(int id, int userId);
        Task<ResponseBase> RejectAttachment(int id, int userId);
        string GetAttachmentHtml(AttachmentResponse request);
        ValidateFileResponse ValidateFile(int? attachmentTypeId, bool imageOrPdf, IFormFile file);
    }
    public class AttachmentService : IAttachmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Attachment> _attachments;
        private readonly IUsersService _usersService;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AttachmentService(IUnitOfWork uow,
            IUsersService usersService,
            ICommonService commonService,
            IWebHostEnvironment webHostEnvironment)
        {
            _uow = uow;
            _attachments = _uow.Set<Attachment>();
            _usersService = usersService;
            _commonService = commonService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<AttachmentResponse>> Get(AttachmentFilter filter)
        {
            var attamnetTypes = new List<string>
                {
                    "UserProfileImage"
                };

            var query = from attachment in _attachments
                        from attachmentType in _uow.Set<AttachmentType>().Where(w => w.Id == attachment.AttachmentTypeId)
                        from user in _uow.Set<User>().Where(w => w.Id == attachment.UserId)
                        select new AttachmentResponse
                        {
                            FileDataByte = filter.WithData == true ? attachment.FileData : null,
                            AttachmentTypeName = attachmentType.Name,
                            AttachmentTypeId = attachmentType.Id,
                            DisplayName = user.DisplayName,
                            FileName = attachment.FileName,
                            FileSize = attachment.FileSize,
                            Id = attachment.Id,
                            RecordId = attachment.RecordId,
                            AttachmentTypeEnglishName = attachmentType.EnglishName,
                            UserId = attachment.UserId,
                            NationalCode = user.NationalCode,
                            FileType = attachment.FileType,
                            Guid = attachment.Guid.ToString(),
                            Reject = attachment.Reject,
                            Confirm = attachment.Confirm,
                            ConfirmDateTime = attachment.ConfirmDateTime,
                            RejectDateTime = attachment.RejectDateTime,
                            IsRegisterableToIme = attamnetTypes.Any(a => a == attachmentType.EnglishName)
                        };

            if (filter.RecordId > 0)
            {
                query = query.Where(w => w.RecordId == filter.RecordId);
            }

            if (filter.RecordIds.Any())
            {
                query = query.Where(w => filter.RecordIds.Any(a => a == w.RecordId.GetValueOrDefault()));
            }

            var currentUserIsAdmin = await _usersService.CurrentUserIsAdmin();
            if (currentUserIsAdmin is null)
            {
                var userId = filter.UserId != null ? filter.UserId : _usersService.GetCurrentUserId();
                query = query.Where(w => w.UserId == userId);
            }

            if (filter.AttachmentTypeId > 0)
            {
                query = query.Where(w => w.AttachmentTypeId == filter.AttachmentTypeId);
            }
            if (filter.AttachmentTypes.Any())
            {
                query = query.Where(w => filter.AttachmentTypes.Any(a => a == w.AttachmentTypeId.GetValueOrDefault()));
            }

            var response = query.OrderByDescending(o => o.Id).AsNoTracking().ToList();

            foreach (var item in response)
            {
                item.ConfirmDateTimeString = item.ConfirmDateTime.ToShortPersianDateString();
                item.RejectDateTimeString = item.RejectDateTime.ToShortPersianDateString();

                if (string.IsNullOrEmpty(Path.GetExtension(item.FileName)))
                {
                    if (string.IsNullOrEmpty(item.FileType) == false
                        && item.FileType.StartsWith(".") == false)
                    {
                        item.FileType = $".{item.FileType}";
                    }
                    item.FileName = item.FileName + item.FileType;
                }
                if (filter.WithData == true && item.FileDataByte != null)
                {
                    var base64 = Convert.ToBase64String(item.FileDataByte);
                    var mimType = base64.Substring(0, 5).ToLower();
                    item.IsImage = _commonService.IsImage(item.FileType, mimType);
                    if (item.IsImage)
                    {
                        if (filter.ShowThumbnail == true)
                        {
                            var stream = new MemoryStream(item.FileDataByte);
                            var image = Image.FromStream(stream);
                            var thumb = image.GetThumbnailImage(280, 220, () => false, IntPtr.Zero);
                            var newStream = new MemoryStream();
                            thumb.Save(newStream, ImageFormat.Png);

                            var bytes = newStream.ToArray();
                            item.FileData = Convert.ToBase64String(bytes);
                        }
                        else
                        {
                            item.FileData = base64;
                        }
                    }
                    item.IsPdf = _commonService.IsPdf(item.FileType, mimType);
                }
            }
            return response;
        }
        public async Task<ResponseBase> Save(AttachmentSave request)
        {
            if (request.FormFile is not null)
            {
                var validateFile = ValidateFile(request.AttachmentTypeId, request.ImageOrPdf, request.FormFile);

                if (validateFile.IsValid == false)
                {
                    return new ResponseBase(false, validateFile.Message);
                }

                request.FileDataBytes = validateFile.Bytes;
            }

            var attachment = new Attachment()
            {
                AttachmentTypeId = request.AttachmentTypeId,
                FileName = request.FileName,
                FileSize = request.FileSize,
                InsertDateTime = DateTime.Now,
                UserId = request.UserId > 0 ? request.UserId : _usersService.GetCurrentUserId(),
                RecordId = request.RecordId,
                Guid = Guid.NewGuid(),
            };
            if (request.RecordId is null)
            {
                attachment.RecordId = attachment.UserId;
            }

            if (string.IsNullOrEmpty(request.FileData) == false)
            {
                var base64 = request.FileData.Split(',');
                var extension = _commonService.GetFileExtension(base64[1].Substring(0, 5));
                attachment.FileType = extension;
                attachment.FileData = Convert.FromBase64String(base64[1]);
            }
            if (request.FileDataBytes != null)
            {
                attachment.FileData = request.FileDataBytes;
                attachment.FileType = request.FileType;
            }
            attachment.FileSize = attachment.FileData.Length;

            if (request.Id > 0)
            {
                var find = _attachments.Find(request.Id);
                if (find is null)
                {
                    return new ResponseBase(false, "شناسه مورد نظر وجود ندارد");
                }
                attachment.Id = request.Id.GetValueOrDefault();

                find.FileData = attachment.FileData;
                find.FileType = attachment.FileType;
                find.FileName = attachment.FileName;
            }
            else
            {
                _attachments.Add(attachment);
            }
            await _uow.SaveChangesAsync();
            return new ResponseBase(true)
            {
                Message = attachment.Guid.ToString(),
                Id = attachment.Id,
            };
        }
        public async Task<ResponseBase> Delete(int id)
        {
            var find = _attachments.Find(id);
            if (find is null)
            {
                return new ResponseBase(false, "شناسه مورد نظر وجود ندارد");
            }
            _attachments.Remove(find);
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }

        public async Task<List<LabelValue>> GetTypes(List<int> attachmentTypes = null)
        {
            var query = _uow.Set<AttachmentType>().AsQueryable();
            if (attachmentTypes != null && attachmentTypes.Any())
            {
                query = query.Where(c => attachmentTypes.Any(a => a == c.Id));
            }
            var response = await query.Select(s => new LabelValue()
            {
                Label = s.Name,
                Value = s.Id
            }).AsNoTracking().ToListAsync();
            return response;
        }

        public async Task<AttachmentResponse> GetById(int id, bool? needFileDataByte = false)
        {
            if (id == 0)
            {
                return null;
            }
            var find = await (from attachment in _attachments.Where(w => w.Id == id)
                              from attachmentType in _uow.Set<AttachmentType>()
                              .Where(w => w.Id == attachment.AttachmentTypeId)
                              select new
                              {
                                  attachment,
                                  attachmentType
                              }).FirstOrDefaultAsync();

            if (find is null)
            {
                return null;
            }
            var userId = _usersService.GetCurrentUserId();
            //todo check company user

            //var currentUserIsAdmin = await _usersService.CurrentUserIsAdmin();
            //if (find.attachment.UserId != userId && currentUserIsAdmin == null && find.attachmentType.Id != 17)
            //{
            //    return null;
            //}

            var response = new AttachmentResponse()
            {
                Id = find.attachment.Id,
                FileName = find.attachment.FileName,
                FileType = find.attachment.FileType,
                FileSize = find.attachment.FileSize,
                AttachmentTypeId = find.attachment.AttachmentTypeId,
                AttachmentTypeName = find.attachmentType.Name,
                RecordId = find.attachment.RecordId,
            };
            if (needFileDataByte == true)
            {
                response.FileDataByte = find.attachment.FileData;
            }
            else
            {
                var base64 = Convert.ToBase64String(find.attachment.FileData);
                response.FileData = base64;
                response.MimeType = base64.Substring(0, 5).ToLower();
            }
            return response;
        }

        public async Task<AttachmentResponse> Download(AttachmentDownloadRequest request)
        {
            var response = new AttachmentResponse();
            if (request.Id > 0)
            {
                response = await GetById(request.Id.GetValueOrDefault());
            }
            if (string.IsNullOrEmpty(request.Guid) == false)
            {
                var find = await _attachments.FirstOrDefaultAsync(c => c.Guid.ToString() == request.Guid);
                response = await GetById(find.Id);
            }
            if (response == null)
            {
                return new AttachmentResponse()
                {
                    IsValid = false,
                    Message = "فایل مورد نظر وجود ندارد"
                };
            }
            response.IsValid = true;

            if (request.ShowHtmlResponse == true)
            {
                response.HtmlResponse = GetAttachmentHtml(response);
                response.FileDataByte = null;
                response.FileData = string.Empty;
            }
            return response;
        }
        public async Task<AttachmentResponse> StaticDownload(StaticDownloadFilter request)
        {
            var response = new AttachmentResponse();
            var reportFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "StaticFile", request.Name);
            if (File.Exists(reportFilePath) == false)
            {
                return null;
            }
            var fileDataByte = File.ReadAllBytes(reportFilePath);
            //response.FileDataByte = File.ReadAllBytes(reportFilePath);
            response.FileData = Convert.ToBase64String(fileDataByte);
            response.FileName = request.Name;

            return response;
        }

        public async Task<bool> UserHaveSignatureSampleForm(int userId)
        {
            return await _attachments.AnyAsync(c => c.UserId == userId && c.AttachmentTypeId == 3);
        }
        public async Task<ResponseBase> ConfirmAttachment(int id, int userId)
        {
            var response = _attachments.Find(id);
            if (response is null)
            {
                return new ResponseBase(false, "ضمیمه مورد نظر وجود ندارد");
            }
            if (response.UserId != userId)
            {
                return new ResponseBase(false, "ضمیمه برای مشتری مورد نظر نمی باشد");
            }
            response.Confirm = true;
            response.Reject = false;

            response.ConfirmDateTime = DateTime.Now;

            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
        public async Task<ResponseBase> RejectAttachment(int id, int userId)
        {
            var response = _attachments.Find(id);
            if (response is null)
            {
                return new ResponseBase(false, "ضمیمه مورد نظر وجود ندارد");
            }
            if (response.UserId != userId)
            {
                return new ResponseBase(false, "ضمیمه برای مشتری مورد نظر نمی باشد");
            }

            response.Reject = true;
            response.Confirm = false;
            response.RejectDateTime = DateTime.Now;

            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
        public string GetAttachmentHtml(AttachmentResponse response)
        {
            var fileType = _commonService.GetFileType(response);
            response.IsImage = fileType.IsImage;
            response.IsPdf = fileType.IsPdf;
            response.IsAudio = fileType.IsAudio;
            response.IsVideo = fileType.IsVideo;

            var htmlResponse = string.Empty;
            if (response.IsImage)
            {
                htmlResponse = _commonService.DownloadImageHtmlResponse(response.FileName, response.FileSize, response.FileData);
            }
            else if (response.IsVideo)
            {
                htmlResponse = _commonService.DownloadVideoHtmlResponse(response.FileName, response.FileSize, response.FileData);
            }
            else if (response.IsAudio)
            {
                htmlResponse = _commonService.DownloadAudioHtmlResponse(response.FileName, response.FileSize, response.FileData);
            }
            else
            {
                htmlResponse = _commonService.DownloadFileHtmlResponse(response.FileName, response.FileSize, response.Id);
                response.FileData = string.Empty;
            }
            return htmlResponse;
        }
        public ValidateFileResponse ValidateFile(int? attachmentTypeId, bool imageOrPdf, IFormFile file)
        {
            if (attachmentTypeId == null)
            {
                return new ValidateFileResponse(false, "");
            }
            if (file == null)
            {
                return new ValidateFileResponse(false, "ضمیمه را وارد نمایید");
            }
            if (file.Length > 5 * 1024 * 1024)
            {
                return new ValidateFileResponse(false, "حداکثر سایز آپلودی 5 مگابایت می باشد");
            }

            byte[] bytes = _commonService.GetBytes(file);

            var isImage = _commonService.IsImage(file);

            var isPdf = false;
            if (isImage == false)
            {
                isPdf = _commonService.IsPdf(file);

                if (isPdf == true && _commonService.IsValidPdf(bytes) == false)
                {
                    return new ValidateFileResponse(false, "فرمت  پی دی اف صحیح نمی باشد");
                }
            }
            if (imageOrPdf)
            {
                if (isPdf == false && isImage == false)
                {
                    return new ValidateFileResponse(false, "فرمت تصویر یا پی دی اف می باشد");
                }
            }

            return new ValidateFileResponse
            {
                IsValid = true,
                Bytes = bytes
            };
        }
    }
}
