using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public static class ShiftsProcessor
    {
        public static int ShiftCreationProcessor(string token, ImportUserInputModel objAMiONShiftMappingModel, List<MappingModel> lstMappingFileModel = null, DataList<Entities.Logger> lstLogger = null)
        {
            if (objAMiONShiftMappingModel == null)
            {
                throw new ArgumentNullException(nameof(objAMiONShiftMappingModel));
            }

            if (lstLogger == null)
            {
                lstLogger = new DataList<Entities.Logger>();
            }

            List<Team> teamsWhichNeedsTobeProcessedBasedOnSelectedDepartment = null;

            //Get data from AmiOn in Json
            List<AssignmentDetails> _assignments = Text2Json.JsonDataFromWebExcel(objAMiONShiftMappingModel.AmionLogin);

            if (string.IsNullOrEmpty(token))
            {
                //Get auth token
                token = Authentication.GetToken();
            }

            //Get teams data from Microsoft 0365
            (bool isGetTeamsApiCallSuccess, string teamsResponseData) = DataProvider.GetDataFromInputUrl(token, "/v1.0/me/joinedTeams");

            if (isGetTeamsApiCallSuccess)
            {
                var objTeams = teamsResponseData.AsObject<TeamList>();

                //MyTask:count code needs to be removed 
                int count = 0;

                if (objTeams.Teams != null && objTeams.Teams.Length > 0)
                {
                    if (objAMiONShiftMappingModel.SelectedDepartments != null && objAMiONShiftMappingModel.SelectedDepartments.Count > 0)
                    {
                        //Teams which needs to be processed based on user selected values
                        teamsWhichNeedsTobeProcessedBasedOnSelectedDepartment = new List<Team>();
                        foreach (string userSelectedTeam in objAMiONShiftMappingModel.SelectedDepartments)
                        {
                            var teamName = userSelectedTeam;
                            //fetch the team name from the mapping file
                            if (lstMappingFileModel != null && lstMappingFileModel.Count > 0 && lstMappingFileModel.Any(mappingModel => (!string.IsNullOrEmpty(mappingModel.AMiONDivision)) && string.Equals(mappingModel.AMiONDivision.Trim(), teamName, StringComparison.OrdinalIgnoreCase)))
                            {
                                teamName = lstMappingFileModel.Find(mappingModel => (!string.IsNullOrEmpty(mappingModel.AMiONDivision)) && string.Equals(mappingModel.AMiONDivision.Trim(), userSelectedTeam, StringComparison.OrdinalIgnoreCase)).AMiONDivision.Trim();
                            }

                            if (objTeams.Teams.Any(team => string.Equals(team.DisplayName, teamName, StringComparison.OrdinalIgnoreCase)))
                            {
                                //Adding team names
                                teamsWhichNeedsTobeProcessedBasedOnSelectedDepartment.Add(objTeams.Teams.Where(team => string.Equals(team.DisplayName, userSelectedTeam, StringComparison.OrdinalIgnoreCase)).First());
                            }
                        }
                        if (teamsWhichNeedsTobeProcessedBasedOnSelectedDepartment != null && teamsWhichNeedsTobeProcessedBasedOnSelectedDepartment.Count > 0)
                        {
                            //RnD:Added the logic here so that we can optimize using paralled.foreach
                            //Team level processing
                            foreach (var team in teamsWhichNeedsTobeProcessedBasedOnSelectedDepartment)
                            {
                                //Amion data based on microsoft team name 
                                var amionDataTobeProcessedBasedOnTeam = _assignments.FindAll(assignment => string.Equals(assignment.Division, team.DisplayName, StringComparison.OrdinalIgnoreCase));
                                if (amionDataTobeProcessedBasedOnTeam != null && amionDataTobeProcessedBasedOnTeam.Count > 0)
                                {
                                    TeamLevelProcessing(team.Id, amionDataTobeProcessedBasedOnTeam, token, MappingFile: lstMappingFileModel, StartDate: objAMiONShiftMappingModel.StartDate.ToString("yyyy-MM-dd"), EndDate: objAMiONShiftMappingModel.EndDate.ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    lstLogger.Add(new Entities.Logger("#Team" + Guid.NewGuid().ToString(), "No amion data found for the selected team name", "Shift Processor", Entities.LogCategory.Info));
                                }
                            }
                        }
                        else
                        {
                            //TODO: to process in current team
                            //lstLogger.Add(new Entities.Logger("#Team" + Guid.NewGuid().ToString(), "No team found in microsoft database based on selected values", "Shift Processor", Entities.LogCategory.Info));
                        }
                    }
                    else
                    {
                        //TODO:Need to add current  team value in departmentValWhichNeedsTobeProcessedBasedOnSelectedVal
                    }
                }
                else
                {
                    //TODO:Teams data is empty in response of graph api
                    lstLogger.Add(new Entities.Logger("#JoinedTeam" + Guid.NewGuid().ToString(), "Teams data is empty in response of graph api", "Shift Processor", Entities.LogCategory.Info));
                }

                #region commented
                //Each data need to push into shift..

                //if (count < 5)
                //{
                //if (mappingFile != null)
                //{
                //    //fetch the team name in microsoft 0365 from mapping file
                //    DataRow amionDivisionEntityTeamRow = Helper.Utilities.CheckValueExistInDatatableBasedonColumnNumber(1, assignment.Division, mappingFile);
                //    assignment.Division = (amionDivisionEntityTeamRow != null && amionDivisionEntityTeamRow.ItemArray.Length > 0) ? amionDivisionEntityTeamRow[2].ToString() : assignment.Division;

                //    //fetch the shift name in microsoft 0365 from mapping file
                //    DataRow amionAssignmentShiftNameRow = Helper.Utilities.CheckValueExistInDatatableBasedonColumnNumber(1, assignment.assignmentName, mappingFile);
                //    assignment.assignmentName = (amionAssignmentShiftNameRow != null && amionAssignmentShiftNameRow.ItemArray.Length > 0) ? amionAssignmentShiftNameRow[2].ToString() : assignment.assignmentName;
                //}

                //Check team id exist with team name
                //if (objTeams != null && objTeams.Teams.Length > 0 && objTeams.Teams.Any(team => team.DisplayName == assignment.Division))
                //{
                //    //check user id exist with user name
                //    //MyTask:Hard Coded emailid logic, needs to be removed and amion data emailid logic needs to be added
                //    (bool isGetUserApiCallSuccess, string userResponseData) = DataProvider.GetDataFromInputUrl(token, @"/v1.0/users/Test7@TESTTESTvsatsinE351Dec21.onmicrosoft.com");
                //    if (isGetUserApiCallSuccess)
                //    {
                //        if ((!string.IsNullOrEmpty(userResponseData)) && (!userResponseData.Contains("error")))
                //        {
                //            //To fetch the team
                //            var team = objTeams.Teams.Single(userTeam => userTeam.DisplayName == assignment.Division);
                //            //check team id exist
                //            if (team != null && (!string.IsNullOrEmpty(team.Id)))
                //            {
                //                //To fetch team scheduling group id
                //                SchedulingGroup schedulingGroup = SchedulingGroupHelper.GetSchedulingGroupsFromTeamIdAndCheckGroupExist(token, team.Id, assignment.assignmentName);
                //                //To fetch the scheduling group id
                //                if (schedulingGroup != null && !string.IsNullOrEmpty(schedulingGroup.Id))
                //                {
                //                    shifts = new Shifts(userResponseData.ParseJson()["id"].ToString(), schedulingGroup.Id, assignment);
                //                }
                //                else
                //                {
                //                    var schedulingGrpId = SchedulingGroupHelper.MapAndCreateSchedulingGrpinTeams(token, team.Id, assignment.assignmentName, new List<Guid> { new Guid(userResponseData.ParseJson()["id"].ToString()) });
                //                    if (!string.IsNullOrEmpty(schedulingGrpId))
                //                    {
                //                        //Check for any error in response, if not parse the data and fetch the user ID and SchedulingGroup id
                //                        shifts = new Shifts(userResponseData.ParseJson()["id"].ToString(), schedulingGrpId, assignment);
                //                    }
                //                    else
                //                    {
                //                        //TODO:Shift group creation failed or errored out with some reason
                //                    }
                //                }
                //                if (shifts != null)
                //                {
                //                    //Push data into shift
                //                    if (CreateShift(team.Id, shifts, token))
                //                    {
                //                        shiftCreationCount++;
                //                    }
                //                    else
                //                    {
                //                        //TODO:Creation failed with some error
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                //TODO:Team id is empty
                //            }
                //        }
                //        else
                //        {
                //            //TODO:Call success but no response data
                //        }
                //    }
                //    else
                //    {
                //        //TODO: Call failed with network error or no data found for the user email id.
                //    }
                //}
                //{
                //    //TODO: If team name not matches the division name or there is no team with the assignment.Division
                //}
                //}
                //else
                //{
                //    //MyTask:Code needs to be removed
                //    //break;
                //}
                //count++;
                //}
                #endregion
            }
            else
            {
                //TODO: If no existing teams found for the user, how this scenario needs to be handled
            }

            return 1;
        }

        /// <summary>
        /// Shift creation helper
        /// </summary>
        /// <param name="teamID">Team id of the microsoft team</param>
        /// <param name="shifts"> shift object to be created</param>
        /// <param name="token">User token of the logged in user</param>
        private static bool CreateShift(string teamID, Shifts shifts, string token)
        {
            StringContent _strContentShiftsData = null;

            using (var strContentShifts = shifts.shift.AsJson())
                _strContentShiftsData = strContentShifts;

            var response = RestHelper.PostData(string.Format("{0}/beta/teams/{1}/schedule/shifts", Constant.GraphUrl, teamID), token, _strContentShiftsData);

            return (response.StatusCode == System.Net.HttpStatusCode.Created) ? true : false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="amionDataTobeProcessedBasedOnTeam"></param>
        /// <param name="token"></param>
        /// <param name="MappingFile"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static int TeamLevelProcessing(string teamID, List<AssignmentDetails> amionDataTobeProcessedBasedOnTeam, string token, List<MappingModel> MappingFile = null, string StartDate = "", string EndDate = "")
        {
            if (amionDataTobeProcessedBasedOnTeam == null)
            {
                throw new ArgumentNullException(nameof(amionDataTobeProcessedBasedOnTeam));
            }

            int shiftCreationCount = 0;
            Shifts shifts = null;


            (bool isGetUserGrpApiSuccess, string userGrpResData) = UserGroupHelper.GetMembersBasedonTeamIdFromGraphApi(teamID, token);

            if (isGetUserGrpApiSuccess)
            {
                var members = userGrpResData.AsObject<MembersList>();

                ProcessFetchingAndDeletionOfShits(token, teamID, StartDate, EndDate);

                foreach (var assignment in amionDataTobeProcessedBasedOnTeam)
                {
                    //Mapping teams data from mapping file
                    if (MappingFile != null && MappingFile.Count > 0)
                    {
                        //fetch the team name in microsoft 0365 from mapping file
                        if (MappingFile.Any(mapping => (!string.IsNullOrEmpty(mapping.AMiONDivision)) && string.Equals(assignment.Division, mapping.AMiONDivision.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            var teamsTeam = MappingFile.Find(mapping => string.Equals(assignment.Division, mapping.AMiONDivision.Trim(), StringComparison.OrdinalIgnoreCase)).TeamsTeam;

                            assignment.Division = string.IsNullOrWhiteSpace(teamsTeam) ? assignment.Division : teamsTeam.Trim();
                        }

                        //fetch the shift name in microsoft 0365 from mapping file 
                        if (MappingFile.Any(mapping => (!string.IsNullOrEmpty(mapping.AMiONAssignment)) && string.Equals(assignment.assignmentName, mapping.AMiONAssignment.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            var shiftGroupName = MappingFile.Find(mapping => (!string.IsNullOrEmpty(mapping.AMiONAssignment)) && string.Equals(mapping.AMiONAssignment.Trim(), assignment.assignmentName, StringComparison.OrdinalIgnoreCase)).ShiftGroup_Role;
                            assignment.assignmentName = string.IsNullOrWhiteSpace(shiftGroupName) ? assignment.assignmentName : shiftGroupName.Trim();
                        }
                    }

                    //MyTask: Currently email is not being returned in Amion data hence havent added email id based check
                    string userID = !members.Members.Any(userInTeam => userInTeam.UserPrincipalName == "Test7@TESTTESTvsatsinE351Dec21.onmicrosoft.com") ? "" : members.Members.Where(userInTeam => userInTeam.UserPrincipalName == "Test7@TESTTESTvsatsinE351Dec21.onmicrosoft.com").First().Id;
                    if (!string.IsNullOrEmpty(userID))
                    {
                        //To fetch team scheduling group id
                        SchedulingGroup schedulingGroup = SchedulingGroupHelper.GetSchedulingGroupsFromTeamIdAndCheckGroupExist(token, teamID, assignment.assignmentName);
                        //To fetch the scheduling group id
                        if (schedulingGroup != null && !string.IsNullOrEmpty(schedulingGroup.Id))
                        {
                            shifts = new Shifts(userID, schedulingGroup.Id, assignment);
                        }
                        else
                        {
                            var schedulingGrpId = SchedulingGroupHelper.MapAndCreateSchedulingGrpinTeams(token, teamID, assignment.assignmentName, new List<Guid> { new Guid(userID) });
                            if (!string.IsNullOrEmpty(schedulingGrpId))
                            {
                                //Check for any error in response, if not parse the data and fetch the user ID and SchedulingGroup id
                                shifts = new Shifts(userID, schedulingGrpId, assignment);
                            }
                            else
                            {
                                //TODO:Shift group creation failed or errored out with some reason
                            }
                        }
                        if (shifts != null)
                        {
                            //Push data into shift
                            if (CreateShift(teamID, shifts, token))
                            {
                                shiftCreationCount++;
                            }
                            else
                            {
                                //TODO:Creation failed with some error
                            }
                        }
                    }
                    else
                    {
                        //TODO:User email not matched in teams response based on amion data
                    }
                }
            }
            else
            {
                //TODO:no members found for the team id
            }
            return 0;
        }

        private static void ProcessFetchingAndDeletionOfShits(string token, string teamId, string startDate, string endDate)
        {
            //Get all shifts based on team and dates filter
            var shiftsDataFromGraphApi = ShiftsHelper.GetShifts(teamId, token, startDate, endDate);
            //Check shift exist based on user id
            //if (shiftsDataFromGraphApi != null && shiftsDataFromGraphApi.Shifts != null && shiftsDataFromGraphApi.Shifts.Length > 0 && shiftsDataFromGraphApi.Shifts.Any(shift => shift.UserId == userID))
            if (shiftsDataFromGraphApi != null && shiftsDataFromGraphApi.Shifts != null && shiftsDataFromGraphApi.Shifts.Length > 0)
            {
                //foreach (var objShift in shiftsDataFromGraphApi.Shifts)
                //{
                //    var (isDeleteCallSuccess, strResponseData) = ShiftsHelper.DeleteShift(teamId, objShift.Id, token);
                //    if (!isDeleteCallSuccess)
                //    {

                //    }
                //}
                var responseList = new List<string>();

                //Framing batch request for deletion
                var combinedBatchRequest = BatchRequestHelper.GenerateJSONBatchRequestList(shiftsDataFromGraphApi.Shifts.Select((shift) => (shift.Id, "DELETE", $"/teams/{teamId}/schedule/shifts/{shift.Id}")).ToList(), 20);

                //var httpWebResponse = SendAsyncRequest($"https://graph.microsoft.com/beta/$batch", HttpMethod.Post, token, jsonStringContent: new StringContent(combinedBatchRequest[0], Encoding.UTF8, "application/json"), isJsonContentTypeRequired: true);
                //running all batch request in parallel
                Parallel.ForEach(combinedBatchRequest, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (jsonData) =>
                {
                    try
                    {
                        var httpWebResponse = RestHelper.SendAsyncRequest($"{Constant.GraphUrl}/beta/$batch", HttpMethod.Post, token, jsonStringContent: new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json"), isJsonContentTypeRequired: true);
                        // Parse response
                        if (!string.IsNullOrEmpty(httpWebResponse))
                        {
                            responseList.Add(httpWebResponse);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });

                //TODO:Log response list
            }
        }
    }
}
