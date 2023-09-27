using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEcsmsl
    {
        public long Smsid { get; set; }
        public DateTime SendDateTime { get; set; }
        public long? ReferenceKey { get; set; }
        public string? MobileNumber { get; set; }
        public string? Smsstatement { get; set; }
        public string? MessageType { get; set; }
        public bool SendStatus { get; set; }
        public string? RequestMessage { get; set; }
        public string? ResponseMessage { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
