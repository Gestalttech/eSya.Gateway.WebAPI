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

    }
}
