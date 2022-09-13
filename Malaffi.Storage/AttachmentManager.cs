using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Malaffi.Storage
{
    public static class AttachmentManager
    {
        public static void InsertAttachment(string fileName, Stream inputStream, string contentType, AttachmentTypes attachmentType, string fileUrl, string referanceId, int byteCount)
        {
            var generatedFileName = FileUploader.UploadFile(fileName, inputStream, contentType, attachmentType);
            fileUrl = String.Format(fileUrl, generatedFileName);
            Attachment attachment = new Attachment()
            {
                ID = Guid.NewGuid(),
                AttachmentType = attachmentType,
                CreatedOn = DateTime.UtcNow,
                MeaningfulFileName = fileName,
                MimeType = contentType,
                Name = generatedFileName,
                ReferenceID = referanceId,
                CreatedBy = null,
                URL = fileUrl,
                Size = byteCount / 1024
            };
            AttachmentRepository.InsertAttachment(attachment);
        }

        public static void DeleteAttachment(Guid attachemntId)
        {
            AttachmentRepository.DeleteAttachment(attachemntId);
        }

        public static Attachment GetAttachment(Guid attachmentId)
        {
            return AttachmentRepository.GetAttachment(attachmentId);
        }

        public static string GetAttachmentURLByReferanceId(AttachmentTypes type, string referenceId)
        {
            var result = "../Content/Imgs/image-not-available.png";
            var attachment = AttachmentRepository.GetAttachmentByReferanceId(type, referenceId);
            if (attachment != null)
            {
                result = attachment.URL;
            }
            return result;
        }

        public static Attachment GetAttachmentByReferanceId(AttachmentTypes type, string referenceId)
        {
            Attachment result = AttachmentRepository.GetAttachmentByReferanceId(type, referenceId);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public static IList<Attachment> GetAttachmentList(AttachmentTypes type, string referenceId)
        {
            return AttachmentRepository.GetAttachmentList(type, referenceId);
        }

        public static IList<string> GetAttachmentUrlList(AttachmentTypes type, string referenceId)
        {
            return AttachmentRepository.GetAttachmentList(type, referenceId).Select(a => a.URL).ToList();
        }

        public static List<string> GetAttachmentListURLByReferanceId(AttachmentTypes type, string referenceId)
        {
            var list = AttachmentRepository.GetAttachmentList(type, referenceId);
            List<string> res = new List<string>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    res.Add(item.URL);
                }

                return res;
            }

            return null;
        }

        public static void UpdateAttachmentReferenceId(string oldRefernceId, string newReferenceId)
        {
            AttachmentRepository.UpdateAttachmentReferenceId(oldRefernceId, newReferenceId);
        }
    }
}
