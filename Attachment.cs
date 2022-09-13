using System;

namespace Malaffi.Storage
{
    public class Attachment
    {
        public Guid ID { get; set; }
        public string MeaningfulFileName { get; set; }
        public string ReferenceID { get; set; }
        public AttachmentTypes AttachmentType { get; set; }
        public string URL { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public int Size { get; set; }
    }

    public enum AttachmentTypes
    {
        all = -1,
        TestFile = 1,
    }

    public class Constants
    {
        public static string SP_InsertAttachment = "[dbo].[AttachmentInsert]";
        public static string SP_GetAttachmentByID = "[dbo].[AttachmentGetByID]";
        public static string SP_ListAttachments = "[dbo].[AttachmentsList]";
        public static string SP_DeleteAttachment = "[dbo].[AttachmentDelete]";
        public static string SP_UpdateAttachmentReferenceId = "[dbo].[AttachmentUpdateReferenceId]";
        public static string SP_GetAttachmentByReferenceId = "[dbo].[AttachmentGetByReferenceId]";
    }
}
