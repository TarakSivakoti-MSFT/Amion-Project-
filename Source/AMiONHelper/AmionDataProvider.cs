using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AMiON.Helper
{
    public class AmiOnDataProvider
    {
        //private static string _amiOnUrl = @"http://www.amion.com/cgi-bin/ocs?Lo={0}&Rpt=625tabs--&Day={1}&Month={2}&Year=2019";
        private static string _amiOnUrl = @"http://www.amion.com/cgi-bin/ocs?Lo={0}&Rpt=625btabs--&Month={1}&Day={2}&Year={3}&Days={4}";
        
        public static Stream GetAmionData(string amionLogin)
        {
            string amiOnUrl = string.Format(_amiOnUrl,
                                            amionLogin,
                                            DateTime.Now.Month,
                                            DateTime.Now.Day,
                                            DateTime.Now.Year,
                                            System.Configuration.ConfigurationManager.AppSettings["DaysCount"].ToString());
            return RestHelper.GetDataFromUrl(amiOnUrl) == null ? null : RestHelper.GetDataFromUrl(amiOnUrl).GetAwaiter().GetResult();
        }

        public static (bool isAmionLoginSucess, string responseMessage) CheckAmionAuthentication(string amionLogin)
        {
            using (StreamReader sr = new StreamReader(AmiOnDataProvider.GetAmionData(amionLogin)))
            {
                if (sr != null)
                {
                    var data = sr.ReadToEnd();
                    if (!string.IsNullOrEmpty(data))
                    {
                        return data.Contains("NOFI=Bad password") ? (false, data) : (true, "Authentication sucess");
                    }
                    
                }
                else
                {
                    return (false, "No data received for given AMiON input login.");
                }
            }
            return (false, "Something went wrong!!! Either network error or no data received for provided input.");
        }
    }
}
