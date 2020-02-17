using AMiON.Helper;
using AMiON.Helper.AuthToken;
using AMiON.Helper.CosmosDB;
using AMiON.Helper.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AMiON.ShiftsMapping.Controllers
{
    public class AmionController : Controller
    {
        string token = string.Empty;

        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [Route("CheckAmion/")]
        public JsonResult CheckAmionAuthentication(ImportUserInputModel objImportModel)
        {
            if (objImportModel == null || string.IsNullOrEmpty(objImportModel.AmionLogin))
            {
                return Json(new { SuccessResponse = false, Message = "Input data object is empty." });
            }
            var amionLoginResponseData = Helper.AmiOnDataProvider.CheckAmionAuthentication(objImportModel.AmionLogin);

            return (amionLoginResponseData.isAmionLoginSucess) ? Json(new { SuccessResponse = amionLoginResponseData.isAmionLoginSucess, Message = amionLoginResponseData.responseMessage }, JsonRequestBehavior.AllowGet) : Json(new { SuccessResponse = amionLoginResponseData.isAmionLoginSucess, Message = amionLoginResponseData.responseMessage }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Route("AmionData/{amionLogin}")]
        public JsonResult AmionData(string amionLogin)

        {
            var amionData = FetchAmionData(amionLogin);

            if (amionData != null)
            {
                var uniqueDepartments = amionData.Select(department => department.Division).Distinct();
                List<DepartmentModel> lstDepartmentModel = new List<DepartmentModel>();

                foreach (string departmentName in uniqueDepartments)
                {
                    DepartmentModel departmentModel = new DepartmentModel();
                    departmentModel.DepartmentName = departmentName;
                    departmentModel.ShiftsCount = amionData.FindAll(depart => depart.Division == departmentName).Count;
                    lstDepartmentModel.Add(departmentModel);
                }
                return Json(lstDepartmentModel, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        private static List<AssignmentDetails> FetchAmionData(string loginData)
        {
            if (loginData == null)
            {
                throw new ArgumentNullException(nameof(loginData));
            }
            return Utilities.ConvertShiftDetailsDataTableToList((Utilities.ConvertStreamDataToDataTable(AmiOnDataProvider.GetAmionData(loginData))))
                   ?? null;
        }

        [HttpPost]
        [Route("SaveFile")]
        public ActionResult UploadFiles(string internalTeamId, string channelId, string userId, string tenantId)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    //for (int i = 0; i < files.Count; i++)
                    //{
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  

                    HttpPostedFileBase file = Request.Files[0];
                    string fname;

                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }

                    if (Request.Url.Host.ToLower().Contains("localhost"))
                    {
                        internalTeamId = Constant.LocalHostInternalTeamId;
                        channelId = Constant.LocalHostChannelId;
                        userId = Constant.LoacalHostUserId;
                        tenantId = Constant.LoacalHostTenantId;
                    }

#pragma warning disable CA1062 // Validate arguments of public methods
                    string filePath = $"Amion/{tenantId}/{string.Join("_", internalTeamId.Split(System.IO.Path.GetInvalidFileNameChars()))}/{fname}";
#pragma warning restore CA1062 // Validate arguments of public methods
                    new AzureBlobStorage().UploadFileToBlobStorage(Constant.containerName, filePath, file.InputStream);

                    //string path = Server.MapPath("~/Uploads/");

                    //if(!(Path.GetExtension(path+fname) == ".xlsx" || Path.GetExtension(path+fname) == ".xls"))
                    //{
                    //    return Json(new { message = "Unsupported format of mapping file...", success = false });
                    //}

                    //if (!Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}
                    //if (System.IO.File.Exists(path+fname))
                    //{
                    //    // deletevprevious image
                    //    System.IO.File.Delete(path+fname);
                    //}
                    //// Get the complete folder path and store the file inside it.  
                    //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                    //file.SaveAs(fname);
                    //}
                    // Returns message that successfully uploaded  
                    return Json(new { message = "File Uploaded Successfully!", success = true, filePath = filePath });
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    return Json(new { message = "Error occurred.Error details: ", success = false });
                }
            }
            else
            {
                return Json(new { message = "No files selected.", success = false });
            }
        }

        [HttpPost]
        [Route("CreateShifts/")]
        public JsonResult ProcessShifts(ImportUserInputModel importUserInputData, string internalTeamId, string channelId, string userId, string tenantId)
        {
            if (importUserInputData != null)
            {
                // importUserInputData.AccessToken;
                
                string TeamId = "";
                Dictionary<string, string> GetColumnMapConfig = Helper.ConfigurationManagerHelper.AppSetting.ColumnNamesConfig.Split(',').Select(x => new { Columnkey = x.Split(':')[0], ColumnValue = x.Split(':')[1] }).ToDictionary(x => x.Columnkey, x => x.ColumnValue);
                if (Request.Url.Host.ToLower().Contains("localhost"))
                {
                    internalTeamId = Constant.LocalHostInternalTeamId;
                    channelId = Constant.LocalHostChannelId;
                    userId = Constant.LoacalHostUserId;
                    tenantId = Constant.LoacalHostTenantId;
                    TeamId = Constant.LoacalHostTeamId;
                   // token = Helper.Authentication.GetToken();
                }

                var adminTabConfigurationDetails = new AMiONTabSetting().GetAdminTabConfigurationDetails(internalTeamId, channelId);
                if (adminTabConfigurationDetails is AdminTabConfigurationDetails)
                {
                    adminTabConfigurationDetails.Login = importUserInputData.AmionLogin;
                    adminTabConfigurationDetails.DefaultColumnOrder = importUserInputData.SelectedColumnNames !=null ? importUserInputData.SelectedColumnNames.Select(x => GetColumnMapConfig.Where(y => y.Value == x).First().Key).ToArray() : null;
                    adminTabConfigurationDetails.ExcludedColumns = importUserInputData.RemovedColumnNames != null ? importUserInputData.RemovedColumnNames.Select(x => GetColumnMapConfig.Where(y => y.Value == x).First().Key).ToArray() : null;
                    //adminTabConfigurationDetails.SelectedDepartments = importUserInputData.SelectedDepartments?.ToArray();
                    //adminTabConfigurationDetails.MappingFilePath = importUserInputData.MappingFilePath;
                    adminTabConfigurationDetails.LastModifiedBy = Guid.Parse(userId);
                    adminTabConfigurationDetails.LastModifiedDate = DateTime.Now;
                    adminTabConfigurationDetails.InternalTeamId = internalTeamId;
                    adminTabConfigurationDetails.ChannelId = channelId;
                    new AMiONTabSetting().InsertOrUpdateAdminTabConfigurationDetailCollection(adminTabConfigurationDetails);
                }
                else
                {
                    adminTabConfigurationDetails = new AdminTabConfigurationDetails();
                    adminTabConfigurationDetails.Login = importUserInputData.AmionLogin;
                    adminTabConfigurationDetails.DefaultColumnOrder = importUserInputData.SelectedColumnNames != null ? importUserInputData.SelectedColumnNames.Select(x => GetColumnMapConfig.Where(y => y.Value == x).First().Key).ToArray() : null;
                    adminTabConfigurationDetails.ExcludedColumns = importUserInputData.RemovedColumnNames != null ? importUserInputData.RemovedColumnNames.Select(x => GetColumnMapConfig.Where(y => y.Value == x).First().Key).ToArray() : null;
                    adminTabConfigurationDetails.TenantId = Guid.Parse(tenantId);
                    //adminTabConfigurationDetails.SelectedDepartments = importUserInputData.SelectedDepartments?.ToArray();
                    //adminTabConfigurationDetails.MappingFilePath = importUserInputData.MappingFilePath;
                    adminTabConfigurationDetails.CreationDate = DateTime.Now;
                    // adminTabConfigurationDetails.TeamId = Guid.Parse(TeamId);
                    adminTabConfigurationDetails.InternalTeamId = internalTeamId;
                    adminTabConfigurationDetails.ChannelId = channelId;
                    new AMiONTabSetting().InsertOrUpdateAdminTabConfigurationDetailCollection(adminTabConfigurationDetails);
                }

                // var token = Helper.Authentication.GetToken();
                //  var workingThread = new Thread(() => OperationToCallAsync(token, importUserInputData));
                //  workingThread.Start();

                return Json(new { success = true });
            }
            return Json(new { errorMessage = "No input data passed", errorCode = "0", success = false });
        }

        void OperationToCallAsync(string token, ImportUserInputModel importUserInputData)
        {
            importUserInputData.StartDate = DateTime.Now;
            importUserInputData.EndDate = DateTime.Now.AddDays(1);
            ShiftsProcessor.ShiftCreationProcessor(token, importUserInputData);
        }


    }
}