using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public class SchedulingGroupHelper
    {
        public SchedulingGroup schedulingGroup;

        /// <summary>
        /// Method to create a new scheduling group
        /// </summary>
        /// <param name="teamID">Team id of the account</param>
        /// <param name="schedulingGroup">Scheduling group object which needs to be created</param>
        /// <param name="token">Token object to be passed for authentication header</param>
        /// <returns></returns>
        public static (bool isScheduleCreateSuccess, string responseData) CreateSchedulingGroups(string teamID,
                                                                                          JObject schedulingGroup,
                                                                                          string token)
        {
            if (teamID == null || schedulingGroup == null || token == null)
            {
                return (false, "Parameter is empty");
            }

            string responseData = string.Empty;
            StringContent _schedulingGroup = null;

            using (var schedulingGroupStrContent = schedulingGroup.AsJson())
                _schedulingGroup = schedulingGroupStrContent;

            var response = RestHelper.PostData(string.Format("{0}/beta/teams/{1}/schedule/schedulingGroups", Constant.GraphUrl, teamID), token, _schedulingGroup);

            if (response != null && response.Content != null)
            {
                responseData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else
            {
                responseData = "Response is empty";
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return (true, responseData);
            }

            return (false, responseData);
        }
        /// <summary>
        /// Method to create a dynamic SchedulingGroupObject instead of using a custom class
        /// </summary>
        /// <param name="displayName">Name of the scheduling group that needs to be added</param>
        /// <param name="userID">UserId to which the scheduling group that needs to be added</param>
        /// <returns></returns>
        private static JObject CreateDynamicSchedulingGroupObject(string displayName, List<Guid> userID)
        {
            dynamic schedulingGroup = new JObject();
            schedulingGroup.displayName = displayName;
            schedulingGroup.isActive = true;
            schedulingGroup.userIds = new JArray(userID.ToArray());
            return schedulingGroup;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="teamID"></param>
        /// <param name="schedulingGroupNameTobeChecked"></param>
        /// <returns></returns>
        public static SchedulingGroup GetSchedulingGroupsFromTeamIdAndCheckGroupExist(string token, string teamID, string schedulingGroupNameTobeChecked)
        {
            (bool isGetSchedulingGroupApiCallSuccess, string schedulingGroupResponseData) = DataProvider.GetDataFromInputUrl(token, string.Format("/beta/teams/{0}/schedule/schedulingGroups", teamID));
            if (isGetSchedulingGroupApiCallSuccess)
            {
                if ((string.IsNullOrEmpty(schedulingGroupResponseData) && (schedulingGroupResponseData.Contains("error"))))
                {
                    //TODO: what needs to be done when there is an error
                }
                else
                {
                    return schedulingGroupResponseData.AsObject<SchedulingGroups>().SchedulingGroup.FirstOrDefault(sGroup => sGroup.DisplayName == schedulingGroupNameTobeChecked);
                    //To fetch the scheduling group id

                }
            }
            else
            {
                //TODO: Call failed with network error or some other reason
            }
            return null;
        }

        public static string MapAndCreateSchedulingGrpinTeams(string token, string teamID, string displayName, List<Guid> userIDs)
        {
            if (userIDs == null)
            {
                throw new ArgumentNullException(nameof(userIDs));
            }

            var (isCreationSuccess, schedulingGrpResData) = SchedulingGroupHelper.CreateSchedulingGroups(
                                                                                            teamID,
                                                                                            CreateDynamicSchedulingGroupObject(displayName, userIDs),
                                                                                            token);
            if (isCreationSuccess)
            {
                //Check for any error in response, if not parse the data and fetch the user ID and SchedulingGroup id
                if (!string.IsNullOrEmpty(schedulingGrpResData) && !schedulingGrpResData.Contains("error"))
                    return schedulingGrpResData.ParseJson()["id"].ToString();
            }
            else
            {
                //TODO:Shift creation failed or errored out with some reason
            }
            return "";
        }
    }
}
