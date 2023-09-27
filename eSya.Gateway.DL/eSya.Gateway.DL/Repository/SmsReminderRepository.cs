using eSya.Gateway.DL.Entities;
using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.EntityFrameworkCore;
using NG.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.DL.Repository
{
   public  class SmsReminderRepository : ISmsReminderRepository
    {
        public async Task<List<DO_SmsReminder>> GetSmsReminderSchedule()
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var ds = db.GtEcsmsses
                     .Where(w => w.ActiveStatus)
                     .Select(r => new DO_SmsReminder
                     {
                         ReminderType = r.ReminderType,
                         SmsId = r.Smsid,
                         ScheduleOnDay = r.ScheduleOnDay,
                         ScheduleTime = r.ScheduleTime
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
