using eSya.Gateway.DL.Repository;
using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSya.Gateway.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApplicationRulesController : ControllerBase
    {
        private readonly IApplicationRulesRepository _applicationRulesRepository;
        
        public ApplicationRulesController(IApplicationRulesRepository applicationRulesRepository)
        {
            _applicationRulesRepository = applicationRulesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationRuleStatusByID(int processID, int ruleID)
        {
            var ds = await _applicationRulesRepository.GetApplicationRuleStatusByID(processID, ruleID);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GetMobileLoginApplicationRuleStatusByID(int processID)
        {
            var ds = await _applicationRulesRepository.GetMobileLoginApplicationRuleStatusByID(processID);
            return Ok(ds);
        }
       //need to delete
        [HttpGet]
        public async Task<IActionResult> GetApplicationRuleListByProcesssID(int processID)
        {
            var ds = await _applicationRulesRepository.GetApplicationRuleListByProcesssID(processID);
            return Ok(ds);
        }
        [HttpGet]
        public async Task<IActionResult> GetBusinessApplicationRuleByBusinessKey(int businesskey, int processID, int ruleID)
        {
            var ds = await _applicationRulesRepository.GetBusinessApplicationRuleByBusinessKey(businesskey,processID, ruleID);
            return Ok(ds);
        }
        #region eSya Culture
        [HttpGet]
        public async Task<IActionResult> GetActiveCultures()
        {
            var ds = await _applicationRulesRepository.GetActiveCultures();
            return Ok(ds);
        }
        
        #endregion
    }
}
