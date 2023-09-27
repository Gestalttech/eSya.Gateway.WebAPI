using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcmamn
    {
        public GtEcmamn()
        {
            GtEcmnfls = new HashSet<GtEcmnfl>();
            GtEcsbmns = new HashSet<GtEcsbmn>();
        }

        public int MainMenuId { get; set; }
        public string MainMenu { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int MenuIndex { get; set; }
        public bool ActiveStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual ICollection<GtEcmnfl> GtEcmnfls { get; set; }
        public virtual ICollection<GtEcsbmn> GtEcsbmns { get; set; }
    }
}
