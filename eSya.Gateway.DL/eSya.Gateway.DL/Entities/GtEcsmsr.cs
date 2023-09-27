using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcsmsr
    {
        public int BusinessKey { get; set; }
        public string Smsid { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string RecipientName { get; set; } = null!;
        public string? Remarks { get; set; }
        public bool ActiveStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual GtEcsmsh Sms { get; set; } = null!;
    }
}
