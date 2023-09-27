using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcfmnm
    {
        public int FormId { get; set; }
        public string FormIntId { get; set; } = null!;
        public string FormDescription { get; set; } = null!;
        public string NavigateUrl { get; set; } = null!;
        public bool ActiveStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual GtEcfmfd Form { get; set; } = null!;
    }
}
