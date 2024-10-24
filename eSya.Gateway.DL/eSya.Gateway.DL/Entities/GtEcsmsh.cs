using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcsmsh
    {
        public GtEcsmsh()
        {
            GtEcsmsds = new HashSet<GtEcsmsd>();
            GtEcsmsrs = new HashSet<GtEcsmsr>();
        }

        public string Smsid { get; set; } = null!;
        public int FormId { get; set; }
        public string Smsdescription { get; set; } = null!;
        public bool IsVariable { get; set; }
        public int TeventId { get; set; }
        public string Smsstatement { get; set; } = null!;
        public int SequenceNumber { get; set; }
        public bool ActiveStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual ICollection<GtEcsmsd> GtEcsmsds { get; set; }
        public virtual ICollection<GtEcsmsr> GtEcsmsrs { get; set; }
    }
}
