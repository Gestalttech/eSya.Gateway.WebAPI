using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcsmsc
    {
        public string ReminderType { get; set; } = null!;
        public string Smsid { get; set; } = null!;
        public long ReferenceKey { get; set; }
        public bool SendStatus { get; set; }
        public bool ActiveStatus { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
