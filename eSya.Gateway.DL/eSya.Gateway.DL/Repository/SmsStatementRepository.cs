﻿using eSya.Gateway.DL.Entities;
using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NG.Gateway.DO;

namespace eSya.Gateway.DL.Repository
{
    public class SmsStatementRepository : ISmsStatementRepository
    {
        public async Task<List<DO_SmsStatement>> GetSmsStatementByForm(DO_SmsParameter sp)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var fs = await db.GtEcsmshes
                        .Where(w => w.FormId == sp.FormID && w.TeventId == sp.TEventID
                                    && w.ActiveStatus == true)
                        .Select(r => new DO_SmsStatement
                        {
                            SMSID = r.Smsid,
                            SMSDescription = r.Smsdescription,
                            SMSStatement = r.Smsstatement,
                            l_SmsParam = r.GtEcsmsds.Where(w => w.ParmAction && w.ActiveStatus)
                                         .Select(d => new DO_SmsParam
                                         {
                                             ParameterID = d.ParameterId,
                                             ParmAction = d.ParmAction
                                         }).ToList(),
                            l_SmsRecipient = r.GtEcsmsrs.Where(w => w.BusinessKey == sp.BusinessKey && w.ActiveStatus)
                                        .Select(x => new DO_SmsRecipient
                                        {
                                            MobileNumber = x.MobileNumber,
                                            RecipientName = x.RecipientName,
                                            Remarks = x.Remarks
                                        }).ToList()
                        }).ToListAsync();

                    foreach (var s in fs)
                    {
                        foreach (var p in s.l_SmsParam)
                        {
                            int id = 0;
                            if (p.ParameterID == (int)smsParams.Direct)
                            {
                                p.MobileNumber = sp.MobileNumber;
                                p.Name = sp.Name;
                            }

                            if (p.ParameterID == (int)smsParams.User)
                                id = sp.UserID;
                            if (id > 0)
                            {
                                var ms = await GetMasterDetail(p.ParameterID, id);
                                p.MobileNumber = ms.MobileNumber;
                                p.ID = ms.ID;
                                p.Name = ms.Name;
                            }
                        }
                    }

                    return fs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DO_SmsStatement>> GetSmsStatementById(DO_SmsParameter sp)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var fs = await db.GtEcsmshes
                        .Where(w => w.Smsid == sp.SMSID
                                    && w.ActiveStatus == true)
                        .Select(r => new DO_SmsStatement
                        {
                            SMSID = r.Smsid,
                            SMSDescription = r.Smsdescription,
                            SMSStatement = r.Smsstatement,
                            l_SmsParam = r.GtEcsmsds.Where(w => w.ParmAction && w.ActiveStatus)
                                         .Select(d => new DO_SmsParam
                                         {
                                             ParameterID = d.ParameterId,
                                             ParmAction = d.ParmAction
                                         }).ToList(),
                            l_SmsRecipient = r.GtEcsmsrs.Where(w => w.BusinessKey == sp.BusinessKey && w.ActiveStatus)
                                        .Select(x => new DO_SmsRecipient
                                        {
                                            MobileNumber = x.MobileNumber,
                                            RecipientName = x.RecipientName,
                                            Remarks = x.Remarks
                                        }).ToList()
                        }).ToListAsync();

                    foreach (var s in fs)
                    {
                        foreach (var p in s.l_SmsParam)
                        {
                            int id = 0;
                            if (p.ParameterID == (int)smsParams.Direct)
                            {
                                p.MobileNumber = sp.MobileNumber;
                                p.Name = sp.Name;
                            }

                            if (p.ParameterID == (int)smsParams.User)
                                id = sp.UserID;
                            if (id > 0)
                            {
                                var ms = await GetMasterDetail(p.ParameterID, id);
                                p.MobileNumber = ms.MobileNumber;
                                p.ID = ms.ID;
                                p.Name = ms.Name;
                            }
                        }
                    }

                    return fs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<DO_Master> GetMasterDetail(int type, int id)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_Master? us = new DO_Master();

