using eSya.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.IF
{
    public interface IApplicationRulesRepository
    {
        Task<bool> GetApplicationRuleStatusByID(int processID, int ruleID);

        Task<List<DO_ApplicationRules>> GetApplicationRuleListByProcesssID(int processID);

        Task<bool> GetBusinessApplicationRuleByBusinessKey(int businesskey, int processID, int ruleID);

        #region eSya Culture
        Task<List<DO_eSyaLoginCulture>> GetActiveCultures();
        #endregion
    }
}
