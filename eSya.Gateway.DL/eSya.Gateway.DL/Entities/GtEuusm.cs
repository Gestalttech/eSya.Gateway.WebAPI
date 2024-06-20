using System;
using System.Collections.Generic;

namespace eSya.Gateway.DL.Entities
{
    public partial class GtEuusm
    {
        public int UserId { get; set; }
        public string LoginId { get; set; } = null!;
        public string LoginDesc { get; set; } = null!;
        public byte[]? Photo { get; set; }
        public string? PhotoUrl { get; set; }
        public string EMailId { get; set; } = null!;
        public bool CreatePasswordInNextSignIn { get; set; }
        public int UnsuccessfulAttempt { get; set; }
        public DateTime? LoginAttemptDate { get; set; }
        public bool BlockSignIn { get; set; }
        public DateTime? LastPasswordUpdatedDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public bool IsUserAuthenticated { get; set; }
        public DateTime? UserAuthenticatedDate { get; set; }
        public bool IsUserDeactivated { get; set; }
        public DateTime? UserDeactivatedOn { get; set; }
        public string? DeactivationReason { get; set; }
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
