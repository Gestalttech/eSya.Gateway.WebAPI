using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcltfc
    {
        public GtEcltfc()
        {
            GtEcltcds = new HashSet<GtEcltcd>();
        }

        public int ResourceId { get; set; }
        public string ResourceName { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual ICollection<GtEcltcd> GtEcltcds { get; set; }
    }
}