                if (type == (int)smsParams.User)
                {
                    us = await db.GtEuusms.Join(db.GtEuusbls,
                           u => u.UserId,
                           b => b.UserId,
                           (u, b) => new { u, b }).Where(x => x.u.UserId == id).
                           Select(r => new DO_Master
                           {
                               MobileNumber = r.b.Isdcode.ToString() + r.b.MobileNumber,
                               ID = r.u.LoginId,
                               Name = r.u.LoginDesc
                           }).FirstOrDefaultAsync();


                    //us = await db.GtEuusms
                    //   .Where(w => w.UserId == id)
                    //   .Select(r => new DO_Master
                    //   {
                    //       //SNO-11
                    //       //MobileNumber = r.Isdcode.ToString() + r.MobileNumber,
                    //       MobileNumber="ABDUL RAHIMAN",
                    //       ID = r.LoginId,
                    //       Name = r.LoginDesc
                    //   })
                    //   .FirstOrDefaultAsync();
                }

                return us;
            }
        }

        public async Task<bool> Insert_SmsLog(DO_SMSLog obj)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    GtEcsmsl l = new GtEcsmsl
                    {
                        SendDateTime = DateTime.Now,
                        ReferenceKey = obj.ReferenceKey,
                        MobileNumber = obj.MobileNumber,
                        Smsstatement = obj.SMSStatement,
                        MessageType = obj.MessageType,
                        SendStatus = obj.SendStatus,
                        RequestMessage = obj.RequestMessage,
                        ResponseMessage = obj.ResponseMessage,
                        ActiveStatus = true
                    };

                    await db.GtEcsmsls.AddAsync(l);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public async Task<bool> Insert_SmsReminderLog(DO_SmsReminder obj)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    GtEcsmsc l = new GtEcsmsc
                    {
                        ReminderType = obj.ReminderType,
                        Smsid = obj.SmsId,
                        ReferenceKey = obj.ReferenceKey,
                        SendStatus = obj.SendStatus,
                        CreatedOn = DateTime.Now,
                        ActiveStatus = true
                    };

                    await db.GtEcsmscs.AddAsync(l);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public async Task<List<DO_SmsStatement>> GetSmsonSaveClick(DO_SmsParameter sp)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var fs = await db.GtEcsmshes.Join(db.GtSmslocs,
                           u => u.FormId,
                           b => b.FormId,
                           (u, b) => new { u, b })
                        .Where(w => w.u.FormId == sp.FormID && w.u.TeventId == sp.TEventID
                                    && w.u.ActiveStatus == true && w.b.BusinessKey == sp.BusinessKey)
                        .Select(r => new DO_SmsStatement
                        {
                            SMSID = r.u.Smsid,
                            SMSDescription = r.u.Smsdescription,
                            SMSStatement = r.u.Smsstatement,
                            l_SmsParam = r.u.GtEcsmsds.Where(w => w.ParmAction && w.ActiveStatus)
                                         .Select(d => new DO_SmsParam
                                         {
                                             ParameterID = d.ParameterId,
                                             ParmAction = d.ParmAction
                                         }).ToList(),
                            l_SmsRecipient = r.u.GtEcsmsrs.Where(w => w.BusinessKey == sp.BusinessKey && w.ActiveStatus)
                                        .Select(x => new DO_SmsRecipient
                                        {
                                            MobileNumber = x.MobileNumber,
                                            RecipientName = x.RecipientName,
                                            Remarks = x.Remarks
                                        }).ToList()
                        }).ToListAsync();

                    foreach (var s in fs)
                    {
                        foreach (var p in s.l_SmsParam)
                        {
                            int id = 0;
                            if (p.ParameterID == (int)smsParams.Direct)
                            {
                                p.MobileNumber = sp.MobileNumber;
                                p.Name = sp.Name;
                            }

                            if (p.ParameterID == (int)smsParams.User)
                                id = sp.UserID;
                            if (id > 0)
                            {
                                var ms = await GetMasterDetail(p.ParameterID, id);
                                p.MobileNumber = ms.MobileNumber;
                                p.ID = ms.ID;
                                p.Name = ms.Name;
                            }
                        }
                    }

                    return fs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<DO_SmsProviderCredential> SmsProviderCredential(int BusinessKey)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var bk = db.GtEcsm91s.Where(x => x.BusinessKey == BusinessKey
                    && DateTime.Now.Date >= x.EffectiveFrom.Date
                           && DateTime.Now.Date <= (x.EffectiveTill ?? DateTime.Now).Date
                    && x.ActiveStatus)
                        .Select(r => new DO_SmsProviderCredential
                        {
                            SMSProviderAPI = r.Api,
                            SMSProviderUserID = eSyaCryptGeneration.Decrypt(r.UserId),
                            SMSProviderPassword = eSyaCryptGeneration.Decrypt(r.Password),
                            SMSProviderSenderID = r.SenderId
                        }).FirstOrDefaultAsync();

                    return await bk;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}