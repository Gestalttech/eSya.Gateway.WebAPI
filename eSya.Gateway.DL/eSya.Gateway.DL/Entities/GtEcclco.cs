using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcclco
    {
        public string CalenderType { get; set; } = null!;
        public int Year { get; set; }
        public int StartMonth { get; set; }
        public string CalenderKey { get; set; } = null!;
        public DateTime FromDate { get; set; }
        public DateTime TillDate { get; set; }
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
