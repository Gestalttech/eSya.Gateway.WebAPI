using eSya.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.IF
{
    public interface IForgotUserPasswordRepository
    {
        #region ForGot User ID
        Task<DO_UserAccount> GetOTPbyMobileNumber(string mobileNo);
        Task<DO_UserAccount> ValidateUserbyOTP(string mobileNo, string otp, int expirytime);
        Task<DO_UserSecurityQuestions> GetRandomSecurityQuestion(string mobileNo);
        Task<DO_UserAccount> ValidateUserSecurityQuestion(DO_UserSecurityQuestions obj);
        #endregion
    }
}
