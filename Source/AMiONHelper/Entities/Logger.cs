using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper.Entities
{
    /// <summary>
    /// Class to log the application information as well as error logs based on requirement 
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// To uniquely identify the log
        /// </summary>
        public string LogID { get; set; }
        /// <summary>
        /// Message of the log error/information
        /// </summary>
        public string LogMessage { get; set; }
        /// <summary>
        /// Source of log
        /// </summary>
        public string LogReference { get; set; }
        /// <summary>
        /// Category of the log
        /// </summary>
        public LogCategory LogCategory { get; set; }

        public Logger(string logID,
                      string logMessage,
                      string logReference,
                      LogCategory logCategory)
        {
            LogID = logID;
            LogMessage = logMessage;
            LogReference = logReference;
            LogCategory = logCategory;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public enum LogCategory
    {
        Info,
        ApplicationError,
        NetworkError
    }
}
