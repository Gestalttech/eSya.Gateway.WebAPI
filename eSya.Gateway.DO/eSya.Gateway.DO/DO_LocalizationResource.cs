using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
    public class DO_LocalizationResource
    {
        public string ResourceName { get; set; }
        public string Culture { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class FormControlProperty
    {
        public int ControlKey { get; set; }
        public string? InternalControlId { get; set; } 
        public string? ControlType { get; set; }
        public string? Property { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
