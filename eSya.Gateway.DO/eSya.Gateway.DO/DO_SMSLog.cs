using System;
using System.Collections.Generic;
using System.Text;

namespace NG.Gateway.DO
{
   public  class DO_SMSLog
    {
        public long? ReferenceKey { get; set; }
        public string MobileNumber { get; set; }
        public string SMSStatement { get; set; }
        public string MessageType { get; set; }
        public bool SendStatus { get; set; }
        public string RequestMessage { get; set; }
        public string ResponseMessage { get; set; }
    }
}
