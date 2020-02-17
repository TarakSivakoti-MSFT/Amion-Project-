using AMiON.Helper;
using AMiON.Helper.AuthToken;
using AMiON.Helper.ConfigurationManagerHelper;
using AMiON.Helper.CosmosDB;
using AMiON.Helper.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace AMiONGraphShift.Controllers
{
    public class ShiftsController : Controller
    {
        public IEnumerable<string> ExcludedColumnKeys { get; private set; }

        [HttpGet]
        public ActionResult Auth()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index(string selDate, string internalTeamId, string channelId, string userId)
        {
            //internalTeamId = Constant.LocalHostInternalTeamId;
            //channelId = Constant.LocalHostChannelId;
            if (string.IsNullOrWhiteSpace(selDate))
                return View("LoadingShiftDetails");

            ViewBag.ColumnConfig = AppSetting.ColumnNamesConfig.Split(',').Select(x => new { Columnkey = x.Split(':')[0], ColumnValue = x.Split(':')[1] }).ToDictionary(x => x.Columnkey, x => x.ColumnValue);

            //setting default value for local running
            //if (Request.Url.Host.ToLower().Contains("localhost"))
            //{
            //    internalTeamId = Constant.LocalHostInternalTeamId;
            //    channelId = Constant.LocalHostChannelId;
            //    userId = Constant.LoacalHostUserId;
            //}

            //local Varibale 
            var AMiONShiftDetailList = new List<AssignmentDetails>();
            var IsAdmin = true;
            var selectedDate = string.IsNullOrWhiteSpace(selDate) ? DateTime.Now : DateTime.ParseExact(selDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var cacheTimeOut = Convert.ToInt32(AppSetting.CacheTimeOut);

            Dictionary<string, string> GetColumnMapConfig = AppSetting.ColumnNamesConfig.Split(',')
                .Select(x => new { Columnkey = x.Split(':')[0], ColumnValue = x.Split(':')[1] }).ToDictionary(x => x.Columnkey, x => x.ColumnValue);

            string[] defaultOrderedColumn = GetColumnMapConfig.Select(x => x.Key).ToArray();
            IEnumerable<string> OrderedColumnkeys = new string[] { };
            OrderedColumnkeys = defaultOrderedColumn;
            //loading tab configuration details from CosmosDB
            //var (adminTabConfigurationDetails, OrderedColumnKeys, ExcludedColumnKeys, GetColumnMapConfig) = GetTabConfigurationDetails(ref IsAdmin, internalTeamId, channelId, userId);

            //Checking exist cache if not, call AMION API call. Cache is only implemented for current date
            var cache = MemoryCache.Default;
            if (true)
            {
                var BaseAdress = string.Format("http://www.amion.com/cgi-bin/ocs?Lo={0}&Rpt=625btabs--&Month={1}&Day={2}&Year={3}&Days=1", userId, selectedDate.Month, selectedDate.Day, selectedDate.Year);

                var stream = RestHelper.GetDataFromUrl(BaseAdress).GetAwaiter().GetResult() ?? throw new Exception();

                //stream data to AMiON shift data table
                DataTable ShiftDetailsDataTable = null;
                using (var _ShiftDetailsDataTable = ConvertStreamDataToDataTable(stream))
                    ShiftDetailsDataTable = _ShiftDetailsDataTable;

                //AMiON Shift data table to list
                AMiONShiftDetailList = ConvertShiftDetailsDataTableToList(ShiftDetailsDataTable);

                //if (selectedDate.Date == DateTime.Now.Date)
                //    cache.Add(new CacheItem(adminTabConfigurationDetails.Login, AMiONShiftDetailList), new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeOut) });
            }
            //else
            //    //loading shift details from cache
            //    AMiONShiftDetailList = cache[adminTabConfigurationDetails.Login] as List<AssignmentDetails>;


            var UpdatedOrderShiftColumnNames = defaultOrderedColumn.Select(x => GetColumnMapConfig[x]).ToArray();
            //var RemovedShiftColumnNames = ExcludedColumnKeys.Select(x => GetColumnMapConfig[x]).ToArray();

            //Remove column division if no value found
            if (AMiONShiftDetailList.Any(x => !string.IsNullOrWhiteSpace(x.Division)) == false)
            {
                UpdatedOrderShiftColumnNames = UpdatedOrderShiftColumnNames.Where(x => x != GetColumnMapConfig["Division"]).ToArray();
                //RemovedShiftColumnNames = RemovedShiftColumnNames.Where(x => x != GetColumnMapConfig["Division"]).ToArray();
                defaultOrderedColumn = defaultOrderedColumn.Where(x => x != "Division").ToArray();
            }

            ViewBag.EditColumns = JsonConvert.SerializeObject(new { UpdatedOrderShiftColumnNames = UpdatedOrderShiftColumnNames, /*RemovedShiftColumnNames = RemovedShiftColumnNames,*/ IsAdmin = false });
            ViewBag.orderedColumns = defaultOrderedColumn;
            ViewBag.ShowCharLength = Convert.ToInt32(AppSetting.ShowCharacterLength);
            var departmentist = AMiONShiftDetailList.Where(x => !string.IsNullOrWhiteSpace(x.Division)).Select(x => x.Division).Distinct();
            ViewBag.DepartmentList = departmentist?.OrderBy(x => x).ToArray() ?? new string[] { };
            ViewBag.DepartmentListCount = departmentist.Count();

            return View(AMiONShiftDetailList);
        }


        /// <summary>
        /// Get Token From Cache If Expire Generate New Token
        /// </summary>
        /// <returns></returns>
        private static string GetTokenFromCacheIfExpireGenerateNewToken()
        {
            var cache = MemoryCache.Default;

            if (cache.Contains("AuthToken") && !string.IsNullOrWhiteSpace(cache["AuthToken"] as string))
                return cache["AuthToken"].ToString();

            var token = new AuthToken().GetToken();
            var jwtToken = new JwtSecurityToken(token);
            var expireDateTime = jwtToken.ValidTo.ToLocalTime();
            cache.Add(new CacheItem("AuthToken", token), new CacheItemPolicy() { AbsoluteExpiration = expireDateTime.AddMinutes(-5) });
            return token;
        }

        [HttpPost]
        public ActionResult SaveEditColumns(EditColumnsModel editColumnModel, bool isAdmin, string internalTeamId, string channelId, string userId, string tenantId)
        {
            if (editColumnModel == null)
                throw new NullReferenceException();
            Dictionary<string, string> GetColumnMapConfig = AppSetting.ColumnNamesConfig.Split(',').Select(x => new { Columnkey = x.Split(':')[0], ColumnValue = x.Split(':')[1] }).ToDictionary(x => x.Columnkey, x => x.ColumnValue);
            if (Request.Url.Host.ToLower().Contains("localhost"))
            {
                internalTeamId = Constant.LocalHostInternalTeamId;
                channelId = Constant.LocalHostChannelId;
                userId = Constant.LoacalHostUserId;
                tenantId = Constant.LoacalHostTenantId;
            }

            //update or insert admin setting
            var adminTabConfigurationDetails = new AMiONTabSetting().GetAdminTabConfigurationDetails(internalTeamId, channelId);
            if (adminTabConfigurationDetails is AdminTabConfigurationDetails)
            {
                adminTabConfigurationDetails.DefaultColumnOrder = editColumnModel.OrderedColumns == null ? null : editColumnModel.OrderedColumns.Select(x => GetColumnMapConfig.Where(y => y.Value == x).First().Key).ToArray();
                adminTabConfigurationDetails.LastModifiedBy = Guid.Parse(userId);
                adminTabConfigurationDetails.LastModifiedDate = DateTime.Now;
                new AMiONTabSetting().InsertOrUpdateAdminTabConfigurationDetailCollection(adminTabConfigurationDetails);
            }

            //update or insert user setting
            var userSetting = new AMiONTabSetting().GetUserSetting(userId, internalTeamId);
            if (userSetting is UserSetting)
            {
                userSetting.ColumnsOrderByKey = editColumnModel.OrderedColumns == null ? null : editColumnModel.OrderedColumns.Select(x => GetColumnMapConfig.Where(y => y.Value == x).First().Key).ToArray();
                userSetting.LastModifiedDate = DateTime.Now;
                new AMiONTabSetting().InsertOrUpdateUserSetting(userSetting);
            }
            else
            {
                userSetting = new UserSetting();
                userSetting.ColumnsOrderByKey = editColumnModel.OrderedColumns == null ? null : editColumnModel.OrderedColumns.Select(x => GetColumnMapConfig.Where(y => y.Value == x).First().Key).ToArray();
                userSetting.CreationDate = DateTime.Now;
                userSetting.UserId = Guid.Parse(userId);
                userSetting.InternalTeamId = internalTeamId;
                userSetting.ChannelId = channelId;
                userSetting.TenantId = Guid.Parse(tenantId);
                new AMiONTabSetting().InsertOrUpdateUserSetting(userSetting);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Convert Shift Details DataTable To List
        /// </summary>
        /// <param name="ShiftDetailsDataTable"></param>
        /// <returns></returns>
        private List<AssignmentDetails> ConvertShiftDetailsDataTableToList(DataTable ShiftDetailsDataTable)
        {
            var AMiONShiftDetailList = new List<AssignmentDetails>();
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\.\-]+)((\.(\w){2,3})+)$");

            string GetColumnValueIfExist(DataRow rec, string columnName) => rec.Table.Columns.Contains(columnName) ? rec.Field<string>(columnName) ?? string.Empty : string.Empty;
            string GetAMiONTimeFormat(string timeString) => string.IsNullOrWhiteSpace(timeString) ? string.Empty :
                 DateTime.ParseExact(timeString.PadLeft(4, '0'), "HHmm", CultureInfo.InvariantCulture).ToString("h:mt").ToLower().Replace(":0", "");
            string GetEmailidIfValid(string emailID) => regex.Match(emailID ?? string.Empty).Success ? emailID : string.Empty;

            AMiONShiftDetailList.AddRange(ShiftDetailsDataTable.AsEnumerable().Select(rec => new AssignmentDetails()
            {
                Division = GetColumnValueIfExist(rec, "Division").Replace("�", "").Trim(),
                Role = GetColumnValueIfExist(rec, "Shift_Name").Replace("�", "").Trim(),
                Name = GetColumnValueIfExist(rec, "Staff_Name").Replace("�", "").Trim(),
                StaffBackupID = GetColumnValueIfExist(rec, "Staff_Bid").Replace("�", "").Trim(),
                Training = GetColumnValueIfExist(rec, "Staff_Type").Replace("�", "").Trim(),
                ShiftTime = GetAMiONTimeFormat(GetColumnValueIfExist(rec, "Start_Time")) + "-" + GetAMiONTimeFormat(GetColumnValueIfExist(rec, "End_Time")),
                Pager = GetColumnValueIfExist(rec, "Pager").Replace("�", "").Trim(),
                Contact = GetColumnValueIfExist(rec, "Tel").Replace("�", "").Trim(),
                EMailId = GetEmailidIfValid(GetColumnValueIfExist(rec, "Email")).Replace("�", "").Trim(),
                ShiftStart = GetColumnValueIfExist(rec, "Start_Time").PadLeft(4, '0'),
                ShiftEnd = GetColumnValueIfExist(rec, "End_Time").PadLeft(4, '0')
            }));

            return AMiONShiftDetailList;
        }


        /// <summary>
        /// Convert Stream Data To DataTable
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static DataTable ConvertStreamDataToDataTable(Stream stream)
        {
            var datatable = new DataTable();
            var isHeader = true;
            using (StreamReader sr = new StreamReader(stream))
            {
                string Row = String.Empty;
                while ((Row = sr.ReadLine()) != null)
                {
                    if (isHeader)
                    {
                        datatable.Columns.AddRange(columns: Row.Split('\t').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new DataColumn(x, typeof(string))).ToArray());
                        isHeader = false;
                        continue;
                    }
                    datatable.Rows.Add(Row.Split('\t'));
                }
            }

            return datatable;
        }


        /// <summary>
        /// Get Tab Configuration Details
        /// </summary>
        /// <param name="IsAdmin"></param>
        /// <param name="internalTeamId"></param>
        /// <param name="channelId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private (AdminTabConfigurationDetails adminTabConfigurationDetails,
            IEnumerable<string> OrderedColumnkeys,
            string[] ExcludedColumnkeys,
            Dictionary<string, string> GetColumnMapConfig)
            GetTabConfigurationDetails(
            ref bool IsAdmin,
            string internalTeamId,
            string channelId,
            string userId)
        {
            Dictionary<string, string> GetColumnMapConfig = AppSetting.ColumnNamesConfig.Split(',')
                .Select(x => new { Columnkey = x.Split(':')[0], ColumnValue = x.Split(':')[1] }).ToDictionary(x => x.Columnkey, x => x.ColumnValue);

            string[] defaultOrderedColumn = GetColumnMapConfig.Select(x => x.Key).ToArray();
            IEnumerable<string> OrderedColumnkeys;
            OrderedColumnkeys = new string[] { };
            var ExcludedColumnkeys = new string[] { };

            var adminTabConfigurationDetails = new AMiONTabSetting().GetAdminTabConfigurationDetails(internalTeamId, channelId);
            if (adminTabConfigurationDetails is AdminTabConfigurationDetails)
                OrderedColumnkeys = adminTabConfigurationDetails.DefaultColumnOrder ?? new string[] { };

            var userSetting = new AMiONTabSetting().GetUserSetting(userId, internalTeamId);
            if (userSetting is UserSetting)
            {
                if (userSetting.ColumnsOrderByKey != null && userSetting.ColumnsOrderByKey.Length > 0)
                {
                    OrderedColumnkeys = (userSetting.ColumnsOrderByKey != null && userSetting.ColumnsOrderByKey.Length > 0 ? ((userSetting.ColumnsOrderByKey.Intersect(OrderedColumnkeys)).Union(OrderedColumnkeys)) : OrderedColumnkeys).ToArray(); ;
                }
            }

            return (adminTabConfigurationDetails, OrderedColumnkeys, ExcludedColumnkeys, GetColumnMapConfig);
        }


        [HttpGet]
        public JsonResult GetChatWindowUrl(string internalTeamId, string channelId, string departmentName, string staffBackupId, string userId)
        {
            try
            {
                var login = string.Empty;
                //setting default value for local running
                if (Request.Url.Host.ToLower().Contains("localhost"))
                {
                    internalTeamId = Constant.LocalHostInternalTeamId;
                    channelId = Constant.LocalHostChannelId;
                }

                string GetColumnValueIfExist(DataRow rec, string columnName) => rec.Table.Columns.Contains(columnName) ? rec.Field<string>(columnName) ?? string.Empty : string.Empty;
                var adminTabConfigurationDetails = new AMiONTabSetting().GetAdminTabConfigurationDetails(internalTeamId, channelId);

                if (!string.IsNullOrWhiteSpace(departmentName))
                {
                    var BaseAdress = $"http://www.amion.com/cgi-bin/ocs?Lo={adminTabConfigurationDetails.Login}&Rpt=31qtabs--";

                    var stream = RestHelper.GetDataFromUrl(BaseAdress).GetAwaiter().GetResult() ?? throw new Exception();

                    DataTable departmentPwdDetailsDataTable = null;
                    using (var _departmentPwdDetailsDataTable = ConvertStreamDataToDataTable(stream))
                        departmentPwdDetailsDataTable = _departmentPwdDetailsDataTable;

                    login = departmentPwdDetailsDataTable.AsEnumerable().Where(x => GetColumnValueIfExist(x, "GroupName") == departmentName)
                                                           .Select(x => GetColumnValueIfExist(x, "PostedYr2")).SingleOrDefault();
                    if (string.IsNullOrWhiteSpace(login))
                        throw new Exception();
                }
                else
                    login = adminTabConfigurationDetails.Login;

                var chatURl = $"https://www.amion.com/cgi-bin/ocs?Lo={login}&Page=Alphapg&Rsel={staffBackupId}&secure";
                return Json(chatURl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
