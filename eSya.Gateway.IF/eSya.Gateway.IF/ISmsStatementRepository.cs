using eSya.Gateway.DO;
using NG.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.IF
{
    public interface ISmsStatementRepository
    {
        Task<List<DO_SmsStatement>> GetSmsStatementByForm(DO_SmsParameter sp);
        Task<List<DO_SmsStatement>> GetSmsStatementById(DO_SmsParameter sp);
        Task<bool> Insert_SmsLog(DO_SMSLog obj);
        Task<bool> Insert_SmsReminderLog(DO_SmsReminder obj);
        Task<List<DO_SmsStatement>> GetSmsonSaveClick(DO_SmsParameter sp);
    }
}
