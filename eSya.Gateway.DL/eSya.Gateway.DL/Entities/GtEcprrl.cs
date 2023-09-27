using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcprrl
    {
        public GtEcprrl()
        {
            GtEcaprls = new HashSet<GtEcaprl>();
        }

        public int ProcessId { get; set; }
        public string ProcessDesc { get; set; } = null!;
        public bool IsSegmentSpecific { get; set; }
        public bool SystemControl { get; set; }
        public string ProcessControl { get; set; } = null!;
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual ICollection<GtEcaprl> GtEcaprls { get; set; }
    }
}
