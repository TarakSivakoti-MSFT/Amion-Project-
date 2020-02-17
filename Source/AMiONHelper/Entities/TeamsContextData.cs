using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMiON.Helper.Entities
{
    /// <summary>
    /// Teams context entity object of the logged in  user
    /// </summary>
    public class TeamsContextData
    {
        
        /// <summary>
        /// Team id
        /// </summary>
        public string TeamID { get; set; }
        
        /// <summary>
        /// Channel id
        /// </summary>
        public string ChannelID { get; set; }

    }
}
