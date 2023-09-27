using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
   public class DO_SmsReminder
    {
        public string ReminderType { get; set; }
        public string SmsId { get; set; }
        public long ReferenceKey { get; set; }
        public int ScheduleOnDay { get; set; }
        public TimeSpan ScheduleTime { get; set; }
        public bool SendStatus { get; set; }
    }
}
