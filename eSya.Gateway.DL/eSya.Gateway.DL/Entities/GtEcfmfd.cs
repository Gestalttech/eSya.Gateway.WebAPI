using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcfmfd
    {
        public GtEcfmfd()
        {
            GtEcfmals = new HashSet<GtEcfmal>();
            GtEcfmnms = new HashSet<GtEcfmnm>();
            GtEcmnfls = new HashSet<GtEcmnfl>();
        }

        public int FormId { get; set; }
        public string? FormCode { get; set; }
        public string FormName { get; set; } = null!;
        public string ControllerName { get; set; } = null!;
        public string? ToolTip { get; set; }
        public bool ActiveStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual ICollection<GtEcfmal> GtEcfmals { get; set; }
        public virtual ICollection<GtEcfmnm> GtEcfmnms { get; set; }
        public virtual ICollection<GtEcmnfl> GtEcmnfls { get; set; }
    }
}
