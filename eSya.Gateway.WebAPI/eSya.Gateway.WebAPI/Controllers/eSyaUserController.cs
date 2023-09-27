using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSya.Gateway.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class eSyaUserController : ControllerBase
    {
        private readonly IeSyaUserRepository _eSyaUserRepository;

        public eSyaUserController(IeSyaUserRepository eSyaUserRepository)
        {
            _eSyaUserRepository = eSyaUserRepository;
        }

        //[HttpPost]
        //public async Task<IActionResult> ValidateUserPassword(DO_UserAccount obj)
        //{
        //    var ds = await _eSyaUserRepository.ValidateUserPassword(obj.LoginID, obj.Password);
        //    return Ok(ds);
        //}
        [HttpPost]
        public async Task<IActionResult> ValidateUserPassword(DO_UserLogIn obj)
        {
            var ds = await _eSyaUserRepository.ValidateUserPassword(obj.LoginID, obj.Password);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GeteSyaUserMenulist(int userID)
        {
            var ds = await _eSyaUserRepository.GeteSyaUserMenulist(userID);
            return Ok(ds);
        }

        [HttpPost]
        public async Task<IActionResult> InsertIntoeSyaUser(DO_eSyaUser obj)
        {
            var ds = await _eSyaUserRepository.InsertIntoeSyaUser(obj);
            return Ok(ds);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateeSyaUser(DO_eSyaUser obj)
        {
            var ds = await _eSyaUserRepository.UpdateeSyaUser(obj);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GeteSyaUser()
        {
            var ds = await _eSyaUserRepository.GeteSyaUser();
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GeteSyaUserByUserID(int userID)
        {
            var ds = await _eSyaUserRepository.GeteSyaUserByUserID(userID);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GeteSyaUserByLoginID(string loginID)
        {
            var ds = await _eSyaUserRepository.GeteSyaUserByLoginID(loginID);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTypeByGroup(int userGroup)
        {
            var ds = await _eSyaUserRepository.GetUserTypeByGroup(userGroup);
            return Ok(ds);
        }

        [HttpGet]
        public async Task<IActionResult> GetBusinessLocation()
        {
            var ds = await _eSyaUserRepository.GetBusinessLocation();
            return Ok(ds);
        }
    }
}
