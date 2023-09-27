using eSya.Gateway.DL.Entities;
using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.DL.Repository
{
    public class LocalizationRepository : ILocalizationRepository
    {
        public async Task<List<DO_LocalizationResource>> GetLocalizationResourceString(string culture, string resourceName)
        {
            using (var db = new eSyaEnterprise())
            {
                var lr = db.GtEcltfcs
                    .GroupJoin(db.GtEcltcds.Where(w => w.Culture == culture),
                        l => l.ResourceId,
                        c => c.ResourceId,
                        (l, c) => new { l, c = c.FirstOrDefault() })
                    .Where(w => w.l.ResourceName == resourceName
                                && w.l.ActiveStatus == true)
                    .Select(x => new DO_LocalizationResource
                    {
                        ResourceName = x.l.ResourceName,
                        Key = x.l.Key,
                        Value = x.c != null ? x.c.Value : x.l.Value
                    }).ToListAsync();

                return await lr;
            }
        }
    }
}
