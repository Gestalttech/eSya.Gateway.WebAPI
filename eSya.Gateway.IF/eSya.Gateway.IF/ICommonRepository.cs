using eSya.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.IF
{
    public interface ICommonRepository
    {
        Task<List<DO_ISDCodes>> GetISDCodes();

        Task<List<DO_ApplicationCodes>> GetApplicationCodesByCodeType(int codeType);

        Task<bool> GetLocationSMSApplicable(int BusinessKey);
    }
}
