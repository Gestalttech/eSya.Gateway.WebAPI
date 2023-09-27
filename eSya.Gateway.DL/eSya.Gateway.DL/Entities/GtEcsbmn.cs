using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcsbmn
    {
        public int MenuItemId { get; set; }
        public int MainMenuId { get; set; }
        public string MenuItemName { get; set; } = null!;
        public int ParentId { get; set; }
        public int MenuIndex { get; set; }
        public string? ImageUrl { get; set; }
        public bool ActiveStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual GtEcmamn MainMenu { get; set; } = null!;
    }
}
