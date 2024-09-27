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

        #region Forgot Password 
        Task<DO_UserAccount> ValidateForgotPasswordOTP(string mobileNo, string otp, int expirytime);
        Task<DO_UserAccount> ValidateForgotPasswordSecurityQuestion(DO_UserSecurityQuestions obj);
        #endregion

        #region Change Password Expiration Password
        Task<DO_ReturnParameter> GetPasswordExpirationDays(string loginId);
        Task<DO_ReturnParameter> ChangeUserExpirationPassword(DO_ChangeExpirationPassword obj);
        Task<int> GetGatewayRuleValuebyRuleID(int GwRuleId);
        Task<DO_UserAccount> CheckValidateUserID(string loginID);
        Task<DO_ReturnParameter> ChangePasswordfromForgotPassword(int userId, string password);
        #endregion
    }
}
