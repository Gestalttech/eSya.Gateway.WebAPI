using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
    public class DO_ReturnParameter
    {
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public decimal ID { get; set; }
        public string Key { get; set; }
        public int BusinessKey { get; set; }
    }
}
