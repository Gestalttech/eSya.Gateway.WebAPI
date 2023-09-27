using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
   public class DO_FormAction
    {
        public int FormID { get; set; }
        public string FormIntID { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }

    public class DO_MenuAction
    {
        public int MenuKey { get; set; }
        public int ActionId { get; set; }
    }
}
