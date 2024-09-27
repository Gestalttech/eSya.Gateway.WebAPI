using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEuuspa
    {
        public int UserId { get; set; }
        public int ParameterId { get; set; }
        public decimal ParmPerc { get; set; }
        public bool ParmAction { get; set; }
        public string ParmDesc { get; set; } = null!;
        public decimal ParmValue { get; set; }
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
