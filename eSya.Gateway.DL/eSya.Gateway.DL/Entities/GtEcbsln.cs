using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcbsln
    {
        public GtEcbsln()
        {
            GtEuusfas = new HashSet<GtEuusfa>();
        }

        public int BusinessId { get; set; }
        public int LocationId { get; set; }
        public int BusinessKey { get; set; }
        public string ShortDesc { get; set; } = null!;
        public string LocationDescription { get; set; } = null!;
        public string BusinessName { get; set; } = null!;
        public int Isdcode { get; set; }
        public int CityCode { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public bool? TolocalCurrency { get; set; }
        public bool TocurrConversion { get; set; }
        public bool TorealCurrency { get; set; }
        public bool Lstatus { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; } = null!;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedTerminal { get; set; }

        public virtual ICollection<GtEuusfa> GtEuusfas { get; set; }
    }
}
