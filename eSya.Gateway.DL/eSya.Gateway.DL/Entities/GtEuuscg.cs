using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEuuscg
    {
        public int UserId { get; set; }
        public string LoginId { get; set; } = null!;
        public string LoginDesc { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int UserGroup { get; set; }
        public int UserType { get; set; }
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
