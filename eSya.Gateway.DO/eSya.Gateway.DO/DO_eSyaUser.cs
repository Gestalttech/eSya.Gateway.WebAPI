using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
    public class DO_eSyaUser
    {
        public int UserID { get; set; }
        public string LoginID { get; set; }
        public string LoginDesc { get; set; }
        public string Password { get; set; }
        public int UserGroup { get; set; }
        public string UserGroupDesc { get; set; }
        public int UserType { get; set; }
        public string UserTypeDesc { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int CreatedBy { get; set; }
        public string TerminalID { get; set; }
    }
}
