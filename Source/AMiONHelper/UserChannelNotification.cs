using AMiON.Helper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper
{
    public class UserChannelNotification
    {
        public static (bool isChannelPostingSuccess, string channelResponse) ChannelNotification(string token, string teamID, string channelId, string bodyContent, ChatMessageAttachment chatMessageAttachment =null)
        {
            string strResponseData = string.Empty;
            if (teamID == null || channelId ==null || token == null)
            {
                throw new ArgumentException();
            }

            StringContent _teamsChatMessageStrContent = null;

            TeamsChatMessage teamsChatMessage=new TeamsChatMessage{
                BodyContent= new BodyContent
                {
                    Content=bodyContent,
                    ContentType="html"
                },
            };

            using (var teamsChatMessageStringContent = teamsChatMessage.AsJson())
                _teamsChatMessageStrContent = teamsChatMessageStringContent;
            var response = RestHelper.PostData(string.Format("{0}/beta/teams/{1}/channels/{2}/messages", Constant.GraphUrl, teamID, channelId), token, _teamsChatMessageStrContent);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return (true,"");
            }
            else
            {
                if(response.Content != null)
                {
                    return (false, response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                }
            }
            return (false, "Error:Something went wrong at channel notification, response is empty");
        }

        private static string frameBodyContentData(string bodyContent)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.AppendFormat("====================================================================\n{0}\n====================================================================\n", bodyContent).ToString();
        }
        
        private static string FrameTableBodyContentForLogsData(DataList<Entities.Logger> dataList)
        {
            if (dataList == null)
            {
                throw new ArgumentNullException(nameof(dataList));
            }

            StringBuilder stringBuilder = new StringBuilder();

            var lstLogDetails = dataList.GetAllDataList();

            if (lstLogDetails != null)
            {
                for (int logLoop = 0; logLoop < lstLogDetails.Count; logLoop++)
                {
                    if (logLoop == 0)
                    {
                        stringBuilder.Append("<table><thead><th>Log message</th><th>Log category</th></thead><tbody>");
                    }
                    stringBuilder.Append($"<tr><td>{lstLogDetails[logLoop].LogMessage}</td><td>{lstLogDetails[logLoop].LogCategory.ToString()}</td></tr>");
                    if(logLoop == lstLogDetails.Count - 1)
                    {
                        stringBuilder.Append("</tbody></table>");
                    }
                }
            }

            return stringBuilder.ToString();
        }

    }
}
