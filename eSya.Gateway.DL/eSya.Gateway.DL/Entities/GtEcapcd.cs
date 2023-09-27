using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcapcd
    {
        public GtEcapcd()
        {
            GtEuusgrUserGroupNavigations = new HashSet<GtEuusgr>();
            GtEuusgrUserTypeNavigations = new HashSet<GtEuusgr>();
        }

        public int ApplicationCode { get; set; }
        public int CodeType { get; set; }
        public string CodeDesc { get; set; } = null!;
        public string? ShortCode { get; set; }
        public bool DefaultStatus { get; set; }
        public bool UsageStatus { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual ICollection<GtEuusgr> GtEuusgrUserGroupNavigations { get; set; }
        public virtual ICollection<GtEuusgr> GtEuusgrUserTypeNavigations { get; set; }
    }
}
