using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.EntityFrameworkCore;
using eSya.Gateway.DL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.DL.Repository
{
    public class ApplicationRulesRepository : IApplicationRulesRepository
    {
        public async Task<bool> GetApplicationRuleStatusByID(int processID, int ruleID)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var ds = db.GtEcprrls
                        .Join(db.GtEcaprls,
                            p => p.ProcessId,
                            r => r.ProcessId,
                            (p, r) => new { p, r })
                        .Where(w => w.p.ProcessId == processID && w.r.RuleId == ruleID
                            && w.p.ActiveStatus && w.r.ActiveStatus)
                       .CountAsync();

                    return await ds > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<DO_ApplicationRules>> GetApplicationRuleListByProcesssID(int processID)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var ds = db.GtEcprrls
                        .Join(db.GtEcaprls,
                            p => p.ProcessId,
                            r => r.ProcessId,
                            (p, r) => new { p, r })
                        .Where(w => w.p.ProcessId == processID
                            && w.p.ActiveStatus)
                       .Select(s => new DO_ApplicationRules
                       {
                           ProcessID = s.p.ProcessId,
                           RuleID = s.r.RuleId,
                           RuleStatus = s.r.ActiveStatus
                       }).ToListAsync();

                    return await ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
