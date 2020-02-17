using AMiON.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMiONGraphShift.Controllers
{
    public class AmionSubscribeController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            string folderRootPath = string.Empty;// @"D:\home\site\wwwroot\Subscribe";
            string content = string.Empty;

            using (MemoryStream copiedStreamData = new MemoryStream())
            {
                Request.InputStream.CopyTo(copiedStreamData);
                copiedStreamData.Position = 0;
                Request.InputStream.Position = 0;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    content = reader.ReadToEnd();
                    var VACC = content.Split(Environment.NewLine.ToCharArray()).Where(x => x.Contains("VACC")).Select(x => x.Split('=')[1]).FirstOrDefault() ?? "VACC";
                    VACC = string.Join("_", VACC.Split(System.IO.Path.GetInvalidFileNameChars()));
                    var excelFilePath = Path.Combine(DateTime.Now.ToString("ddMMMyyyy"), VACC, "NewSchedule_" + DateTime.Now.ToString("HHmmss") + ".xls");
                    new AzureBlobStorage().UploadFileToBlobStorage("amionsubscribe", excelFilePath, copiedStreamData);
                }
            }
            return Content("success");
        }

        [HttpGet]
        public ActionResult Index(string str = "")
        {
            string path = @"D:\home\site\wwwroot";
            var content = System.IO.File.ReadAllText(Path.Combine(path, "Subscribe.txt"));
            return Content(content);
        }


    }
}