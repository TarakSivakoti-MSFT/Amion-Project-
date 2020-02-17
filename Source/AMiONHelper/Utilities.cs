using ExcelDataReader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public class Utilities
    {
        public static DataRow CheckValueExistInDatatableBasedonColumnNumber(int columnNumber, string valueWhichNeedsCheckedinColumn, DataTable dt) => dt.AsEnumerable().Where(c => c.Field<string>(columnNumber) == valueWhichNeedsCheckedinColumn)?.First();

        public static string GetColumnValueIfExist(DataRow record, string columnName)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return record.Table.Columns.Contains(columnName) ? record.Field<string>(columnName) ?? string.Empty : string.Empty;
        }

        //IEnumerable<T> JoinedTeamsResponseParse<T>(JToken jToken, List<TeamList> teamList)
        //{
        //    //var result = JsonConvert.DeserializeObject<IEnumerable<T>>(jToken["body"]["value"].ToString(), Converter.Settings);
        //    //var teamID = jToken["id"].ToString().Split('#')[1];
        //    //foreach (dynamic item in result)
        //    //    item.TeamID = teamID;
        //    //teamList.Where(team => team..ToString() == teamID).First().nextLink = jToken["body"]["@odata.nextLink"] == null ? null : jToken["body"]["@odata.nextLink"].ToString();


        //    //return result;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataTable ConvertStreamDataToDataTable(Stream stream)
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

        public static List<AssignmentDetails> ConvertShiftDetailsDataTableToList(DataTable ShiftDetailsDataTable)
        {
            var AMiONShiftDetailList = new List<AssignmentDetails>();
            string[] pattern = { "M/dd/yy h:m", "MM/dd/yy h:m", "M/dd/yyyy h:m", "M/dd/yyyy H:m", "M/dd/yyyy HH:m", "M/dd/yy H:m", "M/dd/yy HH:m", "M/d/yy h:m", "M/d/yyyy h:m", "M/d/yy H:m", "M/d/yyyy H:m" };
            string GetColumnValueIfExist(DataRow rec, string columnName) => rec.Table.Columns.Contains(columnName) ? rec.Field<string>(columnName) ?? string.Empty : string.Empty;
            string GetAMiONTimeFormat(string timeString) => string.IsNullOrWhiteSpace(timeString) ? string.Empty :
                 DateTime.ParseExact(timeString.PadLeft(4, '0'), "HHmm", CultureInfo.InvariantCulture).ToString("h:mtt").Replace(":0", string.Empty).ToLower();
            DateTime GetAMiONDateTimeFormat(string date, string timeString) => string.IsNullOrWhiteSpace(date) && string.IsNullOrWhiteSpace(timeString) ? DateTime.MinValue :
                 DateTime.ParseExact((date + " " + timeString.Insert(timeString.Length / 2, ":")).Contains("-") ? (date + " " + timeString.Insert(timeString.Length / 2, ":")).Replace("-","/"): date + " " + timeString.Insert(timeString.Length / 2, ":"), pattern, null, DateTimeStyles.None);

            AMiONShiftDetailList.AddRange(ShiftDetailsDataTable.AsEnumerable().Select(rec => new AssignmentDetails()
            {
                Division = GetColumnValueIfExist(rec, "Division"),
                Role = GetColumnValueIfExist(rec, "Shift_Name"),
                Name = GetColumnValueIfExist(rec, "Staff_Name"),
                Training = GetColumnValueIfExist(rec, "Staff_Type"),
                ShiftTime = GetAMiONTimeFormat(GetColumnValueIfExist(rec, "Start_Time")) + "-" + GetAMiONTimeFormat(GetColumnValueIfExist(rec, "End_Time")),
                Pager = GetColumnValueIfExist(rec, "Pager"),
                Contact = GetColumnValueIfExist(rec, "Tel"),
                EMailId = GetColumnValueIfExist(rec, "Email"),
                ShiftStart = GetColumnValueIfExist(rec, "Start_Time").PadLeft(4, '0'),
                ShiftEnd = GetColumnValueIfExist(rec, "End_Time").PadLeft(4, '0'),
                startDateTime = GetAMiONDateTimeFormat(GetColumnValueIfExist(rec, "Date"),GetColumnValueIfExist(rec, "Start_Time")),
                endDateTime= GetAMiONDateTimeFormat(GetColumnValueIfExist(rec, "Date"), GetColumnValueIfExist(rec, "End_Time"))< GetAMiONDateTimeFormat(GetColumnValueIfExist(rec, "Date"), GetColumnValueIfExist(rec, "Start_Time")) ? GetAMiONDateTimeFormat(GetColumnValueIfExist(rec, "Date"), GetColumnValueIfExist(rec, "End_Time")).AddDays(1): GetAMiONDateTimeFormat(GetColumnValueIfExist(rec, "Date"), GetColumnValueIfExist(rec, "End_Time")),

            }));
            return AMiONShiftDetailList;
        }

        /// <summary>
        /// Method to convert stream object to datatable
        /// </summary>
        /// <param name="stream">Input stream object</param>
        /// <returns>Returns a data table</returns>
        public static DataTable ConvertExcelStreamDataToDataTable(Stream stream)
        {
            //checking input object is null
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var ShiftDetailsDataTable = new DataTable())
            {
                try
                {
                    // We return the interface, so that
                    IExcelDataReader reader = null;

                    //Creating instance of ExcelDataReader
                    reader = ExcelReaderFactory.CreateReader(stream);

                    //Setting the first row as header
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };
                    DataSet result = reader.AsDataSet(conf);
                    reader.Close();
                    if (result != null)
                    {
                        return result.Tables[0];
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return null;
        }

        /// <summary>
        /// Method which takes data table as object and converts to List MappingModel object
        /// </summary>
        /// <param name="mappingModelDataTable"></param>
        /// <returns>List of Mapping model object</returns>
        public static List<MappingModel> ConvertMappingDataTableToListMappingModel(DataTable mappingModelDataTable)
        {
            if (mappingModelDataTable == null)
            {
                throw new ArgumentNullException(nameof(mappingModelDataTable));
            }

            var lstAmionMappingModel = new List<MappingModel>();
            // trim column names
            foreach (DataColumn dc in mappingModelDataTable.Columns) 
            {
                dc.ColumnName = dc.ColumnName.Trim();
            }
            //Reading the data table and mapping to model object
            lstAmionMappingModel.AddRange(mappingModelDataTable.AsEnumerable().Select(rec => new MappingModel()
            {
                AMiONDivision = GetColumnValueIfExist(rec, "AMiONDivision"),
                TeamsTeam = GetColumnValueIfExist(rec, "TeamsTeam"),
                AMiONAssignment = GetColumnValueIfExist(rec, "AMiONAssignment"),
                ShiftGroup_Role = GetColumnValueIfExist(rec, "ShiftGroup_Role"),
                ShouldImport = GetColumnValueIfExist(rec, "Import")
            }));
            return lstAmionMappingModel;
        }
    }
}
