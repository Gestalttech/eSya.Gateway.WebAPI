using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
    public enum smsParams
    {
        User = 1,
        Patient = 2,
        NextToKin = 3,
        Doctor = 4,
        Recipient = 5,
        Customer = 6,
        Vendor = 7,
        Employee = 8,
        Direct = 9,

    }

    public class DO_SmsParameter
    {
        public string MessageType { get; set; }
        public string ReminderType { get; set; }
        public string NavigationURL { get; set; }
        public int FormID { get; set; }
        public string SMSID { get; set; }
        public long ReferenceKey { get; set; }
        public int TEventID { get; set; }
        public int UserID { get; set; }
        public string LoginID { get; set; }
        public string UserName { get; set; }
        public int BusinessKey { get; set; }
        public int UHID { get; set; }
        public int DoctorID { get; set; }
        public int CustomerID { get; set; }
        public int VendorID { get; set; }
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public Dictionary<string, string> SmsVariables { get; set; }

        public string OTP { get; set; }
        public string Password { get; set; }
        public bool IsUserPasswordInclude { get; set; }
    }

    public class DO_SmsStatement
    {
        public string NavigationURL { get; set; }
        public string SMSID { get; set; }
        public int FormID { get; set; }
        public string SMSDescription { get; set; }
        public bool IsVariable { get; set; }
        public string SMSStatement { get; set; }

        public List<DO_SmsParam> l_SmsParam { get; set; }
        public List<DO_SmsRecipient> l_SmsRecipient { get; set; }
    }

    public class DO_SmsParam
    {
        public string SMSID { get; set; }
        public int ParameterID { get; set; }
        public bool ParmAction { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
    }
    public class DO_SmsRecipient
    {
        public string SMSID { get; set; }
        public string MobileNumber { get; set; }
        public string RecipientName { get; set; }
        public string Remarks { get; set; }
    }

    public class DO_Master
    {
        public string MobileNumber { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
