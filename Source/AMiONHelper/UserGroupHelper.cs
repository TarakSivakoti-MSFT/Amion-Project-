using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public class UserGroupHelper
    {
        public static (bool isGetUserGrpApiSuccess, string userGrpResData) GetMembersBasedonTeamIdFromGraphApi(string teamID, string token)
        {
            string strResponseData = string.Empty;
            if (teamID == null || token == null)
            {
                throw new ArgumentException();
            }

            using (var userGrpResponseData = RestHelper.GetDataFromUrl(string.Format(@"{0}/v1.0/groups/{1}/members", Constant.GraphUrl, teamID), token))
            {
                if (userGrpResponseData.Result != null)
                {
                    using (StreamReader sr = new StreamReader(userGrpResponseData.GetAwaiter().GetResult()))
                    {
                        strResponseData = sr.ReadToEnd();
                        return (!string.IsNullOrEmpty(strResponseData)
                            && !strResponseData.Contains("error")) ? (true, strResponseData) : (false, strResponseData);
                    }
                }
                else
                {
                    return (false, "error: something went wrong, network error or code error");
                    //TODO: Log response result is empty
                }
            }
        }
    }
}
