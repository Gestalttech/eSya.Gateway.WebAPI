using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEuusm
    {
        public int UserId { get; set; }
        public string LoginId { get; set; } = null!;
        public string LoginDesc { get; set; } = null!;
        public bool EnforcePasswdPolicy { get; set; }
        public bool EnforcePasswdExpiration { get; set; }
        public string Password { get; set; } = null!;
        public int? Isdcode { get; set; }
        public string MobileNumber { get; set; } = null!;
        public bool? AllowMobileLogin { get; set; }
        public string EMailId { get; set; } = null!;
        public byte[]? Photo { get; set; }
        public string? PhotoUrl { get; set; }
        public byte[]? DigitalSignature { get; set; }
        public bool ForcePasswordChangeNextSignIn { get; set; }
        public bool BlockSignIn { get; set; }
        public int UnsuccessfulLoginAttempts { get; set; }
        public DateTime? LoginAttemptDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public string? Otpnumber { get; set; }
        public DateTime? OtpgeneratedDate { get; set; }
        public string? PreferredLanguage { get; set; }
        public DateTime? UserCreatedDate { get; set; }
        public bool IsUserAuthenticated { get; set; }
        public DateTime? UserAuthenticatedDate { get; set; }
        public bool IsUserDeactivated { get; set; }
        public DateTime? UserDeactivatedOn { get; set; }
        public string? DeactivationReason { get; set; }
        public bool IsApprover { get; set; }
        public bool IsDoctor { get; set; }
        public int? DoctorId { get; set; }
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
