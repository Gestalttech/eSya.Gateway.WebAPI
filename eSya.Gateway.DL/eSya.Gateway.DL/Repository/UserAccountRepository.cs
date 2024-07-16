using eSya.Gateway.DL.Entities;
using eSya.Gateway.DL.Utility;
using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NG.Gateway.DO.StaticVariables;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using System.Collections;
using static System.Net.WebRequestMethods;

namespace eSya.Gateway.DL.Repository
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IStringLocalizer<UserAccountRepository> _localizer;
        public UserAccountRepository(IStringLocalizer<UserAccountRepository> localizer)
        {
            _localizer = localizer;
        }
        public async Task<DO_UserAccount> ValidateUserPassword(string loginID, string password, int maxUnsuccessfulAttempts, int unLockLoginAfter, int PasswordValidity)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var lg = await db.GtEuusms
                    .Where(w => w.LoginId == loginID)
                    .FirstOrDefaultAsync();

                if (lg != null)
                {
                    if (!lg.ActiveStatus)
                    {
                        //return new DO_UserAccount() { IsSucceeded = false, StatusCode = "W0002", Message = string.Format(_localizer[name: "W0002"]) };

                        us.IsSucceeded = false;
                        us.Message = string.Format(_localizer[name: "W0004"]);
                        us.StatusCode = "W0004";
                        return us;
                    }
                    if (lg.BlockSignIn)
                    {
                        us.IsSucceeded = false;
                        us.Message = string.Format(_localizer[name: "W0005"]);
                        us.StatusCode = "W0005";
                        return us;
                    }
                    if (lg.CreatePasswordInNextSignIn)
                    {
                        us.IsSucceeded = false;
                        us.Message = string.Format(_localizer[name: "W0016"]);
                        us.StatusCode = "W0016";
                        return us;
                    }
                    var validpassword = db.GtEuuspws.Where(x => x.UserId == lg.UserId && x.ActiveStatus).FirstOrDefault();
                    if (validpassword!=null)
                    {
                        string enterpass = CryptGeneration.Decrypt(password);
                        string existingpass = CryptGeneration.Decrypt(Encoding.UTF8.GetString(validpassword.EPasswd));
                        if(existingpass != enterpass)
                        {
                            us.IsSucceeded = false;
                            us.Message = string.Format(_localizer[name: "W0002"]);
                            us.StatusCode = "W0002";
                            return us;

                        }
                        
                    }
                    else
                    {
                        us.IsSucceeded = false;
                        us.Message = string.Format(_localizer[name: "W0015"]);
                        us.StatusCode = "W0015";
                        return us;
                    }

                    //var unLockLoginAfter = await db.GtEupapp.Where(w => w.ParameterId == AppParameter.Password_UnLockLoginAfter && w.ActiveStatus).Select(s => (int)s.ParmValue).DefaultIfEmpty(0).FirstOrDefaultAsync();
                    //var maxUnsuccessfulAttempts = await db.GtEupapp.Where(w => w.ParameterId == AppParameter.Password_MaxUnsuccessfulAttempts && w.ActiveStatus).Select(s => (int)s.ParmValue).DefaultIfEmpty(0).FirstOrDefaultAsync();

                    if (maxUnsuccessfulAttempts > 0)
                    {
                        //SNO-6 resolved
                        if (lg.UnsuccessfulAttempt >= maxUnsuccessfulAttempts)
                        {
                            if (unLockLoginAfter > 0)
                            {
                                if (lg.LoginAttemptDate.HasValue)
                                    if (lg.LoginAttemptDate.Value.AddHours((int)unLockLoginAfter) > DateTime.Now)
                                    {

                                        var waitingHour = (lg.LoginAttemptDate.Value.AddHours((int)unLockLoginAfter) - DateTime.Now);
                                        us.IsSucceeded = false;
                                        us.Message = string.Format(_localizer[name: "W0006"]) + waitingHour.Hours.ToString() +":"+ waitingHour.Minutes.ToString() + string.Format(_localizer[name: "W0007"]) + maxUnsuccessfulAttempts.ToString() + string.Format(_localizer[name: "W0008"]);
                                        return us;
                                    }
                            }
                            else
                            {
                                us.IsSucceeded = false;
                                us.Message = string.Format(_localizer[name: "W0009"]) + maxUnsuccessfulAttempts.ToString() + string.Format(_localizer[name: "W0008"]);
                                return us;
                            }
                        //SNO-6
                    }
                }
                    //SNO-6
                    //if (lg.Password == password)
                    //{
                      //lg.UnsuccessfulLoginAttempts = 0;
                        lg.LastActivityDate = DateTime.Now;
                    //SNO-6
                    //us.ForcePasswordChangeNextSignIn = lg.ForcePasswordChangeNextSignIn;
                    us.BlockSignIn = lg.BlockSignIn;

                        us.IsSucceeded = true;
                        us.UserID = lg.UserId;
                    //SNO-6
                    //us.DoctorID = lg.DoctorId;

                        //if (lg.LastPasswordChangeDate.HasValue)
                        //    us.LastPasswordChangedDay = (DateTime.Now.Date - lg.LastPasswordChangeDate.Value.Date).Days;

                        var ub = db.GtEuusbls
                            .Join(db.GtEcbslns,
                                u => u.BusinessKey,
                                b => b.BusinessKey,
                                (u, b) => new { u, b })
                            .Where(w => w.u.UserId == lg.UserId);

                        us.l_BusinessKey = ub.Select(x => new KeyValuePair<int, string>(x.u.BusinessKey, x.b.BusinessName + "-" + x.b.LocationDescription))
                           .ToDictionary(x => x.Key, x => x.Value);

                        if (ub.Count() > 0)
                        //SNO-6
                        //us.UserType = ub.FirstOrDefault().u.UserType ?? 0;

                        if (ub.Where(w => w.u.AllowMtfy).Count() > 0)
                        {  //SNO-6
                           //us.l_FinancialYear = db.GtEcclcos
                           //    .Where(w => w.FromDate.Date <= System.DateTime.Now.Date)

                            //.Select(x => (int)x.FinancialYear).Distinct().OrderByDescending(o => o).ToList();

                          

                            us.l_FinancialYear = db.GtEcblcls
                             .Where(x => x.ActiveStatus) // Filter active records
                             .Select(x => x.CalenderKey) // Select the CalenderKey
                             .Where(calenderKey => calenderKey.Length > 2) // Ensure the string has more than 2 characters
                             .Select(calenderKey => calenderKey.Substring(2)) // Remove the first two characters
                             .Select(calenderKey => calenderKey.Length > 4 ? calenderKey.Substring(0, 4) : calenderKey) // Truncate after four characters if length > 4
                             .Distinct()
                             .OrderByDescending(calenderKey => calenderKey)
                             .Select(calenderKey => int.Parse(calenderKey)) // Convert to integer
                             .ToList();

                        }
                        else
                        {  

                            us.l_FinancialYear = db.GtEcblcls
                                .Where(x => x.ActiveStatus) // Filter active records
                                .Select(x => x.CalenderKey) // Select the CalenderKey
                                .Where(calenderKey => calenderKey.Length > 2) // Ensure the string has more than 2 characters
                                .Select(calenderKey => calenderKey.Substring(2)) // Remove the first two characters
                                .Select(calenderKey => calenderKey.Length > 4 ? calenderKey.Substring(0, 4) : calenderKey) // Truncate after four characters if length > 4
                                .Distinct()
                                .OrderByDescending(calenderKey => calenderKey)
                                .Select(calenderKey => int.Parse(calenderKey)) // Convert to integer
                                .ToList();

                        }
                    //SNO-6
                    //}
                    else
                        {
                        us.IsSucceeded = false;
                        us.Message = string.Format(_localizer[name: "W0002"]);
                        //SNO-6
                        //if (lg.LoginAttemptDate.HasValue && lg.LoginAttemptDate.Value.Date == DateTime.Now.Date)

                        //lg.UnsuccessfulLoginAttempts += 1;

                        //else
                      
                        //lg.UnsuccessfulLoginAttempts = 1;
                        //lg.LoginAttemptDate = DateTime.Now;
                        //SNO-6
                        //if ((maxUnsuccessfulAttempts - lg.UnsuccessfulLoginAttempts) > 0)
                        //    us.Message += string.Format(_localizer[name: "W0010"])+":" + (maxUnsuccessfulAttempts - lg.UnsuccessfulLoginAttempts).ToString();

                        //else if (maxUnsuccessfulAttempts > 0)
                        //    us.Message += string.Format(_localizer[name: "W0009"]) + maxUnsuccessfulAttempts.ToString() + string.Format(_localizer[name: "W0008"]);
                        //us.StatusCode = "W0009";
                    }

                    await db.SaveChangesAsync();
                }
                else
                {
                    us.IsSucceeded = false;
                    us.Message = string.Format(_localizer[name: "W0001"]);
                    us.StatusCode="W0001";


                }

                return us;
            }
        }

        public async Task<DO_UserAccount> ValidateUserMobileLogin(string mobileNumber)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();
                //SNO-8
                var lg = await db.GtEuusms
                    .Where(w => 
                    //w.MobileNumber == mobileNumber &&
                                w.ActiveStatus == true)
                    .FirstOrDefaultAsync();

                if (lg != null)
                { //SNO-8
                    //if (lg.AllowMobileLogin != null && (bool)lg.AllowMobileLogin)
                    //{
                    //    Random rnd = new Random();
                    //    var OTP = rnd.Next(100000, 999999).ToString();

                    //    us.IsSucceeded = true;
                    //    us.UserID = lg.UserId;
                    //    us.OTP = OTP;

                    //    lg.Otpnumber = OTP;
                    //    lg.OtpgeneratedDate = System.DateTime.Now;
                    //    db.SaveChanges();
                    //}
                    //else
                    //{
                    //    us.IsSucceeded = false;
                    //    us.StatusCode = "100";
                    //}
                }
                else
                {
                    us.IsSucceeded = false;
                    us.StatusCode = "404";
                }

                return us;
            }
        }

        public async Task<DO_UserAccount> ValidateUserMobile(string mobileNumber)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();
                //SNO-9
                var lg = await db.GtEuusms
                    .Where(w => 
                    //w.MobileNumber == mobileNumber &&

                                 w.ActiveStatus == true)
                    .FirstOrDefaultAsync();

                if (lg != null)
                {
                    Random rnd = new Random();
                    var OTP = rnd.Next(100000, 999999).ToString();

                    us.IsSucceeded = true;
                    us.UserID = lg.UserId;
                    us.OTP = OTP;
                    //SNO-9
                    //lg.Otpnumber = OTP;
                    //lg.OtpgeneratedDate = System.DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    us.IsSucceeded = false;
                    us.StatusCode = "404";
                }

                return us;
            }
        }

        public async Task<DO_UserAccount> ValidateUserOTP(string mobileNumber, string otp)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();
                //SNO-10
                var lg = await db.GtEuusms
                    .Where(w =>
                    //w.MobileNumber == mobileNumber &&
                                 w.ActiveStatus == true)
                    .FirstOrDefaultAsync();

                if (lg != null)
                {//SNO-10
                    //if (lg.Otpnumber == otp)
                    //{
                    //    lg.UnsuccessfulLoginAttempts = 0;
                    //    lg.LastActivityDate = DateTime.Now;
                    //    await db.SaveChangesAsync();

                    //    us.IsSucceeded = true;
                    //    us.UserID = lg.UserId;
                    //    us.LoginID = lg.LoginId;

                    //    var ub = db.GtEuusbls
                    //        .Join(db.GtEcbslns,
                    //            u => u.BusinessKey,
                    //            b => b.BusinessKey,
                    //            (u, b) => new { u, b })
                    //        .Where(w => w.u.UserId == lg.UserId);

                    //    us.l_BusinessKey = ub.Select(x => new KeyValuePair<int, string>(x.u.BusinessKey, x.b.LocationDescription))
                    //       .ToDictionary(x => x.Key, x => x.Value);

                    //    if (ub.Where(w => w.u.AllowMtfy).Count() > 0)
                    //    {
                    //        us.l_FinancialYear = db.GtEcclcos
                    //            .Where(w => w.FromDate.Date <= System.DateTime.Now.Date)
                    //            .Select(x => (int)x.FinancialYear).OrderByDescending(o => o).ToList();
                    //    }
                    //    else
                    //    {
                    //        us.l_FinancialYear = db.GtEcclcos
                    //             .Where(w => w.FromDate.Date >= System.DateTime.Now.Date
                    //                && w.TillDate.Date <= System.DateTime.Now.Date)
                    //             .Select(x => (int)x.FinancialYear).OrderByDescending(o => o).ToList();
                    //    }
                    //}
                    //else
                    //{
                    //    us.IsSucceeded = false;
                    //    us.Message = string.Format(_localizer[name: "W0011"]);
                    //    us.StatusCode = "W0011";
                    //}
                }
                else
                {
                    us.IsSucceeded = false;
                    us.Message = string.Format(_localizer[name: "W0011"]);
                    us.StatusCode = "W0011";
                }

                return us;
            }
        }

        public async Task<DO_ReturnParameter> CreateUserPassword(int userID, string password, int passwordRepeatationPolicy)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var lg = await db.GtEuusms
                    .Where(w => w.UserId == userID
                                && w.ActiveStatus == true)
                    .FirstOrDefaultAsync();

                if (lg != null)
                {
                    //var pp = await db.GtEupapp.Where(w => w.ParameterId == AppParameter.Password_VerfiyPreviousPasswordsResetValue && w.ActiveStatus).FirstOrDefaultAsync();
                    //if(pp != null && pp.ParmValue > 0)
                    //{
                    //int takeLastChangePassword = Convert.ToInt32(pp.ParmValue);
                    if (passwordRepeatationPolicy > 0)
                    {
                        int takeLastChangePassword = passwordRepeatationPolicy;
                        var l_ph = await db.GtEuusphs.Where(w => w.UserId == userID && w.ActiveStatus)
                            //SNO-4
                            //.OrderByDescending(o => o.LastPasswordChanged)
                            .Take(takeLastChangePassword)
                            .ToListAsync();
                        //SNO-4
                        //var previousPasswordExists = l_ph.Where(w => w.LastPassword == password).Count();
                        //if (previousPasswordExists > 0)
                        //{
                        //    return new DO_ReturnParameter() { Status = false,StatusCode= "W0012", Message = string.Format(_localizer[name: "W0012"]) };
                        //}
                    }
                    //}
                    //SNO-4
                    //lg.LastPasswordChangeDate = DateTime.Now;
                    //lg.Password = password;
                    //lg.ForcePasswordChangeNextSignIn = false;

                    GtEuusph ph = new GtEuusph
                    {
                        UserId = userID,
                        //SNO-4
                        //LastPassword = password,
                        //LastPasswordChanged = DateTime.Now,
                        ActiveStatus = true,
                        FormId = "0",
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now,
                        CreatedTerminal = "login"
                    };
                    db.GtEuusphs.Add(ph);

                    await db.SaveChangesAsync();
                    return new DO_ReturnParameter() { Status = true };
                }
                else
                {
                    return new DO_ReturnParameter() { Status = false, Message = "Internal Error." };
                }
            }
        }

        public async Task<DO_ReturnParameter> ResetUserPassword(int userID, string oldpassword, string newPassword, int passwordRepeatationPolicy)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var lg = await db.GtEuusms
                    .Where(w => w.UserId == userID
                    //SNO-5
                                //&& w.Password == oldpassword
                                && w.ActiveStatus == true)
                    .FirstOrDefaultAsync();

                if (lg != null)
                {
                    //var pp = await db.GtEupapp.Where(w => w.ParameterId == AppParameter.Password_VerfiyPreviousPasswordsResetValue && w.ActiveStatus).FirstOrDefaultAsync();
                    //if (pp != null && pp.ParmValue > 0)
                    //{
                    //int takeLastChangePassword = Convert.ToInt32(pp.ParmValue);
                    if (passwordRepeatationPolicy > 0)
                    {
                        int takeLastChangePassword = passwordRepeatationPolicy;
                        var l_ph = await db.GtEuusphs.Where(w => w.UserId == userID && w.ActiveStatus)
                            //SNO-5
                                //.OrderByDescending(o => o.LastPasswordChanged)
                                .Take(takeLastChangePassword)
                                .ToListAsync();
                        //SNO-5
                        //var previousPasswordExists = l_ph.Where(w => w.LastPassword == newPassword).Count();
                        //if (previousPasswordExists > 0)
                        //{
                        //    return new DO_ReturnParameter() { Status = false,StatusCode="W0012", Message = string.Format(_localizer[name: "W0012"]) };
                        //}
                    }
                    //}
                    //SNO-5
                    //lg.Password = newPassword;
                    //lg.LastPasswordChangeDate = DateTime.Now;
                    //lg.ForcePasswordChangeNextSignIn = false;

                    GtEuusph ph = new GtEuusph
                    {
                        UserId = userID,
                        //SNO-5
                        //LastPassword = newPassword,
                        //LastPasswordChanged = DateTime.Now,
                        ActiveStatus = true,
                        FormId = "0",
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now,
                        CreatedTerminal = "login"
                    };

                    await db.SaveChangesAsync();
                    return new DO_ReturnParameter { Status = true };
                }
                else
                {
                    return new DO_ReturnParameter { Status = false,StatusCode = "W0013",Message = string.Format(_localizer[name: "W0013"]) }; ;
                }
            }
        }

        public async Task<string> GetUserPassword(int userID)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var lg = await db.GtEuusms
                    .Where(w => w.UserId == userID)
                    .FirstOrDefaultAsync();
                //SNO-7
                //if (lg != null)
                //{
                //    return CryptGeneration.Decrypt(lg.Password);
                //}
                //else
                //{
                //    return null;
                //}
               return "Abdul Rahiman";
            }
        }

        public async Task<string> GetUserNameById(int userId)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var lg = await db.GtEuusms
                    .Where(w => w.UserId == userId)
                    .Select(s => s.LoginDesc).FirstOrDefaultAsync();
                return lg;
            }
        }

        public async Task<List<DO_MainMenu>> GeteSyaMenulist_V1()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var menuList = db.GtEcmamns.Where(w => w.ActiveStatus == true)
                                   .Select(m => new DO_MainMenu()
                                   {
                                       MainMenuId = m.MainMenuId,
                                       MainMenu = m.MainMenu,
                                       MenuIndex = m.MenuIndex,
                                       l_SubMenu = db.GtEcsbmns.Where(w => w.MainMenuId == m.MainMenuId && w.ActiveStatus == true)
                                        .Select(s => new DO_SubMenu()
                                        {
                                            MainMenuId = s.MainMenuId,
                                            MenuItemId = s.MenuItemId,
                                            MenuItemName = s.MenuItemName,
                                            MenuSubGroupName = s.ParentId > 0 ? db.GtEcsbmns.Where(w => w.MenuItemId == s.ParentId).FirstOrDefault().MenuItemName : m.MainMenu,
                                            MenuIndex = s.MenuIndex,
                                            l_FormMenu = db.GtEcmnfls
                                                        .Join(db.GtEcfmnms,
                                                        f => f.FormId,
                                                        i => i.FormId,
                                                        (f, i) => new { f, i })
                                                .Where(w => w.f.MenuItemId == s.MenuItemId && w.f.ActiveStatus == true)
                                                .AsQueryable()
                                                .Select(f => new DO_FormMenu()
                                                {
                                                    FormId = f.f.FormId,
                                                    FormInternalID = f.i.FormIntId,
                                                    FormNameClient = f.f.FormNameClient,
                                                    NavigateUrl = f.i.NavigateUrl,
                                                    Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
                                                    Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
                                                    View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
                                                    FormIndex = f.f.FormIndex,
                                                    MenuKey = f.f.MenuKey
                                                }).OrderBy(o => o.FormIndex).ToList(),

                                        }).OrderBy(o => o.MenuIndex).ToList(),
                                       l_FormMenu = db.GtEcmnfls
                                                        .Join(db.GtEcfmnms,
                                                        f => f.FormId,
                                                        i => i.FormId,
                                                        (f, i) => new { f, i })
                                                .Where(w => w.f.MainMenuId == m.MainMenuId && w.f.MenuItemId == 0 && w.f.FormId > 0 && w.f.ActiveStatus == true)
                                                .AsQueryable()
                                                .Select(f => new DO_FormMenu()
                                                {
                                                    FormId = f.f.FormId,
                                                    FormInternalID = f.i.FormIntId,
                                                    FormNameClient = f.f.FormNameClient,
                                                    NavigateUrl = f.i.NavigateUrl,
                                                    Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
                                                    Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
                                                    View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
                                                    FormIndex = f.f.FormIndex,
                                                    MenuKey = f.f.MenuKey
                                                }).OrderBy(o => o.FormIndex).ToList(),
                                   }).OrderBy(o => o.MenuIndex);
                    return await menuList.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DO_MainMenu>> GeteSyaMenulist_v2()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    List<DO_MainMenu> l_MenuList = new List<DO_MainMenu>();
                    var mainMenus = db.GtEcmamns.Where(w => w.ActiveStatus == true);
                    foreach (var m in mainMenus)
                    {
                        DO_MainMenu do_MainMenu = new DO_MainMenu();
                        do_MainMenu.MainMenuId = m.MainMenuId;
                        do_MainMenu.MainMenu = m.MainMenu;
                        do_MainMenu.MenuIndex = m.MenuIndex;

                        do_MainMenu.l_SubMenu = GeteSyaFormList(m.MainMenuId, 0);

                        do_MainMenu.l_FormMenu = db.GtEcmnfls
                                                .Join(db.GtEcfmnms,
                                                    f => f.FormId,
                                                    i => i.FormId,
                                                    (f, i) => new { f, i })
                                                .Where(w => w.f.MainMenuId == m.MainMenuId && w.f.MenuItemId == 0 && w.f.FormId > 0 && w.f.ActiveStatus == true)
                                                .AsQueryable()
                                                .Select(f => new DO_FormMenu()
                                                {
                                                    FormId = f.f.FormId,
                                                    FormInternalID = f.i.FormIntId,
                                                    FormNameClient = f.f.FormNameClient,
                                                    NavigateUrl = f.i.NavigateUrl,
                                                    Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
                                                    Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
                                                    View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
                                                    FormIndex = f.f.FormIndex,
                                                    MenuKey = f.f.MenuKey
                                                }).OrderBy(o => o.FormIndex).ToList();

                        l_MenuList.Add(do_MainMenu);
                    }
                    return l_MenuList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DO_SubMenu> GeteSyaFormList(int mainMenuID, int parentSubMenuID)
        {
            List<DO_SubMenu> l_SubMenuList = new List<DO_SubMenu>();

            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                var sb = db.GtEcsbmns.Where(w => w.MainMenuId == mainMenuID && w.ParentId == parentSubMenuID && w.ActiveStatus == true);
                foreach (var s in sb)
                {
                    DO_SubMenu do_SubMenu = new DO_SubMenu();
                    do_SubMenu.MenuItemId = s.MenuItemId;
                    do_SubMenu.MenuItemName = s.MenuItemName;
                    do_SubMenu.MenuIndex = s.MenuIndex;
                    do_SubMenu.l_FormMenu = db.GtEcmnfls
                         .Join(db.GtEcfmnms,
                                 f => f.FormId,
                                 i => i.FormId,
                                 (f, i) => new { f, i })
                         .Where(w => w.f.MainMenuId == mainMenuID && w.f.MenuItemId == s.MenuItemId && w.f.FormId > 0 && w.f.ActiveStatus == true)
                         .AsQueryable()
                         .Select(f => new DO_FormMenu()
                         {
                             FormId = f.f.FormId,
                             FormInternalID = f.i.FormIntId,
                             FormNameClient = f.f.FormNameClient,
                             NavigateUrl = f.i.NavigateUrl,
                             Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
                             Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
                             View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
                             FormIndex = f.f.FormIndex,
                             MenuKey = f.f.MenuKey
                         }).OrderBy(o => o.FormIndex).ToList();

                    var l_sb = GeteSyaFormList(mainMenuID, do_SubMenu.MenuItemId);
                    if (l_sb.Count() > 0)
                        do_SubMenu.l_SubMenu = l_sb;

                    l_SubMenuList.Add(do_SubMenu);
                }
            }

            return l_SubMenuList;
        }

        public async Task<DO_UserFormRole> GetFormAction(string navigationURL)
        {
            using (var db = new eSyaEnterprise())
            {
                var lr = db.GtEcfmnms
                    .Where(w => w.NavigateUrl == navigationURL && w.ActiveStatus == true)
                    .AsNoTracking()
                    .Select(x => new DO_UserFormRole
                    {
                        FormID = x.FormId,
                        FormIntID = x.FormIntId,
                        FormName = db.GtEcmnfls.Where(w => w.FormId == x.FormId).FirstOrDefault().FormNameClient,
                        IsView = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 1).Count() > 0,
                        IsInsert = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 2).Count() > 0,
                        IsEdit = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 3).Count() > 0,
                        IsDelete = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 4).Count() > 0,
                        IsPrint = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 5).Count() > 0,
                        IsRePrint = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 6).Count() > 0,
                        IsApprove = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 7).Count() > 0,
                        IsAuthenticate = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 8).Count() > 0,
                        IsGiveConcession = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 9).Count() > 0,
                        IsGiveDiscount = db.GtEcfmals.Where(w => w.FormId == x.FormId && w.ActionId == 10).Count() > 0,
                    }).FirstOrDefaultAsync();

                return await lr;
            }
        }

        public List<DO_SubMenu> GetSubMenuItem(int mainMenuID, int menuItemID, List<DO_SubMenu> userMenu)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                var sm = db.GtEcsbmns.Where(w => w.MainMenuId == mainMenuID
                    && w.ParentId == menuItemID && w.ActiveStatus == true).ToList();
                foreach (var s in sm)
                {
                    userMenu.Add(new DO_SubMenu { MenuItemId = s.MenuItemId });
                    GetSubMenuItem(s.MainMenuId, s.MenuItemId, userMenu);
                }
                return userMenu;
            }
        }

        //OLD
        public async Task<List<DO_MainMenu>> GetUserMenulist_O(int businessKey, int userID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var userForms = db.GtEcmnfls
                                    .Join(db.GtEuusmls,
                                        f => f.MenuKey,
                                        u => u.MenuKey,
                                        (f, u) => new { f, u })
                                    .Where(w => w.u.BusinessKey == businessKey && w.u.UserId == userID
                                        && w.u.ActiveStatus && w.f.ActiveStatus)
                                    .Select(r => new DO_FormMenu
                                    {
                                        MainMenuId = r.f.MainMenuId,
                                        MenuItemId = r.f.MenuItemId,
                                        FormId = r.f.FormId
                                    }).ToList();

                    List<DO_SubMenu> userMenu = new List<DO_SubMenu>();
                    foreach (var um in userForms)
                    {
                        userMenu.Add(new DO_SubMenu { MenuItemId = um.MenuItemId });
                        GetSubMenuItem(um.MainMenuId, um.MenuItemId, userMenu);
                    }

                    List<DO_MainMenu> l_MenuList = new List<DO_MainMenu>();
                    var mainMenus = db.GtEcmamns.Where(w => userForms.Any(m => m.MainMenuId == w.MainMenuId)
                        && w.ActiveStatus == true).OrderBy(o => o.MenuIndex);
                    foreach (var m in mainMenus)
                    {
                        DO_MainMenu do_MainMenu = new DO_MainMenu();
                        do_MainMenu.MainMenuId = m.MainMenuId;
                        do_MainMenu.MainMenu = m.MainMenu;
                        do_MainMenu.MenuIndex = m.MenuIndex;
                        do_MainMenu.l_FormMenu = GetUserSubMenuFormsItem(m.MainMenuId, 0, userMenu, userForms);
                        l_MenuList.Add(do_MainMenu);
                    }
                    return l_MenuList;

                    //var menuList = db.GtEcmamn.Where(w => w.ActiveStatus
                    //                    && userForms.Any(m => m.MainMenuId == w.MainMenuId))
                    //               .Select(m => new DO_MainMenu()
                    //               {
                    //                   MainMenuId = m.MainMenuId,
                    //                   MainMenu = m.MainMenu,
                    //                   MenuIndex = m.MenuIndex,
                    //                   l_SubMenu = db.GtEcsbmn.Where(w => w.MainMenuId == m.MainMenuId && w.ActiveStatus == true
                    //                                && userMenu.Any(f => f.MenuItemId == w.MenuItemId))
                    //                    .Select(s => new DO_SubMenu()
                    //                    {
                    //                        MainMenuId = s.MainMenuId,
                    //                        MenuItemId = s.MenuItemId,
                    //                        MenuItemName = s.MenuItemName,
                    //                        MenuIndex = s.MenuIndex,
                    //                        l_FormMenu = db.GtEcmnfl
                    //                                    .Join(db.GtEcfmnm,
                    //                                    f => f.FormId,
                    //                                    i => i.FormId,
                    //                                    (f, i) => new { f, i })
                    //                            .Where(w => w.f.MenuItemId == s.MenuItemId && w.f.ActiveStatus == true
                    //                                     && userForms.Any(f => f.FormId == w.f.FormId))
                    //                            .AsQueryable()
                    //                            .Select(f => new DO_FormMenu()
                    //                            {
                    //                                FormId = f.f.FormId,
                    //                                FormInternalID = f.i.FormIntId,
                    //                                FormNameClient = f.f.FormNameClient,
                    //                                NavigateUrl = f.i.NavigateUrl,
                    //                                Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
                    //                                Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
                    //                                View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
                    //                                FormIndex = f.f.FormIndex,
                    //                                MenuKey = f.f.MenuKey
                    //                            }).ToList(),

                    //                    }).ToList()
                    //               });
                    //return await menuList.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //NEW
        //public async Task<List<DO_MainMenu>> GetUserMenulist(int businessKey, int userID)
        //{
        //    try
        //    {
        //        using (eSyaEnterprise db = new eSyaEnterprise())
        //        {
        //            var us = db.GtEuubgrs.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.ActiveStatus).FirstOrDefault();

        //            if (us != null)
        //            {
        //                var userForms = db.GtEcmnfls
        //                                .Join(db.GtEuusgrs,
        //                                    f => f.MenuKey,
        //                                    u => u.MenuKey,
        //                                    (f, u) => new { f, u })
        //                                .Where(w => w.u.UserGroup == us.UserGroup
        //                                    //&& w.u.UserType == us.UserType
        //                                    && w.u.UserRole == us.UserRole
        //                                    && w.u.ActiveStatus && w.f.ActiveStatus)
        //                                .Select(r => new DO_FormMenu
        //                                {
        //                                    MainMenuId = r.f.MainMenuId,
        //                                    MenuItemId = r.f.MenuItemId,
        //                                    FormId = r.f.FormId
        //                                }).ToList();



        //                List<DO_SubMenu> userMenu = new List<DO_SubMenu>();
        //                foreach (var um in userForms)
        //                {
        //                    userMenu.Add(new DO_SubMenu { MenuItemId = um.MenuItemId });
        //                    GetSubMenuItem(um.MainMenuId, um.MenuItemId, userMenu);
        //                }

        //                List<DO_MainMenu> l_MenuList = new List<DO_MainMenu>();


        //                //var mainMenus = db.GtEcmamns.Where(w => userForms.Any(m => m.MainMenuId == w.MainMenuId)
        //                //    && w.ActiveStatus == true).OrderBy(o => o.MenuIndex);

        //                // Step 1: Extract MainMenuId values from userForms into a list
        //                var mainMenuIds = userForms.Select(m => m.MainMenuId).ToList();

        //                // Step 2: Use the list of MainMenuId values in the query against GtEcmamns
        //                var mainMenus = db.GtEcmamns
        //                    .Where(w => mainMenuIds.Contains(w.MainMenuId) && w.ActiveStatus == true)
        //                    .OrderBy(o => o.MenuIndex)
        //                    .ToList();


        //                foreach (var m in mainMenus)
        //                {
        //                    DO_MainMenu do_MainMenu = new DO_MainMenu();
        //                    do_MainMenu.MainMenuId = m.MainMenuId;
        //                    do_MainMenu.MainMenu = m.MainMenu;
        //                    do_MainMenu.MenuIndex = m.MenuIndex;
        //                    do_MainMenu.l_FormMenu = GetUserSubMenuFormsItem(m.MainMenuId, 0, userMenu, userForms);
        //                    l_MenuList.Add(do_MainMenu);
        //                }
        //                return l_MenuList;
        //            }
        //        }
        //        return new List<DO_MainMenu>();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public  List<DO_FormMenu> GetUserSubMenuFormsItem(int mainMenuID, int menuItemID, List<DO_SubMenu> userMenu, List<DO_FormMenu> userForms)
        //{
        //    using (eSyaEnterprise db = new eSyaEnterprise())
        //    {
        //        List<DO_FormMenu> l_menuForm = new List<DO_FormMenu>();

        //        // Step 1: Extract MenuItemId values from userMenu into a list
        //        var userMenuItemIds = userMenu.Select(m => m.MenuItemId).ToList();

        //        // Step 2: Use the list of MenuItemId values in the query against GtEcsbmns
        //        var sm = db.GtEcsbmns
        //            .Where(w => userMenuItemIds.Contains(w.MenuItemId) && w.ActiveStatus == true)
        //                    .Select(f => new DO_FormMenu()
        //                    {
        //                        MenuItemId = f.MenuItemId,
        //                        MenuItemName = f.MenuItemName,
        //                        FormIndex = f.MenuIndex,
        //                        ParentId = menuItemID,
        //                    }).OrderBy(o => o.FormIndex).ToList();

        //        // Step 1: Extract FormId values from userForms into a list
        //        var userFormIds = userForms.Select(f => f.FormId).ToList();

        //        // Step 2: Use the list of MenuItemId values in the query against GtEcsbmns

        //        var fm = db.GtEcmnfls
        //           .Join(db.GtEcfmnms,
        //               f => f.FormId,
        //               i => i.FormId,
        //               (f, i) => new { f, i })
        //           .Where(w => w.f.MainMenuId == mainMenuID && w.f.MenuItemId == menuItemID
        //                       && w.f.ActiveStatus && w.i.ActiveStatus
        //                       && userFormIds.Contains(w.f.FormId)
        //                       && w.f.FormId > 0 && w.f.ActiveStatus == true)
        //           .OrderBy(o => o.f.FormIndex)
        //           .AsQueryable()
        //           .Select(f => new DO_FormMenu()
        //           {
        //               // MenuItemId = f.f.MenuItemId,
        //               FormId = f.f.FormId,
        //               FormInternalID = f.i.FormIntId,
        //               FormNameClient = f.f.FormNameClient,
        //               NavigateUrl = f.i.NavigateUrl,
        //               Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
        //               Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
        //               View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
        //               FormIndex = f.f.FormIndex,
        //               MenuKey = f.f.MenuKey,
        //               ParentId = menuItemID,
        //           }).OrderBy(o => o.FormIndex).ToList();

        //        //var fm = db.GtEcmnfls
        //        //    .Join(db.GtEcfmnms,
        //        //        f => f.FormId,
        //        //        i => i.FormId,
        //        //        (f, i) => new { f, i })
        //        //    .Where(w => w.f.MainMenuId == mainMenuID && w.f.MenuItemId == menuItemID
        //        //                && w.f.ActiveStatus && w.i.ActiveStatus
        //        //                && userForms.Any(f => f.FormId == w.f.FormId)
        //        //                && w.f.FormId > 0 && w.f.ActiveStatus == true)
        //        //    .OrderBy(o => o.f.FormIndex)
        //        //    .AsQueryable()
        //        //    .Select(f => new DO_FormMenu()
        //        //    {
        //        //        // MenuItemId = f.f.MenuItemId,
        //        //        FormId = f.f.FormId,
        //        //        FormInternalID = f.i.FormIntId,
        //        //        FormNameClient = f.f.FormNameClient,
        //        //        NavigateUrl = f.i.NavigateUrl,
        //        //        Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
        //        //        Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
        //        //        View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
        //        //        FormIndex = f.f.FormIndex,
        //        //        MenuKey = f.f.MenuKey,
        //        //        ParentId = menuItemID,
        //        //    }).OrderBy(o => o.FormIndex).ToList();

        //        var mf = sm.Union(fm);

        //        foreach (var s in sm)
        //        {
        //            var l_s = GetUserSubMenuFormsItem(mainMenuID, s.MenuItemId, userMenu, userForms);
        //            s.l_FormMenu = l_s;
        //        }

        //        l_menuForm.AddRange(mf.OrderBy(o => o.FormIndex));

        //        return  l_menuForm;
        //    }
        //}

        //NEW
        public async Task<List<DO_MainMenu>> GetUserMenulist(int businessKey, int userID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var us = db.GtEuubgrs.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.ActiveStatus).FirstOrDefault();

                    if (us != null)
                    {
                        var userForms = db.GtEcmnfls
                                        .Join(db.GtEuusgrs,
                                            f => f.MenuKey,
                                            u => u.MenuKey,
                                            (f, u) => new { f, u })
                                        .Where(w => w.u.UserGroup == us.UserGroup
                                            //&& w.u.UserType == us.UserType
                                            && w.u.UserRole == us.UserRole
                                            && w.u.ActiveStatus && w.f.ActiveStatus)
                                        .Select(r => new DO_FormMenu
                                        {
                                            MainMenuId = r.f.MainMenuId,
                                            MenuItemId = r.f.MenuItemId,
                                            FormId = r.f.FormId
                                        }).ToList();

                        List<DO_SubMenu> userMenu = new List<DO_SubMenu>();
                        foreach (var um in userForms)
                        {
                            userMenu.Add(new DO_SubMenu { MenuItemId = um.MenuItemId });
                            GetSubMenuItem(um.MainMenuId, um.MenuItemId, userMenu);
                        }

                        List<DO_MainMenu> l_MenuList = new List<DO_MainMenu>();

                        var main_m = db.GtEcmamns.Where(x => x.ActiveStatus).OrderBy(o => o.MenuIndex).ToList();

                        var mainMenus = main_m.Where(w => userForms.Any(m => m.MainMenuId == w.MainMenuId)).OrderBy(o => o.MenuIndex).ToList();
                        //var mainMenus = db.GtEcmamns.Where(w => userForms.Any(m => m.MainMenuId == w.MainMenuId)
                        //    && w.ActiveStatus == true).OrderBy(o => o.MenuIndex);

                        foreach (var m in mainMenus)
                        {
                            DO_MainMenu do_MainMenu = new DO_MainMenu();
                            do_MainMenu.MainMenuId = m.MainMenuId;
                            do_MainMenu.MainMenu = m.MainMenu;
                            do_MainMenu.MenuIndex = m.MenuIndex;
                            do_MainMenu.l_FormMenu = GetUserSubMenuFormsItem(m.MainMenuId, 0, userMenu, userForms);
                            l_MenuList.Add(do_MainMenu);
                        }
                        return l_MenuList;
                    }
                }
                return new List<DO_MainMenu>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DO_FormMenu> GetUserSubMenuFormsItem(int mainMenuID, int menuItemID, List<DO_SubMenu> userMenu, List<DO_FormMenu> userForms)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                List<DO_FormMenu> l_menuForm = new List<DO_FormMenu>();


                var sm_m = db.GtEcsbmns.Where(w => w.MainMenuId == mainMenuID
                          && w.ParentId == menuItemID && w.ActiveStatus).OrderBy(o => o.MenuIndex).ToList();

                var sm = sm_m
                        .Where(w => userMenu.Any(f => f.MenuItemId == w.MenuItemId))
                        .Select(f => new DO_FormMenu()
                        {
                            MenuItemId = f.MenuItemId,
                            MenuItemName = f.MenuItemName,
                            FormIndex = f.MenuIndex,
                            ParentId = menuItemID,
                        }).OrderBy(o => o.FormIndex).ToList();


                var fm_m = db.GtEcmnfls
                    .Join(db.GtEcfmnms,
                        f => f.FormId,
                        i => i.FormId,
                        (f, i) => new { f, i })
                     .Where(w => w.f.MainMenuId == mainMenuID && w.f.MenuItemId == menuItemID
                                && w.f.ActiveStatus && w.i.ActiveStatus
                                && w.f.FormId > 0 && w.f.ActiveStatus == true)
                    .OrderBy(o => o.f.FormIndex)
                    .ToList();

                var fm= fm_m.Where(w=>userForms.Any(f => f.FormId == w.f.FormId))
                    .OrderBy(o => o.f.FormIndex)
                    .ToList()
                    .Select(f => new DO_FormMenu()
                    {
                        // MenuItemId = f.f.MenuItemId,
                        FormId = f.f.FormId,
                        FormInternalID = f.i.FormIntId,
                        FormNameClient = f.f.FormNameClient,
                        NavigateUrl = f.i.NavigateUrl,
                        Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
                        Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
                        View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
                        FormIndex = f.f.FormIndex,
                        MenuKey = f.f.MenuKey,
                        ParentId = menuItemID,
                    }).OrderBy(o => o.FormIndex).ToList();

                var mf = sm.Union(fm);

                foreach (var s in sm)
                {
                    var l_s = GetUserSubMenuFormsItem(mainMenuID, s.MenuItemId, userMenu, userForms);
                    s.l_FormMenu = l_s;
                }

                l_menuForm.AddRange(mf.OrderBy(o => o.FormIndex));

                return l_menuForm;
            }
        }
        public async Task<DO_UserFormRole> GetFormActionByUser(int businessKey, int userID, string navigationURL)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {

                    //SNO-3 resolved
                    //var us = db.GtEuusrls.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.ActiveStatus).FirstOrDefault();
                    var us = db.GtEuubgrs.Where(x => x.BusinessKey == businessKey && x.UserId == userID && x.ActiveStatus).FirstOrDefault();
                    if (us != null)
                    {

                    
                    var uf = db.GtEuusgrs.Where(w =>
                    w.UserGroup == us.UserGroup &&
                    //&& w.UserType == us.UserType &&
                    w.UserRole == us.UserRole && w.ActiveStatus).
                   Join(db.GtEuusrls.Where(x => x.ActiveStatus),
                   h => h.UserRole,
                   y => y.UserRole,
                   (h, y) => new { h, y })
                  .Select(r => new DO_MenuAction
                  {
                      MenuKey = r.h.MenuKey,
                      ActionId = r.y.ActionId
                  }).ToList();
                    var distinctMenuKey = uf
                    .GroupBy(p => new { p.MenuKey, p.ActionId })
                    .Select(g => g.First())
                    .ToList();

                    var userForms =  db.GtEcfmfds
                       .Join(db.GtEcfmnms,
                           f => f.FormId,
                           d => d.FormId,
                           (f, d) => new { f, d })
                       .Join(db.GtEcmnfls,
                           fd => fd.f.FormId,
                           m => m.FormId,
                           (fd, m) => new { fd, m })
                       .Where(w => w.fd.d.NavigateUrl == navigationURL)
                       .Select(x => new
                       {
                           x.fd.f.FormId,
                           x.fd.d.FormIntId,
                           x.m.FormNameClient,
                           x.m.MenuKey
                       }).ToList();
                    var userFormRoles = userForms.Select(x => new DO_UserFormRole
                    {
                        FormID = x.FormId,
                        FormIntID=x.FormIntId,
                        FormName = x.FormNameClient,
                        IsInsert = uf.Any(w => w.MenuKey == x.MenuKey && w.ActionId == 1),
                        IsEdit = uf.Any(w => w.MenuKey == x.MenuKey && w.ActionId == 2),
                        IsView = uf.Any(w => w.MenuKey == x.MenuKey && w.ActionId == 3),
                        IsDelete = uf.Any(w => w.MenuKey == x.MenuKey && (w.ActionId == 4 || w.ActionId == 7)),
                        IsAuthenticate = uf.Any(w => w.MenuKey == x.MenuKey && w.ActionId == 8)
                    }).FirstOrDefault();


                        //var lr = await db.GtEcfmfds
                        //    .Join(db.GtEcfmnms,
                        //        f => f.FormId,
                        //        d => d.FormId,
                        //        (f, d) => new { f, d })
                        //    .Join(db.GtEcmnfls,
                        //        fd => fd.f.FormId,
                        //        m => m.FormId,
                        //        (fd, m) => new { fd, m })
                        //    .Where(w => w.fd.d.NavigateUrl == navigationURL)
                        //    .Select(x => new DO_UserFormRole
                        //    {
                        //        FormID = x.fd.f.FormId,
                        //        FormIntID = x.fd.d.FormIntId,
                        //        FormName = x.m.FormNameClient,
                        //        IsInsert = uf.ToList().Where(w => w.MenuKey == x.m.MenuKey && w.ActionId == 1).Count() > 0,
                        //        //IsInsert = uf.Where(w => w.MenuKey == x.m.MenuKey && w.ActionId == 1).Count() > 0,
                        //        //IsEdit = uf.Where(w => w.MenuKey == x.m.MenuKey && w.ActionId == 2).Count() > 0,
                        //        //IsView = uf.Where(w => w.MenuKey == x.m.MenuKey && w.ActionId == 3).Count() > 0,
                        //        //IsDelete = uf.Where(w => w.MenuKey == x.m.MenuKey && (w.ActionId == 4 || w.ActionId == 7)).Count() > 0,
                        //        //IsAuthenticate = uf.Where(w => w.MenuKey == x.m.MenuKey && w.ActionId == 8).Count() > 0,
                        //        //IsPrint = uf.Where(w => w.MenuKey == x.m.MenuKey && w.ActionId == 5).Count() > 0,
                        //        //IsRePrint = uf.Where(w => w.MenuKey == x.m.MenuKey && w.ActionId == 6).Count() > 0,
                        //        //IsView = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 1 && w.ActiveStatus).Count() > 0,
                        //        //IsInsert = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 2 && w.ActiveStatus).Count() > 0,
                        //        //IsEdit = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 3 && w.ActiveStatus).Count() > 0,
                        //        //IsDelete = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 4 && w.ActiveStatus).Count() > 0,
                        //        //IsPrint = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 5 && w.ActiveStatus).Count() > 0,
                        //        //IsRePrint = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 6 && w.ActiveStatus).Count() > 0,
                        //        //IsApprove = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 7 && w.ActiveStatus).Count() > 0,
                        //        //IsAuthenticate = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 8 && w.ActiveStatus).Count() > 0,
                        //        //IsGiveConcession = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 9 && w.ActiveStatus).Count() > 0,
                        //        //IsGiveDiscount = db.GtEuusfa.Where(w => w.BusinessKey == businessKey && w.UserId == userID && w.MenuKey == x.m.MenuKey && w.ActionId == 10 && w.ActiveStatus).Count() > 0,
                        //    }).FirstOrDefaultAsync();
                        return userFormRoles;
                    }
                    else
                    {
                        var userFormRoles = new DO_UserFormRole
                        {
                            FormID = 0,
                            FormIntID = null,
                            FormName = null,
                            IsInsert = false,
                            IsEdit = false,
                            IsView = false,
                            IsDelete = false,
                            IsAuthenticate = false
                        };
                        return userFormRoles;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }


        public async Task<DO_UserAccount> GetUserBusinessLocation(int userID)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var ub = await db.GtEuusbls
                    .Join(db.GtEcbslns,
                        u => u.BusinessKey,
                        b => b.BusinessKey,
                        (u, b) => new { u, b })
                    .Where(w => w.u.UserId == userID).ToListAsync();

                us.l_BusinessKey = ub.Select(x => new KeyValuePair<int, string>(x.u.BusinessKey, x.b.BusinessName))
                   .ToDictionary(x => x.Key, x => x.Value);

                return us;
            }
        }


        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "eSya@12345Tabibi247";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public async Task<List<DO_MainMenu>> GeteSyaMenulist()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    List<DO_MainMenu> l_MenuList = new List<DO_MainMenu>();
                    var mainMenus = db.GtEcmamns.Where(w => w.ActiveStatus == true).OrderBy(o => o.MenuIndex);
                    foreach (var m in mainMenus)
                    {
                        DO_MainMenu do_MainMenu = new DO_MainMenu();
                        do_MainMenu.MainMenuId = m.MainMenuId;
                        do_MainMenu.MainMenu = m.MainMenu;
                        do_MainMenu.MenuIndex = m.MenuIndex;
                        do_MainMenu.l_FormMenu = GetSubMenuFormsItem(m.MainMenuId, 0);
                        //do_MainMenu.l_SubMenu = GeteSyaFormList(m.MainMenuId, 0);

                        //do_MainMenu.l_FormMenu = db.GtEcmnfl
                        //                        .Join(db.GtEcfmnm,
                        //                            f => f.FormId,
                        //                            i => i.FormId,
                        //                            (f, i) => new { f, i })
                        //                        .Where(w => w.f.MainMenuId == m.MainMenuId && w.f.MenuItemId == 0 && w.f.FormId > 0 && w.f.ActiveStatus == true)
                        //                        .AsQueryable()
                        //                        .Select(f => new DO_FormMenu()
                        //                        {
                        //                            FormId = f.f.FormId,
                        //                            FormInternalID = f.i.FormIntId,
                        //                            FormNameClient = f.f.FormNameClient,
                        //                            NavigateUrl = f.i.NavigateUrl,
                        //                            Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
                        //                            Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
                        //                            View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
                        //                            FormIndex = f.f.FormIndex,
                        //                            MenuKey = f.f.MenuKey
                        //                        }).OrderBy(o => o.FormIndex).ToList();

                        l_MenuList.Add(do_MainMenu);
                    }
                    return l_MenuList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DO_FormMenu> GetSubMenuFormsItem(int mainMenuID, int menuItemID)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                List<DO_FormMenu> l_menuForm = new List<DO_FormMenu>();

                var sm = db.GtEcsbmns
                        .Where(w => w.MainMenuId == mainMenuID
                            && w.ParentId == menuItemID
                            && w.ActiveStatus == true)
                        .OrderBy(o => o.MenuIndex)
                        .Select(f => new DO_FormMenu()
                        {
                            MenuItemId = f.MenuItemId,
                            MenuItemName = f.MenuItemName,
                            FormIndex = f.MenuIndex,
                            ParentId = menuItemID,
                        }).OrderBy(o => o.FormIndex).ToList();

                var fm = db.GtEcmnfls
                    .Join(db.GtEcfmnms,
                        f => f.FormId,
                        i => i.FormId,
                        (f, i) => new { f, i })
                    .Where(w => w.f.MainMenuId == mainMenuID
                            && w.f.MenuItemId == menuItemID
                            && w.f.FormId > 0
                            && w.f.ActiveStatus
                            && w.i.ActiveStatus)
                    .OrderBy(o => o.f.FormIndex)
                    .AsQueryable()
                    .Select(f => new DO_FormMenu()
                    {
                        // MenuItemId = f.f.MenuItemId,
                        FormId = f.f.FormId,
                        FormInternalID = f.i.FormIntId,
                        FormNameClient = f.f.FormNameClient + (f.i.FormDescription != "Standard" ? (" - " + f.i.FormDescription) : ""),
                        NavigateUrl = f.i.NavigateUrl,
                        Area = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[0],
                        Controller = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[1],
                        View = f.i.NavigateUrl.Split('/', StringSplitOptions.None)[2],
                        FormIndex = f.f.FormIndex,
                        MenuKey = f.f.MenuKey,
                        ParentId = menuItemID,
                    }).OrderBy(o => o.FormIndex).ToList();

                var mf = sm.Union(fm);

                foreach (var s in sm)
                {
                    var l_s = GetSubMenuFormsItem(mainMenuID, s.MenuItemId);
                    s.l_FormMenu = l_s;
                }

                l_menuForm.AddRange(mf.OrderBy(o => o.FormIndex));

                return l_menuForm;
            }
        }

        #region OTP Process
        public async Task<DO_UserAccount> ValidateCreateUserOTP(int userId, string otp)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var lg = await db.GtEuusms.Join(db.GtEuuotps,
                     u => u.UserId,
                     f => f.UserId,
                     (u, f) => new { u, f })
                 .Where(w => w.u.CreatePasswordInNextSignIn && w.u.UserId==userId && w.u.ActiveStatus && (!w.f.UsageStatus) && w.f.ActiveStatus)
                 .Select(r => new
                 {
                     r.f.Otpnumber,
                     r.u.UserId,
                     r.u.LoginId,
                     r.u.LoginDesc
                    
                 }).FirstOrDefaultAsync();

                if (lg != null)
                {
                    if (lg.Otpnumber == otp)
                    {
                        us.IsSucceeded = true;
                        us.Message = "Your OTP has Sucessfully Validated";
                        us.LoginID = lg.LoginId;
                        us.UserID = lg.UserId;
                        us.LoginDesc = lg.LoginDesc;
                    }
                    else
                    {
                        us.IsSucceeded = false;
                        us.Message = "Invalid OTP";
                        us.UserID = lg.UserId;
                    }
                }
                else
                {
                    us.IsSucceeded = false;
                    us.Message = "Invalid OTP";
                }

                return us;
            }
        }
        #endregion

        #region Change Password
        public async Task<DO_ReturnParameter> CreateUserPasswordINNextSignIn(int userId, string password)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    byte[] Epass = Encoding.UTF8.GetBytes(CryptGeneration.Encrypt(password));

                    var userexists = db.GtEuusms.Where(x => x.UserId == userId && x.ActiveStatus == true && x.CreatePasswordInNextSignIn).FirstOrDefault();
                    if (userexists != null)
                    {
                        var PasswordExist = db.GtEuuspws.Where(x => x.UserId == userId).FirstOrDefault();
                        if (PasswordExist == null)
                        {
                            var _pas = new GtEuuspw()
                            {
                                UserId = userId,
                                EPasswd = Epass,
                                LastPasswdDate = DateTime.Now,
                                ActiveStatus = true,
                                FormId = "0",
                                CreatedBy = userId,
                                CreatedOn = DateTime.Now,
                                CreatedTerminal = "GTPL"

                            };
                            db.GtEuuspws.Add(_pas);
                            db.SaveChanges();


                            var serialno = db.GtEuusphs.Select(x => x.SerialNumber).DefaultIfEmpty().Max() + 1;
                            var passhistory = new GtEuusph
                            {
                                UserId = userId,
                                SerialNumber = serialno,
                                EPasswd = Encoding.UTF8.GetBytes(CryptGeneration.Encrypt(password)),
                                LastPasswdChangedDate = DateTime.Now,
                                ActiveStatus = true,
                                FormId = "0",
                                CreatedBy = userId,
                                CreatedOn = DateTime.Now,
                                CreatedTerminal = "GTPL"
                            };
                            db.GtEuusphs.Add(passhistory);
                            db.SaveChanges();
                            userexists.CreatePasswordInNextSignIn = false;
                            db.SaveChanges();

                            var otppw = db.GtEuuotps.Where(x => x.UserId == userId && x.ActiveStatus).FirstOrDefault();
                            if (otppw != null)
                            {
                                otppw.ActiveStatus = false;
                                otppw.UsageStatus = true;
                                otppw.ModifiedOn = DateTime.Now;
                                otppw.ModifiedBy = userId;
                                otppw.ModifiedTerminal = "GTPL";
                            }
                            db.SaveChanges();

                            dbContext.Commit();
                            return new DO_ReturnParameter() { Status = true, StatusCode = "S0003", Message = string.Format(_localizer[name: "S0003"]),ID= userId};

                        }
                        else
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0014", Message = string.Format(_localizer[name: "W0014"]) };
                        }


                    }
                    else
                    {
                        return new DO_ReturnParameter() { Status = false, StatusCode = "W0014", Message = string.Format(_localizer[name: "W0014"]) };

                    }
                }
            }
        }
        public async Task<DO_ReturnParameter> ChkIsCreatePasswordInNextSignIn(string loginId)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var ds = await db.GtEuusms.Where(x => x.LoginId == loginId && x.CreatePasswordInNextSignIn && x.ActiveStatus).FirstOrDefaultAsync();
                    if (ds != null)
                    {
                        return new DO_ReturnParameter() { Status = true, StatusCode = "1", ID = ds.UserId, Key = ds.LoginDesc };
                    }
                    else
                    {
                        return new DO_ReturnParameter() { Status = true, StatusCode = "0" };
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region User Security Question
        public async Task<DO_ReturnParameter> InsertUserSecurityQuestion(DO_UserSecurityQuestions obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {            
                            var QId = db.GtEuussqs.Where(x => x.SecurityQuestionId==obj.SecurityQuestionId && x.UserId==obj.UserId
                            && x.EffectiveFrom==obj.EffectiveFrom).FirstOrDefault();
                    if (QId != null)
                    {
                        return new DO_ReturnParameter() { Status = false, StatusCode = "W0017", Message = string.Format(_localizer[name: "W0017"]) };

                    }
                    else
                    {
                        var Ques = new GtEuussq
                        {
                            UserId = obj.UserId,
                            SecurityQuestionId = obj.SecurityQuestionId,
                            EffectiveFrom = DateTime.Now,
                            SecurityAnswer = CryptGeneration.Encrypt(obj.SecurityAnswer),
                            EffectiveTill = obj.EffectiveTill,
                            ActiveStatus = true,
                            FormId = obj.FormID,
                            CreatedBy = obj.UserId,
                            CreatedOn = DateTime.Now,
                            CreatedTerminal = obj.TerminalID
                        };
                        db.GtEuussqs.Add(Ques);
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter() { Status = true, StatusCode = "S0004", Message = string.Format(_localizer[name: "S0004"]) };

                    }
                           
                }
            }
        }
        #endregion

        #region Get User ID
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
                                userOtp.Otpnumber= OTP;
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
        public async Task<DO_UserAccount> ValidateUserbyOTP(String mobileNo, string otp, int expirytime)
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

                    if (userOtp!=null)
                    {
                        var expOtp = await db.GtEuuotps
                       .Where(t =>  t.OtpgeneratedDate >= validTime && (!t.UsageStatus))
                       .FirstOrDefaultAsync();
                        if(expOtp == null)
                        {
                            us.IsSucceeded = false;
                            us.Message = "OTP Expired";
                            return us;
                        }
                        
                        if (userOtp.Otpnumber != otp)
                        {
                            us.IsSucceeded = false;
                            us.Message = "In Valid OTP";
                            return us;
                        }
                        if (userOtp.Otpnumber == otp)
                        {
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
                    us.IsSucceeded = false;
                    us.Message = "Mobile Number Not Exist";
                }

                return us;
            }
        }
        #endregion
       
    }
}
