using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcsm91
    {
        public int BusinessKey { get; set; }
        public string ServiceProvider { get; set; } = null!;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTill { get; set; }
        public string Api { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SenderId { get; set; } = null!;
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }
    }
}
