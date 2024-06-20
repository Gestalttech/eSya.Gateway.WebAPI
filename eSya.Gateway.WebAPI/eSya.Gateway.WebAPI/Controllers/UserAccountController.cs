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
    }
}
