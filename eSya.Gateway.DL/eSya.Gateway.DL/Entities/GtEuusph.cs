using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEuusph
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string LastPassword { get; set; } = null!;
        public DateTime LastPasswordChanged { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
    }
}
