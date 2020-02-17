using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace AMiON.Helper
{
    public class Text2Json
    {
        public static List<AssignmentDetails> JsonDataFromWebExcel(string amionLogin)
        {
            List<AssignmentDetails> lstAssignmentDetails = null;
            using (StreamReader sr = new StreamReader(AmiOnDataProvider.GetAmionData(amionLogin)))
            {
                if (sr != null)
                {
                    lstAssignmentDetails = new List<AssignmentDetails>();
                    string Row = String.Empty;
                    int count = 0;
                    while ((Row = sr.ReadLine()) != null)
                    {
                        string[] column = Regex.Split(Row, "\t");
                        if (count == 0)
                        {
                            if (column[0].Contains("Bad password")) 
                            {
                                break;
                            }
                            else if(string.Equals(column[0], "division", StringComparison.OrdinalIgnoreCase))
                            {
                                count++;
                                continue;
                            }
                            else{
                                //Handle case when division is not there
                            }

                        }

                        List<DateTime> dateTimes = CalculateDateAndTime(column[7], column[8], column[9]);

                        lstAssignmentDetails.Add(new AssignmentDetails
                        {
                            Division = column[0],
                            staffName = column[1],
                            staffId = new StaffId
                            {
                                id = Int32.Parse(column[2]),
                                backupId = Int32.Parse(column[3])
                            },
                            assignmentName = column[4],
                            assignmentId = new AssignmentId
                            {
                                id = Int32.Parse(column[5]),
                                backupId = Int32.Parse(column[6])
                            },
                            startDateTime = dateTimes[0],
                            endDateTime = dateTimes[1]
                        });
                    }
                }
            }
            return lstAssignmentDetails;
        }
       
        static List<DateTime> CalculateDateAndTime(string date, string start, string end)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            string[] pattern = { "dd/M/yy h:m", "dd/MM/yy h:m", "dd/M/yyyy h:m", "dd/M/yyyy H:m", "dd/M/yyyy HH:m", "dd/M/yy H:m", "dd/M/yy HH:m", "d/M/yy h:m", "d/M/yyyy h:m", "d/M/yy H:m", "d/M/yyyy H:m" };
            DateTime startDate;
            DateTime endDate;
            date = date.Contains("-") ? date.Replace("-", "/") : date;
            if (DateTime.TryParseExact(string.Format("{0} {1}", date, start.Insert(start.Length / 2, ":")), pattern, null, DateTimeStyles.None, out startDate))
            {
                dateTimes.Add(startDate);
            }
            if (DateTime.TryParseExact(string.Format("{0} {1}", date, end.Insert(start.Length / 2, ":")), pattern, null, DateTimeStyles.None, out endDate))
            {
                dateTimes.Add(endDate < startDate ? endDate.AddDays(1) : endDate);
            }
            return dateTimes;
        }

        #region commented
        //static List<DateTime> CalculateDateAndTime(string date, string start, string end)
        //{
        //    List<DateTime> dateTimes = new List<DateTime>();

        //    DateTime startDate = DateTime.Parse(string.Format("{0} {1}", date.Replace("-", "/"), start.Insert(start.Length / 2, ":")));
        //    DateTime endDate = DateTime.Parse(string.Format("{0} {1}", date.Replace("-", "/"), end.Insert(end.Length / 2, ":")));
        //    dateTimes.Add(startDate);
        //    dateTimes.Add(endDate);

        //    return dateTimes;
        //}
        #endregion

    }
}
