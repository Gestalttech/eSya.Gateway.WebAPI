using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSya.Gateway.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ForgotUserPasswordController : ControllerBase
    {
        private readonly IForgotUserPasswordRepository _forgotUserPasswordRepository;

        public ForgotUserPasswordController(IForgotUserPasswordRepository forgotUserPasswordRepository)
        {
            _forgotUserPasswordRepository = forgotUserPasswordRepository;
        }

        #region Forgot Get User ID
        [HttpGet]
        public async Task<IActionResult> GetOTPbyMobileNumber(string mobileNo)
        {
            var ds = await _forgotUserPasswordRepository.GetOTPbyMobileNumber(mobileNo);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateUserbyOTP(String mobileNo, string otp, int expirytime)
        {
            var ds = await _forgotUserPasswordRepository.ValidateUserbyOTP(mobileNo, otp, expirytime);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GetRandomSecurityQuestion(string mobileNo)
        {
            var ds = await _forgotUserPasswordRepository.GetRandomSecurityQuestion(mobileNo);
            return Ok(ds);
        }
        [HttpPost]
        public async Task<IActionResult> ValidateUserSecurityQuestion(DO_UserSecurityQuestions obj)
        {
            var msg = await _forgotUserPasswordRepository.ValidateUserSecurityQuestion(obj);
            return Ok(msg);
        }
        #endregion

        #region Forgot Password 
        [HttpGet]
        public async Task<IActionResult> ValidateForgotPasswordOTP(string mobileNo, string otp, int expirytime)
        {
            var ds = await _forgotUserPasswordRepository.ValidateForgotPasswordOTP(mobileNo, otp, expirytime);
            return Ok(ds);
        }
        [HttpPost]
        public async Task<IActionResult> ValidateForgotPasswordSecurityQuestion(DO_UserSecurityQuestions obj)
        {
            var ds = await _forgotUserPasswordRepository.ValidateForgotPasswordSecurityQuestion(obj);
            return Ok(ds);
        }
        #endregion

        #region Change Password Expiration Password
        [HttpGet]
        public async Task<IActionResult> GetPasswordExpirationDays(string loginId)
        {
            var ds = await _forgotUserPasswordRepository.GetPasswordExpirationDays(loginId);
            return Ok(ds);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeUserExpirationPassword(DO_ChangeExpirationPassword obj)
        {
            var ds = await _forgotUserPasswordRepository.ChangeUserExpirationPassword(obj);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GetGatewayRuleValuebyRuleID(int GwRuleId)
        {
            var ds = await _forgotUserPasswordRepository.GetGatewayRuleValuebyRuleID(GwRuleId);
            return Ok(ds);
        }
        #endregion
    }
}
