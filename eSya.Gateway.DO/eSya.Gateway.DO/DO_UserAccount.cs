using System;
using System.Collections.Generic;
using System.Text;

namespace eSya.Gateway.DO
{
    public class DO_UserAccount
    {
        public bool IsSucceeded { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ePassword { get; set; }
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
        public int UserID { get; set; }
        public int UserType { get; set; }
        public bool ForcePasswordChangeNextSignIn { get; set; }
        public bool BlockSignIn { get; set; }
        public int PasswordRepeatationPolicy { get; set; }
        public int UnsuccessfulLoginAttempt { get; set; }
        public int UnLockLoginInHours { get; set; }
        public int PasswordValidity { get; set; }
        public int LastPasswordChangedDay { get; set; }
        public Dictionary<int, string> l_BusinessKey { get; set; }
        public List<int> l_FinancialYear { get; set; }
        public int SelectedBusinessKey { get; set; }
        public int SelectedFinancialYear { get; set; }
        public int? DoctorID { get; set; }
        public string? LoginDesc { get; set; }
        public int SecurityQuestionId { get; set; }

    }
    public class DO_UserLogIn
    {
        public string? LoginID { get; set; }

        public string Password { get; set; }
        public string ePassword { get; set; }

        public int UnsuccessfulLoginAttempt { get; set; }

        public int UnLockLoginInHours { get; set; }

        public int PasswordValidity { get; set; }
    }
    public class DO_UserFinBusinessLocation
    { 
        public int BusinessKey { get; set; }
        public string BusinessLocation { get; set; }
        public List<int>? lstFinancialYear { get; set; }
        public List<DO_UserFinBusinessLocation>? lstUserLocation { get; set; }

    }
    public class DO_ChangeExpirationPassword
    {
        public int userID { get; set; }
        public string oldpassword { get; set; }
        public string newPassword { get; set; }
        public string FormID { get; set; }
        public int CreatedBy { get; set; }
        public string TerminalID { get; set; }
    }
    public class DO_UserPassword
    {
        public int UserID { get; set; }
        public string? LoginDesc { get; set; }
        public byte[] EPasswd { get; set; }
    }
}
