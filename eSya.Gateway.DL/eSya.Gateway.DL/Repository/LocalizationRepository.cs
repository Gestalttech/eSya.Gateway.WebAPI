using eSya.Gateway.DL.Entities;
using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                //var lr = db.GtEcltfcs
                //    .GroupJoin(db.GtEcltcds.Where(w => w.Culture == culture),
                //        l => l.ResourceId,
                //        c => c.ResourceId,
                //        (l, c) => new { l, c = c.FirstOrDefault() })
                //    .Where(w => w.l.ResourceName == resourceName
                //                && w.l.ActiveStatus == true)
                //    .Select(x => new DO_LocalizationResource
                //    {
                //        ResourceName = x.l.ResourceName,
                //        Key = x.l.Key,
                //        Value = x.c != null ? x.c.Value : x.l.Value
                //    }).ToListAsync();

                var lr = db.GtEcltfcs.Where(x => x.ResourceName == resourceName && x.ActiveStatus)
                   .GroupJoin(db.GtEcltcds.Where(w => w.Culture == culture),
                       l => l.ResourceId,
                       c => c.ResourceId,
                       (l, c) => new { l, c })
                  .SelectMany(z => z.c.DefaultIfEmpty(),
                  (a, b) => new DO_LocalizationResource
                  {
                      ResourceName = a.l.ResourceName,
                      Key = a.l.Key,
                      Value = b == null ? a.l.Value : b.Value,
                      //Value = x.c != null ? x.c.Value : x.l.Value
                  }).ToList();
                var DistinctKeys = lr.GroupBy(x => x.Key).Select(y => y.First());
                return DistinctKeys.ToList();

               
            }
        }
        public async Task<List<FormControlProperty>> GetFormControlPropertybyUserRole(int userRole , string forminternalID)
        {
            using (var db = new eSyaEnterprise())
            {

                
                var lr = db.GtEuufcls.Where(x => x.UserRole == userRole && x.ActiveStatus)
                 .Join(db.GtEcfmnms.Where(w =>w.FormIntId== forminternalID &&  w.ActiveStatus),
                     l => l.FormId,
                     c => c.FormId,
                     (l, c) => new { l, c }).
                     Join(db.GtEcfmcts.Where(x=>x.ActiveStatus),
                     ll=>ll.l.ControlKey,
                     m=>m.ControlKey,
                     (ll, m) => new { ll,m})
                .Select(r => new FormControlProperty
                {
                    ControlKey = r.ll.l.ControlKey,
                    InternalControlId= r.m.InternalControlId,
                    ControlType=r.m.ControlType,
                    Property=r.m.Property,
                    ActiveStatus=r.ll.l.ActiveStatus,
                }).ToListAsync();
                return await lr;
            }
        }

    }
}
