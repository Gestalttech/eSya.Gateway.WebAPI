using eSya.Gateway.DL.Entities;
using eSya.Gateway.DL.Utility;
using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.DL.Repository
{
    public class ForgotUserPasswordRepository: IForgotUserPasswordRepository
    {
        private readonly IStringLocalizer<ForgotUserPasswordRepository> _localizer;
        public ForgotUserPasswordRepository(IStringLocalizer<ForgotUserPasswordRepository> localizer)
        {
            _localizer = localizer;
        }

        #region Forgot User ID
        public async Task<DO_UserAccount> GetOTPbyMobileNumber(string mobileNo)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {

                    try
                    {

                        DO_UserAccount us = new DO_UserAccount();
                        var user = await db.GtEuusbls.Join(db.GtEuusms,
                            b => b.UserId,
                            u => u.UserId,
                            (b, u) => new { b, u })
                        .Where(w => w.b.MobileNumber.ToUpper().Replace(" ", "") == mobileNo.ToUpper().Replace(" ", "") && w.b.ActiveStatus && w.u.ActiveStatus)
                         .Select(r => new
                         {
                             r.b.MobileNumber,
                             r.u.UserId,
                             r.u.LoginId,
                             r.u.LoginDesc
                         }).FirstOrDefaultAsync();

                        if (user != null)
                        {

                            var userOtp = db.GtEuuotps.Where(x => x.UserId == user.UserId).FirstOrDefault();
                            Random rnd = new Random();
                            var OTP = rnd.Next(100000, 999999).ToString();

                            if (userOtp == null)
                            {
                                var lotp = new GtEuuotp()
                                {
                                    UserId = user.UserId,
                                    Otpnumber = OTP,
                                    Otpsource="SMS OTP",
                                    OtpgeneratedDate = System.DateTime.Now,
                                    UsageStatus = false,
                                    ActiveStatus = true,
                                    FormId = "0",
                                    CreatedBy = user.UserId,
                                    CreatedOn = System.DateTime.Now,
                                    CreatedTerminal = "GTPL"
                                };
                                db.GtEuuotps.Add(lotp);
                                db.SaveChanges();

                            }
                            else
                            {
                                userOtp.Otpnumber = OTP;
                                userOtp.Otpsource = "SMS OTP";
                                userOtp.UsageStatus = false;
                                userOtp.ActiveStatus = true;
                                userOtp.OtpgeneratedDate = System.DateTime.Now;
                                userOtp.ModifiedBy = user.UserId;
                                userOtp.ModifiedOn = System.DateTime.Now;
                                userOtp.ModifiedTerminal = "GTPL";
                            }
                            db.SaveChanges();
                            dbContext.Commit();
                            us.IsSucceeded = true;
                            us.Message = "Mobile Number Validated";
                            us.OTP = OTP;
                        }
                        else
                        {
                            us.IsSucceeded = false;
                            us.Message = "Mobile Number Not Exist";

                        }
                        return us;
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }

                }
            }
        }
        public async Task<DO_UserAccount> ValidateUserbyOTP(string mobileNo, string otp, int expirytime)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var user = await db.GtEuusbls.Join(db.GtEuusms,
                            b => b.UserId,
                            u => u.UserId,
                            (b, u) => new { b, u })
                        .Where(w => w.b.MobileNumber.ToUpper().Replace(" ", "") == mobileNo.ToUpper().Replace(" ", "") && w.b.ActiveStatus && w.u.ActiveStatus)
                         .Select(r => new
                         {
                             r.b.MobileNumber,
                             r.u.UserId,
                             r.u.LoginId,
                             r.u.LoginDesc,

                         }).FirstOrDefaultAsync();

                if (user != null)
                {
                    var validTime = DateTime.Now.AddMinutes(-expirytime);

                    var userOtp = db.GtEuuotps.Where(x => x.UserId == user.UserId).FirstOrDefault();

                    if (userOtp != null)
                    {
                        var expOtp = await db.GtEuuotps
                       .Where(t => t.OtpgeneratedDate >= validTime && (!t.UsageStatus) && t.ActiveStatus)
                       .FirstOrDefaultAsync();
                        if (expOtp == null)
                        {
                            us.SecurityQuestionId = 0;
                            us.IsSucceeded = false;
                            us.Message = "OTP Expired";
                            return us;
                        }

                        if (userOtp.Otpnumber != otp)
                        {
                            us.SecurityQuestionId = 0;
                            us.IsSucceeded = false;
                            us.Message = "In Valid OTP";
                            return us;
                        }
                        if (userOtp.Otpnumber == otp)
                        {
                            us.SecurityQuestionId = 0;
                            us.IsSucceeded = true;
                            us.Message = "Your OTP has Sucessfully Validated";
                            us.LoginID = user.LoginId;
                            us.UserID = user.UserId;
                            us.LoginDesc = user.LoginDesc;
                            userOtp.UsageStatus = true;
                            userOtp.ActiveStatus = false;
                            userOtp.ModifiedOn = System.DateTime.Now;
                            db.SaveChanges();

                        }

                    }

                }
                else
                {
                    us.SecurityQuestionId = 0;
                    us.IsSucceeded = false;
                    us.Message = "Mobile Number Not Exist";
                }

                return us;
            }
        }
        public async Task<DO_UserSecurityQuestions> GetRandomSecurityQuestion(string mobileNo)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {

                    try
                    {

                        DO_UserSecurityQuestions seq = new DO_UserSecurityQuestions();
                        var user = await db.GtEuusbls.Join(db.GtEuusms,
                            b => b.UserId,
                            u => u.UserId,
                            (b, u) => new { b, u })
                        .Where(w => w.b.MobileNumber.ToUpper().Replace(" ", "") == mobileNo.ToUpper().Replace(" ", "") && w.b.ActiveStatus && w.u.ActiveStatus)
                         .Select(r => new
                         {
                             r.b.MobileNumber,
                             r.u.UserId,
                             r.u.LoginId,
                             r.u.LoginDesc
                         }).FirstOrDefaultAsync();

                        if (user != null)
                        {

                            var QuestionsList = db.GtEuussqs.Where(x => x.UserId == user.UserId).ToList();
                            if (QuestionsList != null)
                            {
                                Random random = new Random();
                                int questionIndex = random.Next(QuestionsList.Count);
                                var seqQuestion = QuestionsList[questionIndex];
                                if (seqQuestion != null)
                                {
                                    seq.IsSucceeded = true;
                                    seq.Message = "Mobile Number Validated";
                                    seq.SecurityQuestionId = seqQuestion.SecurityQuestionId;
                                    seq.UserId = seqQuestion.UserId;
                                    seq.QuestionDesc = db.GtEcapcds.Where(x => x.ApplicationCode == seq.SecurityQuestionId).FirstOrDefault().CodeDesc;

                                }
                                else
                                {
                                    seq.IsSucceeded = false;
                                    seq.Message = "Mobile Number Not Exist";
                                }
                               
                            }
                           
                        }
                        else
                        {
                            seq.IsSucceeded = false;
                            seq.Message = "Mobile Number Not Exist";

                        }
                        return seq;
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }

                }
            }
        }
        public async Task<DO_UserAccount> ValidateUserSecurityQuestion(DO_UserSecurityQuestions obj)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var validAns = await db.GtEuussqs.Where(x => x.UserId == obj.UserId && x.SecurityQuestionId==obj.SecurityQuestionId && x.ActiveStatus).FirstOrDefaultAsync();
                string answer = "";
                bool validQuestion = false;
                if (validAns != null)
                {
                    answer= CryptGeneration.Decrypt(validAns.SecurityAnswer);
                    validQuestion = answer.ToUpper().Replace(" ", "") == obj.SecurityAnswer.ToUpper().Replace(" ", "");
                }

                if (validQuestion)
                {
                    var user = await db.GtEuusms.Where(x=>x.ActiveStatus && x.UserId== obj.UserId)
                     .Select(r => new
                     {
                         r.UserId,
                         r.LoginId,
                         r.LoginDesc
                     }).FirstOrDefaultAsync();
                    if(user != null)
                    {
                        us.IsSucceeded = true;
                        us.Message = "Sequrity Question Validated";
                        us.UserID=user.UserId;
                        us.LoginID= user.LoginId;
                        us.LoginDesc= user.LoginDesc;
                    }
                    else
                    {
                        us.IsSucceeded = false;
                        us.Message = "You Entered Wrong Answer";
                        us.UserID = user.UserId;
                    }

                }
                else
                {
                    us.IsSucceeded = false;
                    us.Message = "You Entered Wrong Answer";
                    us.UserID = obj.UserId;
                }
                       
                return us;
            }
        }
        #endregion

        #region Forgot Password 
        
        public async Task<DO_UserAccount> ValidateForgotPasswordOTP(string mobileNo, string otp, int expirytime)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var user = await db.GtEuusbls.Join(db.GtEuusms,
                            b => b.UserId,
                            u => u.UserId,
                            (b, u) => new { b, u })
                           .Join(db.GtEuuspws,
                            bu=>bu.u.UserId,
                            p=>p.UserId,
                           (bu,p)=>new {bu,p })
                        .Where(w => w.bu.b.MobileNumber.ToUpper().Replace(" ", "") == mobileNo.ToUpper().Replace(" ", "") && w.bu.b.ActiveStatus && w.bu.u.ActiveStatus && w.p.ActiveStatus)
                         .Select(r => new
                         {
                             r.bu.b.MobileNumber,
                             r.bu.u.UserId,
                             r.bu.u.LoginId,
                             r.bu.u.LoginDesc,
                             r.p.EPasswd,

                         }).FirstOrDefaultAsync();

                if (user != null)
                {
                    var validTime = DateTime.Now.AddMinutes(-expirytime);

                    var userOtp = db.GtEuuotps.Where(x => x.UserId == user.UserId).FirstOrDefault();

                    if (userOtp != null)
                    {
                        var expOtp = await db.GtEuuotps
                       .Where(t => t.OtpgeneratedDate >= validTime && (!t.UsageStatus) && t.ActiveStatus)
                       .FirstOrDefaultAsync();
                        if (expOtp == null)
                        {
                            us.SecurityQuestionId = 0;
                            us.IsSucceeded = false;
                            us.Message = "OTP Expired";
                            return us;
                        }

                        if (userOtp.Otpnumber != otp)
                        {
                            us.SecurityQuestionId = 0;
                            us.IsSucceeded = false;
                            us.Message = "In Valid OTP";
                            return us;
                        }
                        if (userOtp.Otpnumber == otp)
                        {
                            us.SecurityQuestionId = 0;
                            us.IsSucceeded = true;
                            us.Message = "Your OTP has Sucessfully Validated";
                            us.LoginID = user.LoginId;
                            us.UserID = user.UserId;
                            us.LoginDesc = user.LoginDesc;
                            us.Password = CryptGeneration.Decrypt(Encoding.UTF8.GetString(user.EPasswd)); 
                            userOtp.UsageStatus = true;
                            userOtp.ActiveStatus = false;
                            userOtp.ModifiedOn = System.DateTime.Now;
                            db.SaveChanges();

                        }

                    }

                }
                else
                {
                    us.SecurityQuestionId = 0;
                    us.IsSucceeded = false;
                    us.Message = "Mobile Number Not Exist Or Password Not Created";
                }

                return us;
            }
        }
      
        public async Task<DO_UserAccount> ValidateForgotPasswordSecurityQuestion(DO_UserSecurityQuestions obj)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var validAns = await db.GtEuussqs.Where(x => x.UserId == obj.UserId && x.SecurityQuestionId == obj.SecurityQuestionId && x.ActiveStatus).FirstOrDefaultAsync();
                string answer = "";
                bool validQuestion = false;
                if (validAns != null)
                {
                    answer = CryptGeneration.Decrypt(validAns.SecurityAnswer);
                    validQuestion = answer.ToUpper().Replace(" ", "") == obj.SecurityAnswer.ToUpper().Replace(" ", "");
                }

                if (validQuestion)
                {
                    var user = await db.GtEuusms.Join(db.GtEuuspws,
                            b => b.UserId,
                            u => u.UserId,
                            (b, u) => new { b, u })
                        .Where(x => x.b.ActiveStatus && x.b.UserId == obj.UserId && x.u.UserId==obj.UserId && x.u.ActiveStatus)
                     .Select(r => new
                     {
                         r.b.UserId,
                         r.b.LoginId,
                         r.b.LoginDesc,
                         r.u.EPasswd
                     }).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        us.IsSucceeded = true;
                        us.Message = "Sequrity Question Validated";
                        us.UserID = user.UserId;
                        us.LoginID = user.LoginId;
                        us.LoginDesc = user.LoginDesc;
                        us.Password = CryptGeneration.Decrypt(Encoding.UTF8.GetString(user.EPasswd));
                    }
                    else
                    {
                        us.IsSucceeded = false;
                        us.Message = "You Entered Wrong Answer";
                        us.UserID = user.UserId;
                    }

                }
                else
                {
                    us.IsSucceeded = false;
                    us.Message = "You Entered Wrong Answer";
                    us.UserID = obj.UserId;
                }

                return us;
            }
        }
        #endregion

        #region Change Password Expiration Password
        public async Task<DO_ReturnParameter> GetPasswordExpirationDays(string loginId)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var ds = await db.GtEuusms.Where(x => x.LoginId.ToUpper().Replace(" ", "") == loginId.ToUpper().Replace(" ", "") && x.IsUserAuthenticated && x.ActiveStatus
                    && (!x.BlockSignIn) && (!x.CreatePasswordInNextSignIn)).FirstOrDefaultAsync();
                    var exp = await db.GtEcgwrls.Where(w => w.GwruleId == 1 && w.ActiveStatus)
                                  .FirstOrDefaultAsync();
                    if (ds != null && ds.LastPasswordUpdatedDate != null && exp != null)
                    {

                        DateTime lastPasswordUpdatedDate = ds.LastPasswordUpdatedDate.Value;
                        DateTime currentDate = DateTime.Now.AddDays(1);
                        TimeSpan difference = currentDate - lastPasswordUpdatedDate;
                        int days = difference.Days;
                        int numberOfDays = exp.RuleValue - days;

                        if (numberOfDays < 11)
                        {
                            return new DO_ReturnParameter() { Status = true, StatusCode = "1", Message = "Your Password will Expire in next " + numberOfDays + " days Kindly Reset before Expires", ID = numberOfDays, Key = ds.UserId.ToString() };
                        }
                        else
                        {
                            return new DO_ReturnParameter() { Status = true, StatusCode = "0", Message = "No Password Expiration Rule" };

                        }

                    }
                    else
                    {
                        return new DO_ReturnParameter() { Status = true, StatusCode = "0", Message = "No Password Expiration Rule" };
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> ChangeUserExpirationPassword(DO_ChangeExpirationPassword obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {

                        var user = await db.GtEuusms
                            .Join(db.GtEuuspws,
                                u => new { u.UserId },
                                up => new { up.UserId },
                                (u, up) => new { u, up }).Where(x => x.u.UserId == obj.userID && x.u.ActiveStatus && x.up.UserId == obj.userID)
                                .Select(m => new DO_UserPassword
                                {
                                    UserID = m.u.UserId,
                                    EPasswd = m.up.EPasswd
                                }).FirstOrDefaultAsync();


                        if (user != null)
                        {
                            string existingpassword = string.Empty;
                            string olduserpassword = string.Empty;
                            if (user.EPasswd.Length != 0)
                            {
                                existingpassword = CryptGeneration.Decrypt(Encoding.UTF8.GetString(user.EPasswd));

                                //existingpassword = CryptGeneration.Decrypt(Convert.ToBase64String(user.EPasswd));


                                Byte[] oldpasswordbitmapData = Encoding.UTF8.GetBytes(CryptGeneration.Encrypt(obj.oldpassword));
                                olduserpassword = CryptGeneration.Decrypt(Encoding.UTF8.GetString(oldpasswordbitmapData));

                                if (existingpassword != olduserpassword)
                                {
                                    return new DO_ReturnParameter() { Status = false, StatusCode = "W0022", Message = string.Format(_localizer[name: "W0022"]) };
                                }

                                else
                                {
                                    var usermaster = db.GtEuusms.Where(x => x.UserId == obj.userID).FirstOrDefault();
                                    var passwordmaster = db.GtEuuspws.Where(x => x.UserId == obj.userID).FirstOrDefault();

                                  
                                    var repeat = await db.GtEcgwrls.Where(w => w.GwruleId == 2 && w.ActiveStatus)
                                  .FirstOrDefaultAsync();
                                    var pr = db.GtEcprrls
                                     .Join(db.GtEcaprls,
                                     p => p.ProcessId,
                                     r => r.ProcessId,
                                     (p, r) => new { p, r })
                                    .Where(w => w.p.ProcessId == 4 && w.r.RuleId == 4
                                     && w.p.ActiveStatus && w.r.ActiveStatus)
                                    .Count();

                                    if(repeat!=null && pr > 0)
                                    {
                                      
                                        var pass_history = db.GtEuusphs
                                       .Where(x => x.UserId == obj.userID)
                                       .OrderByDescending(x => x.SerialNumber)
                                        .Take(repeat.RuleValue)
                                         .Select(x => new DO_ChangeExpirationPassword
                                         {
                                            newPassword = CryptGeneration.Decrypt(Encoding.UTF8.GetString(x.EPasswd))
                                          })
                                           .ToList();   

                                        if (pass_history != null)
                                        {
                                            foreach (var p in pass_history)
                                            {
                                                if (p.newPassword == obj.newPassword)
                                                {
                                                    return new DO_ReturnParameter() { Status = false, StatusCode = "W0025", Message = string.Format(_localizer[name: "W0025"]) };
                                                }
                                            }
                                        }

                                    }

                                  

                                    if (usermaster != null && passwordmaster != null)
                                    {
                                        usermaster.LastPasswordUpdatedDate = DateTime.Now;
                                        usermaster.LastActivityDate = System.DateTime.Now;
                                        await db.SaveChangesAsync();
                                    }



                                    passwordmaster.EPasswd = Encoding.UTF8.GetBytes(CryptGeneration.Encrypt(obj.newPassword));
                                    passwordmaster.LastPasswdDate = DateTime.Now;
                                    await db.SaveChangesAsync();
                                    var serialno = db.GtEuusphs.Select(x => x.SerialNumber).DefaultIfEmpty().Max() + 1;
                                    var passhistory = new GtEuusph
                                    {
                                        UserId = obj.userID,
                                        SerialNumber = serialno,
                                        EPasswd = Encoding.UTF8.GetBytes(CryptGeneration.Encrypt(obj.newPassword)),
                                        LastPasswdChangedDate = DateTime.Now,
                                        ActiveStatus = true,
                                        FormId = obj.FormID,
                                        CreatedBy = obj.CreatedBy,
                                        CreatedOn = DateTime.Now,
                                        CreatedTerminal = obj.TerminalID
                                    };
                                    db.GtEuusphs.Add(passhistory);
                                    await db.SaveChangesAsync();
                                    dbContext.Commit();
                                    return new DO_ReturnParameter() { Status = true, StatusCode = "S0010", Message = string.Format(_localizer[name: "S0010"]) };

                                }
                            }
                            else
                            {
                                return new DO_ReturnParameter() { Status = false, StatusCode = "W0023", Message = string.Format(_localizer[name: "W0023"]) };

                            }



                        }
                        else
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0024", Message = string.Format(_localizer[name: "W0024"]) };


                        }
                    }
                    
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        return new DO_ReturnParameter() { Status = false, Message = ex.Message };
                    }
                }

            }
        }

        public async Task<int> GetGatewayRuleValuebyRuleID(int GwRuleId)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();
                int rval = 0;
                var ruleval = await db.GtEcgwrls
                    .Where(w => w.GwruleId == GwRuleId && w.ActiveStatus)
                    .FirstOrDefaultAsync();

                if (ruleval != null)
                {
                    rval = ruleval.RuleValue;
                }
                return rval;


            }
        }
        #endregion
    }
}
