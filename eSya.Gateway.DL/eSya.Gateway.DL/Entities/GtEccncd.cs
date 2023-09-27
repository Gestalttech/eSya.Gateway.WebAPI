using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEccncd
    {
        public int Isdcode { get; set; }
        public string CountryCode { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public string CountryFlag { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
        public string? MobileNumberPattern { get; set; }
        public string? Nationality { get; set; }
        public bool IsPoboxApplicable { get; set; }
        public string? PoboxPattern { get; set; }
        public bool IsPinapplicable { get; set; }
        public string? PincodePattern { get; set; }
        public string? DateFormat { get; set; }
        public string? ShortDateFormat { get; set; }
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
