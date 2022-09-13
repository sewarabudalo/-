using System.Web;

namespace Malaffi.Models.Test
{
    public class UploadTestModel
    {
        public int Id { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}