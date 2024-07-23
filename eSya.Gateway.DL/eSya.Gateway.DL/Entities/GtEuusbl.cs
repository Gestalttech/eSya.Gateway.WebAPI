using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEuusbl
    {
        public GtEuusbl()
        {
            GtEuusmls = new HashSet<GtEuusml>();
        }

        public int UserId { get; set; }
        public int BusinessKey { get; set; }
        public bool AllowMtfy { get; set; }
        public string PreferredLanguage { get; set; } = null!;
        public int Isdcode { get; set; }
        public string MobileNumber { get; set; } = null!;
        public int IsdcodeWan { get; set; }
        public string WhatsappNumber { get; set; } = null!;
        public int ESyaAuthentication { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual ICollection<GtEuusml> GtEuusmls { get; set; }
    }
}
