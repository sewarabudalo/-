using System;
using System.IO;

namespace Malaffi.Storage
{
    public class FileUploader
    {
        public static string UploadFile(string fileName, System.IO.Stream stream, string contentType, AttachmentTypes type)
        {
            try
            {
                var fileId = Guid.NewGuid();
                string path = AppDomain.CurrentDomain.BaseDirectory + "Documents\\" + type + "\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = fileId + Path.GetExtension(fileName); ;

                using (var fileStream = File.Create(Path.Combine(path, filename)))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
                return filename;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeletePicture(string fileName, AttachmentTypes type)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Documents\\" + type + "\\";
                string FilePath = Path.Combine(path, fileName);
                System.IO.File.Delete(FilePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
