using eSya.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.IF
{
    public interface IUserAccountRepository
    {
        Task<DO_UserAccount> ValidateUserPassword(string loginID, string password, int maxUnsuccessfulAttempts, int unLockLoginAfter, int PasswordValidity);
        Task<DO_UserAccount> ValidateUserMobileLogin(string mobileNumber);
        Task<DO_UserAccount> ValidateUserMobile(string mobileNumber);
        Task<DO_UserAccount> ValidateUserOTP(string mobileNumber, string otp);
        Task<DO_ReturnParameter> CreateUserPassword(int userID, string password, int passwordRepeatationPolicy);
        Task<DO_ReturnParameter> ResetUserPassword(int userID, string oldpassword, string newPassword, int passwordRepeatationPolicy);
        Task<string> GetUserPassword(int userID);
        Task<string> GetUserNameById(int userId);

        Task<List<DO_MainMenu>> GeteSyaMenulist();
        Task<DO_UserFormRole> GetFormAction(string navigationURL);

        Task<List<DO_MainMenu>> GetUserMenulist(int businessKey, int userID);
        Task<DO_UserFormRole> GetFormActionByUser(int businessKey, int userID, string navigationURL);

        Task<DO_UserAccount> GetUserBusinessLocation(int userID);

        #region Check User is Authenticated
        Task<DO_ReturnParameter> ChkIsUserAuthenticated(string loginId);
        #endregion

        #region Getting the User Location List
        Task<DO_UserFinBusinessLocation> GetUserLocationsbyUserID(string loginID);
        #endregion

        #region OTP Process
        Task<DO_UserAccount> ValidateCreateUserOTP(int userId, string otp);
        #endregion

        #region Create Password
        Task<DO_ReturnParameter> CreateUserPasswordINNextSignIn(int userId, string password);
        Task<DO_ReturnParameter> ChkIsCreatePasswordInNextSignIn(string loginId);
        #endregion

        #region User Security Question
        Task<DO_ReturnParameter> ChkIsUserQuestionsExists(string loginID);
        Task<int> GetNumberofQuestion(int GwRuleId);
        Task<DO_ReturnParameter> InsertUserSecurityQuestion(List<DO_UserSecurityQuestions> obj);
        #endregion

        
    }
}
