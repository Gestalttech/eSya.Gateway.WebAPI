using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
    public class DO_ConfigureMenu
    {
        public List<DO_MainMenu> l_MainMenu { get; set; }
        public List<DO_SubMenu> l_SubMenu { get; set; }
        public List<DO_FormMenu> l_FormMenu { get; set; }
    }
    public class DO_MainMenu
    {
        public int MainMenuId { get; set; }
        public string MainMenu { get; set; }
        public string ImageURL { get; set; }
        public int MenuIndex { get; set; }
        public bool ActiveStatus { get; set; }
        public int UserId { get; set; }
        public string TerminalId { get; set; }

        public List<DO_SubMenu> l_SubMenu { get; set; }

        public List<DO_FormMenu> l_FormMenu { get; set; }
    }

    public class DO_SubMenu
    {
        public int MenuItemId { get; set; }
        public int MainMenuId { get; set; }
        public string MenuItemName { get; set; }
        public string MenuSubGroupName { get; set; }
        public int ParentID { get; set; }
        public string ImageURL { get; set; }
        public int MenuIndex { get; set; }
        public bool ActiveStatus { get; set; }
        public int UserId { get; set; }
        public string TerminalId { get; set; }

        public List<DO_SubMenu> l_SubMenu { get; set; }
        public List<DO_FormMenu> l_FormMenu { get; set; }
    }

    public class DO_FormMenu
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public int MainMenuId { get; set; }
        public int FormId { get; set; }
        public string FormNameClient { get; set; }
        public int FormIndex { get; set; }
        public bool ActiveStatus { get; set; }
        public int UserId { get; set; }
        public string TerminalId { get; set; }
        //property for UserGroup 
        public int MenuKey { get; set; }

        public int ParentId { get; set; }

        public string FormInternalID { get; set; }
        public string NavigateUrl { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string View { get; set; }

        public List<DO_FormMenu> l_FormMenu { get; set; }
    }
}
