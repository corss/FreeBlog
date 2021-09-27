using System;
using System.Collections.Generic;
using System.Text;

namespace FreeBlog.Model.ViewModels.Api
{
    public class APITeamMembersViewModel
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 成员ID
        /// </summary>
        public string Name { get; set; }
    }
    public class APITeamMembersViewModel2
    {
        public APITeamMembersViewModel2(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }
        /// <summary>
        /// 成员ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 成员ID
        /// </summary>
        public string Name { get; set; }
    }
   
}
