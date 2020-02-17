using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AMiON.Helper
{
    public class TeamList
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonProperty("value")]
        public Team[] Teams { get; set; }
    }

    public class Team
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("IsArchived")]
        public bool IsArchived { get; set; }

    }

    public class ShiftList
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonProperty("value")]
        public Shift[] Shifts { get; set; }
    }

    public class Shift
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "schedulingGroupId")]
        public string SchedulingGroupId { get; set; }
        [JsonProperty(PropertyName = "sharedShift")]
        public ShiftItem SharedShift { get; set; }
        [JsonProperty(PropertyName = "draftShift")]
        public ShiftItem DraftShift { get; set; }
    }

    public class ShiftItem
    {
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
        [JsonProperty(PropertyName = "startDateTime")]
        public string StartDateTime { get; set; }
        [JsonProperty(PropertyName = "endDateTime")]
        public string EndDateTime { get; set; }
        [JsonProperty(PropertyName = "theme")]
        public string Theme { get; set; }
        [JsonProperty(PropertyName = "activities")]
        public List<ShiftActivity> Activities { get; set; }
    }

    public class ShiftActivity
    {
        [JsonProperty(PropertyName = "isPaid")]
        public bool IsPaid { get; set; }
        [JsonProperty(PropertyName = "startDateTime")]
        public string StartDateTime { get; set; }
        [JsonProperty(PropertyName = "endDateTime")]
        public string EndDateTime { get; set; }
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
    }

    public class AssignmentDetails
    {
        public string Division { get; set; }
        public string staffName { get; set; }
        public StaffId staffId { get; set; }
        public string assignmentName { get; set; }
        public AssignmentId assignmentId { get; set; }
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }
        public bool IsValidEmailID { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string ShiftTime { get; set; }
        public string Pager { get; set; }
        public string Contact { get; set; }
        public string EMailId { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public string Training { get; set; }
        public string StaffBackupID { get; set; }
    }

    public class AssignmentId
    {
        public int id { get; set; }
        public int backupId { get; set; }
    }

    public class StaffId
    {
        public int id { get; set; }
        public int backupId { get; set; }
    }

    public class SchedulingGroups
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonProperty("@odata.count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public SchedulingGroup[] SchedulingGroup { get; set; }
    }

    public class SchedulingGroup
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTimeOffset? CreatedDateTime { get; set; }

        [JsonProperty("lastModifiedDateTime")]
        public DateTimeOffset? LastModifiedDateTime { get; set; }

        [JsonProperty("userIds")]
        public Guid[] UserIds { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("lastModifiedBy")]
        public LastModifiedBy LastModifiedBy { get; set; }

        [JsonProperty("draftShift")]
        public Shift DraftShift { get; set; }
    }

    public class LastModifiedBy
    {
        [JsonProperty("application")]
        public object Application { get; set; }

        [JsonProperty("device")]
        public object Device { get; set; }

        [JsonProperty("conversation")]
        public object Conversation { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }

    public class MembersList
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        [JsonProperty("value")]
        public Member[] Members { get; set; }
    }

    public class Member
    {
        [JsonProperty("@odata.type")]
        public string OdataEtag { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("businessPhones")]
        public string[] BusinessPhones { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }

        [JsonProperty("jobTitle")]
        public string JobTitle { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }

        [JsonProperty("mobilePhone")]
        public string MobilePhone { get; set; }

        [JsonProperty("officeLocation")]
        public string OfficeLocation { get; set; }

        [JsonProperty("PreferredLanguage")]
        public string PreferredLanguage { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }
    }


    public class Shifts

    {
        public Shift shift;
        public Shifts()
        {
        }

        public Shifts(string userID, string schedulingGroupId, AssignmentDetails assignment)
        {
            if (assignment == null)
            {
                throw new ArgumentNullException(nameof(assignment));
            }

            shift = new Shift
            {
                Id = $"SHFT_{Guid.NewGuid()}",
                UserId = string.IsNullOrEmpty(userID) ? Guid.NewGuid().ToString() : userID,
                SchedulingGroupId = schedulingGroupId,
                SharedShift = new ShiftItem
                {
                    DisplayName = "",
                    StartDateTime = assignment.startDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    EndDateTime = assignment.endDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Theme = "blue",
                    Activities = new List<ShiftActivity>()
                },
                DraftShift = null
            };
        }
    }

    public class ImportUserInputModel
    {
        public string AccessToken { get; set; }
        public string AmionLogin { get; set; }

        public string MappingFilePath { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<string> SelectedDepartments { get; set; }

        public List<string> SelectedColumnNames { get; set; }

        public List<string> RemovedColumnNames { get; set; }
    }

    public class DepartmentModel
    {
        public string DepartmentName { get; set; }

        public int ShiftsCount { get; set; }

    }

    public class EditColumnsModel
    {
        public string[] OrderedColumns { get; set; }
        public string[] RemovedColumns { get; set; }
    }

    public class MappingModel
    {
        public string AMiONDivision { get; set; }

        public string TeamsTeam { get; set; }

        public string AMiONAssignment { get; set; }

        public string ShiftGroup_Role { get; set; }

        public string ShouldImport { get; set; }
    }

    
}
