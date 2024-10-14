using eSya.Gateway.DO;
using eSya.Gateway.IF;
using eSya.Gateway.WebAPI.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace eSya.Gateway.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;

        public UserAccountController(IUserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        //[HttpPost]
        //public async Task<IActionResult> ValidateUserPassword(DO_UserAccount obj)
        //{
        //    obj.ePassword = CryptGeneration.Encrypt(obj.Password);
        //    var ds = await _userAccountRepository.ValidateUserPassword(obj.LoginID, obj.ePassword, obj.UnsuccessfulLoginAttempt, obj.UnLockLoginInHours, obj.PasswordValidity);
        //    return Ok(ds);
        //}
        [HttpPost]
        public async Task<IActionResult> ValidateUserPassword(DO_UserLogIn obj)
        {
            obj.ePassword = CryptGeneration.Encrypt(obj.Password);
            var ds = await _userAccountRepository.ValidateUserPassword(obj.LoginID, obj.ePassword, obj.UnsuccessfulLoginAttempt, obj.UnLockLoginInHours, obj.PasswordValidity);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> ValidateUserMobileLogin(string mobileNumber)
        {
            var ds = await _userAccountRepository.ValidateUserMobileLogin(mobileNumber);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateUserMobile(string mobileNumber)
        {
            var ds = await _userAccountRepository.ValidateUserMobile(mobileNumber);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNameById(int userId)
        {
            var ds = await _userAccountRepository.GetUserNameById(userId);
            return Ok(new { Key = ds });
        }

        [HttpPost]
        public async Task<IActionResult> ValidateUserOTP(DO_UserAccount obj)
        {
            var ds = await _userAccountRepository.ValidateUserOTP(obj.MobileNumber, obj.OTP);
            return Ok(ds);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserPassword(DO_UserAccount obj)
        {
            var password = CryptGeneration.Encrypt(obj.Password);
            var ds = await _userAccountRepository.CreateUserPassword(obj.UserID, password, obj.PasswordRepeatationPolicy);
            return Ok(ds);
        }

        [HttpPost]
        public async Task<IActionResult> ResetUserPassword(DO_UserAccount obj)
        {
            var oldPassword = CryptGeneration.Encrypt(obj.Password);
            var newPassword = CryptGeneration.Encrypt(obj.NewPassword);
            var ds = await _userAccountRepository.ResetUserPassword(obj.UserID, oldPassword, newPassword, obj.PasswordRepeatationPolicy);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GeteSyaMenulist()
        {
            var ds = await _userAccountRepository.GeteSyaMenulist();
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GetFormAction(string navigationURL)
        {
            var ds = await _userAccountRepository.GetFormAction(navigationURL);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMenulist(int businessKey, int userID)
        {
            var ds = await _userAccountRepository.GetUserMenulist(businessKey, userID);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GetFormActionByUser(int businessKey, int userID, string navigationURL)
        {
            var ds = await _userAccountRepository.GetFormActionByUser(businessKey, userID, navigationURL);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserBusinessLocation(int userID)
        {
            var ds = await _userAccountRepository.GetUserBusinessLocation(userID);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserRolebyUserID(int userID, int businbessKey)
        {
            var ds = await _userAccountRepository.GetUserRolebyUserID(userID, businbessKey);
            return Ok(ds);
        }
        

        #region Getting the User Location List
        [HttpGet]
        public async Task<IActionResult> GetUserLocationsbyUserID(string loginID)
        {
            var ds = await _userAccountRepository.GetUserLocationsbyUserID(loginID);
            return Ok(ds);
        }
        #endregion

        #region Check User is Authenticated
        [HttpGet]
        public async Task<IActionResult> ChkIsUserAuthenticated(string loginId)
        {
            var ds = await _userAccountRepository.ChkIsUserAuthenticated(loginId);
            return Ok(ds);
        }
        #endregion

        #region OTP Process
        [HttpGet]
        public async Task<IActionResult> ValidateCreateUserOTP(int userId, string otp)
        {
            var ds = await _userAccountRepository.ValidateCreateUserOTP(userId, otp);
            return Ok(ds);
        }
        #endregion

        #region Create Password
        [HttpGet]
        public async Task<IActionResult> CreateUserPasswordINNextSignIn(int userId, string password)
        {
            var ds = await _userAccountRepository.CreateUserPasswordINNextSignIn(userId, password);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> ChkIsCreatePasswordInNextSignIn(string loginId)
        {
            var ds = await _userAccountRepository.ChkIsCreatePasswordInNextSignIn(loginId);
            return Ok(ds);
        }
        #endregion

        #region User Security Question
        [HttpGet]
        public async Task<IActionResult> ChkIsUserQuestionsExists(string loginID)
        {
            var ds = await _userAccountRepository.ChkIsUserQuestionsExists(loginID);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GetNumberofQuestion(int GwRuleId)
        {
            var ds = await _userAccountRepository.GetNumberofQuestion(GwRuleId);
            return Ok(ds);
        }
       
        [HttpPost]
        public async Task<IActionResult> InsertUserSecurityQuestion(List<DO_UserSecurityQuestions> obj)
        {
            var ds = await _userAccountRepository.InsertUserSecurityQuestion(obj);
            return Ok(ds);
        }
        #endregion

        #region Mobile Login functionality
        [HttpGet]
        public async Task<IActionResult> ValidateUserMobileNumberGetOTP(string mobileNo)
        {
            var ds = await _userAccountRepository.ValidateUserMobileNumberGetOTP(mobileNo);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> ValidateUserMobileNumberbyOTP(string mobileNo, string otp, int expirytime)
        {
            var ds = await _userAccountRepository.ValidateUserMobileNumberbyOTP(mobileNo, otp, expirytime);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> ValidateUserMobileNumberGetRandomSecurityQuestion(string mobileNo)
        {
            var ds = await _userAccountRepository.ValidateUserMobileNumberGetRandomSecurityQuestion(mobileNo);
            return Ok(ds);
        }
        [HttpPost]
        public async Task<IActionResult> ValidateMobileLoginUserSecurityQuestion(DO_UserSecurityQuestions obj)
        {
            var ds = await _userAccountRepository.ValidateMobileLoginUserSecurityQuestion(obj);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserLocationsbyMobileNumber(string mobileNo)
        {
            var ds = await _userAccountRepository.GetUserLocationsbyMobileNumber(mobileNo);
            return Ok(ds);
        }
        #endregion
    }
}
