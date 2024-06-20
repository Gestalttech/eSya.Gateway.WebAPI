using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEsdocd
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string DoctorShortName { get; set; } = null!;
        public int Gender { get; set; }
        public string DoctorRegnNo { get; set; } = null!;
        public int Isdcode { get; set; }
        public string MobileNumber { get; set; } = null!;
        public string? EmailId { get; set; }
        public int DoctorClass { get; set; }
        public int DoctorCategory { get; set; }
        public int TraiffFrom { get; set; }
        public string? Password { get; set; }
        public int SeniorityLevel { get; set; }
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
