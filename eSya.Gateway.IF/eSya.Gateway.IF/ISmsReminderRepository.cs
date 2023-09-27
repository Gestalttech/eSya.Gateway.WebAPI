using eSya.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.IF
{
    public interface ISmsReminderRepository
    {
        Task<List<DO_SmsReminder>> GetSmsReminderSchedule();
    }
}
