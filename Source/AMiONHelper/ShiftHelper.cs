using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public class ShiftsHelper
    {
        /// <summary>
        /// Get all shifts data based on team or startDate or endDate 
        /// </summary>
        /// <param name="teamID">Microsoft team id having all shifts</param>
        /// <param name="token">user token for authentication</param>
        /// <param name="startDate">start date filter for shifts</param>
        /// <param name="endDate">end date filter for shifts</param>
        /// <returns></returns>
        public static ShiftList GetShifts(string teamID,
                                          string token,
                                          string startDate = "",
                                          string endDate = "")
        {
            if (teamID == null || token == null)
            {
                throw new ArgumentException();
            }
            string getShiftUrl = string.Empty;

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                getShiftUrl = string.Format(@"/beta/teams/{0}/schedule/shifts?$filter=sharedShift/startDateTime ge {1}T00:00:00.000Z and sharedShift/endDateTime le {2}T12:00:00.000Z", teamID, startDate, endDate);
            }
            else if (!string.IsNullOrEmpty(startDate))
            {
                getShiftUrl = string.Format(@"/beta/teams/{0}/schedule/shifts?$filter=sharedShift/startDateTime ge {1}T12:00:00.000Z", teamID, startDate);
            }
            else
            {
                getShiftUrl = string.Format(@"/beta/teams/{0}/schedule/shifts", teamID);
            }

            var responseList = new List<string>();
            var requestList = new List<string>();

            var (isGetShiftCallSuccess, shiftResponseData) = DataProvider.GetDataFromInputUrl(token, getShiftUrl);

            if (isGetShiftCallSuccess)
            {
                if (!string.IsNullOrEmpty(shiftResponseData) && !shiftResponseData.Contains("error"))
                {
                    return shiftResponseData.AsObject<ShiftList>();
                }
                else
                {
                    //TODO:Log get shifts call failed with error response
                }
            }
            return null;
        }
        /// <summary>
        /// Method to delete shift entry in microsoft team
        /// </summary>
        /// <param name="teamID">Microsoft team id</param>
        /// <param name="shiftID">Shift data id</param>
        /// <param name="token">Token for authentication</param>
        /// <returns></returns>
        public static (bool isDeleteApiCallSuccess, string strResponseData) DeleteShift(string teamID,
                                                                                        string shiftID,
                                                                                        string token)
        {
            if (teamID == null || token == null)
            {
                throw new ArgumentException();
            }

            var deleteResponse = RestHelper.AsyncDeleteData(string.Format(@"{0}/beta/teams/{1}/schedule/shifts/{2}", Constant.GraphUrl, teamID, shiftID), token);
            //Checking for status code other then 204 no content, if found return false
            if (deleteResponse.Result != null && deleteResponse.Result.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                return (false, deleteResponse.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            }
            return (true, "");
        }
    }
}
