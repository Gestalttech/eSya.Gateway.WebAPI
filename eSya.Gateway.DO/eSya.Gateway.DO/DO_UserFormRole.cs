using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
   public class DO_UserFormRole
    {
        public int FormID { get; set; }
        public string FormIntID { get; set; }
        public string FormName { get; set; }
        public bool IsView { get; set; }
        public bool IsInsert { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsRePrint { get; set; }
        public bool IsApprove { get; set; }
        public bool IsAuthenticate { get; set; }
        public bool IsGiveConcession { get; set; }
        public bool IsGiveDiscount { get; set; }
    }
}
