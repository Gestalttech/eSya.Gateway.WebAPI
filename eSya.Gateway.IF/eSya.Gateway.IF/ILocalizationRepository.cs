using eSya.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.IF
{
    public interface ILocalizationRepository
    {
        Task<List<DO_LocalizationResource>> GetLocalizationResourceString(string culture, string resourceName);
    }
}
