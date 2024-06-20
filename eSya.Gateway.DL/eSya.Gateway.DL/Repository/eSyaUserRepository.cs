using eSya.Gateway.DL.Entities;
using eSya.Gateway.DO;
using eSya.Gateway.IF;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.DL.Repository
{
    public class eSyaUserRepository : IeSyaUserRepository
    {
        private readonly IStringLocalizer<eSyaUserRepository> _localizer;
        public eSyaUserRepository(IStringLocalizer<eSyaUserRepository> localizer)
        {
            _localizer = localizer;
        }
        public async Task<DO_UserAccount> ValidateUserPassword(string loginID, string password)
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var lg = await db.GtEuuscgs
                    .Where(w => w.LoginId == loginID
                                && w.ActiveStatus == true)
                    .FirstOrDefaultAsync();

                if (lg != null)
                {
                    if (lg.Password == password)
                    {
                        us.IsSucceeded = true;
                        us.UserID = lg.UserId;
                    }
                    else
                    {
                        us.IsSucceeded = false;
                        return new DO_UserAccount() {IsSucceeded=false, StatusCode = "W0002", Message = string.Format(_localizer[name: "W0002"])};
                       
                    }
                }
                else
                {
                    us.IsSucceeded = false;
                    return new DO_UserAccount() {IsSucceeded=false, StatusCode = "W0001", Message = string.Format(_localizer[name: "W0001"]) };
                    
                }

                return us;
            }
        }

        public async Task<List<DO_MainMenu>> GeteSyaUserMenulist(int userID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    int userGroup = 0, userType = 0;
                    var ug = db.GtEuuscgs.Where(w => w.UserId == userID).FirstOrDefault();
                    if(ug != null)
                    {
                        userGroup = ug.UserGroup;
                        userType = ug.UserType;
                    }

                    var userForms = db.GtEcmnfls
                                    .Join(db.GtEuusgrs,
                                        f => f.MenuKey,
                                        u => u.MenuKey,
                                        (f, u) => new { f, u })
                                    .Where(w =>
                                    //SNO-12
                                    //w.u.UserGroup == userGroup && w.u.UserType == userType
                                    //    && w.u.ActiveStatus &&
                                        w.f.ActiveStatus)
                                    .Select(r => new
                                    {
                                        r.f.MainMenuId,
                                        r.f.MenuItemId,
                                        r.f.FormId
                                    }).ToList();

                    List<DO_SubMenu> userMenu = new List<DO_SubMenu>();
                    foreach (var um in userForms)
                    {
                        userMenu.Add(new DO_SubMenu { MenuItemId = um.MenuItemId });
                        GetSubMenuItem(um.MainMenuId, um.MenuItemId, userMenu);
                    }

                    var menuList = db.GtEcmamns.Where(w => w.ActiveStatus
                                        && userForms.Any(m => m.MainMenuId == w.MainMenuId))
                                   .Select(m => new DO_MainMenu()
                                   {
                                       MainMenuId = m.MainMenuId,
                                       MainMenu = m.MainMenu,
                                       MenuIndex = m.MenuIndex,
                                       l_SubMenu = db.GtEcsbmns.Where(w => w.MainMenuId == m.MainMenuId && w.ActiveStatus == true
                                                    && userMenu.Any(f => f.MenuItemId == w.MenuItemId))
                                        .Select(s => new DO_SubMenu()
                                        {
                                            MainMenuId = s.MainMenuId,
                                            MenuItemId = s.MenuItemId,
                                            MenuItemName = s.MenuItemName,
                                            MenuIndex = s.MenuIndex,
                                            l_FormMenu = db.GtEcmnfls
                                                        .Join(db.GtEcfmnms,
                                                        f => f.FormId,
                                                        i => i.FormId,
                                                        (f, i) => new { f, i })
                                                .Where(w => w.f.MenuItemId == s.MenuItemId && w.f.ActiveStatus == true
                                                         && userForms.Any(f => f.FormId == w.f.FormId))
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
                                                }).ToList(),

                                        }).ToList()
                                   });
                    return await menuList.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DO_SubMenu> GetSubMenuItem(int mainMenuID, int menuItemID, List<DO_SubMenu> userMenu)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                var sm = db.GtEcsbmns.Where(w => w.MainMenuId == mainMenuID
                    && w.ParentId == menuItemID && w.ActiveStatus == true);
                foreach (var s in sm)
                {
                    userMenu.Add(new DO_SubMenu { MenuItemId = s.MenuItemId });
                    GetSubMenuItem(s.MainMenuId, s.MenuItemId, userMenu);
                }
                return userMenu;
            }
        }
        public async Task<DO_ReturnParameter> InsertIntoeSyaUser(DO_eSyaUser obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {

                        bool isExist = await db.GtEuuscgs.Where(a => a.LoginId.Trim().ToUpper() == obj.LoginID.Trim().ToUpper()).CountAsync() > 0;
                        if (isExist)
                        {
                            return new DO_ReturnParameter() { Status = false, StatusCode = "W0003", Message = string.Format(_localizer[name: "W0003"]) };
                        }
                        int maxID = db.GtEuuscgs.Select(c => c.UserId).DefaultIfEmpty(0).Max() + 1;
                        var uscg = new GtEuuscg
                        {
                            UserId = maxID,
                            LoginId = obj.LoginID,
                            Password = obj.Password,
                            LoginDesc = obj.LoginDesc,
                            UserGroup = obj.UserGroup,
                            UserType = obj.UserType,
                            ActiveStatus = obj.ActiveStatus,
                            FormId = obj.FormId,
                            CreatedBy = obj.CreatedBy,
                            CreatedOn = DateTime.Now,
                            CreatedTerminal = obj.TerminalID

                        };
                        db.GtEuuscgs.Add(uscg);

                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter() { Status = true , StatusCode = "S0001", Message = string.Format(_localizer[name: "S0001"]) };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public async Task<DO_ReturnParameter> UpdateeSyaUser(DO_eSyaUser obj)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {

                        var uscg = db.GtEuuscgs.Where(w => w.UserId == obj.UserID).FirstOrDefault();
                        uscg.LoginDesc = obj.LoginDesc;
                        uscg.UserGroup = obj.UserGroup;
                        uscg.UserType = obj.UserType; ;
                        uscg.ActiveStatus = obj.ActiveStatus;
                        uscg.ModifiedBy = obj.UserID;
                        uscg.ModifiedOn = DateTime.Now;
                        uscg.ModifiedTerminal = obj.TerminalID;

                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter() { Status = true, StatusCode = "S0002", Message = string.Format(_localizer[name: "S0002"]) };
                    }
                    catch (DbUpdateException ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public async Task<List<DO_eSyaUser>> GeteSyaUser()
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var ds = db.GtEuuscgs
                        .GroupJoin(db.GtEcapcds,
                        u => new { u.UserGroup},
                        g => new { UserGroup = g.ApplicationCode },
                        (u, g) => new { u, g = g.FirstOrDefault() })
                         .GroupJoin(db.GtEcapcds,
                        ug => new { ug.u.UserType },
                        t => new { UserType = t.ApplicationCode },
                        (ug, t) => new { ug, t = t.FirstOrDefault() })
                        .Select(r => new DO_eSyaUser
                         {
                             UserID = r.ug.u.UserId,
                             LoginID = r.ug.u.LoginId,
                             LoginDesc = r.ug.u.LoginDesc,
                             UserGroup = r.ug.u.UserGroup,
                             UserGroupDesc = r.ug.g.CodeDesc,
                             UserType = r.ug.u.UserType,
                             UserTypeDesc = r.t.CodeDesc,
                             ActiveStatus = r.ug.u.ActiveStatus

                         }).ToListAsync();

                    return await ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DO_eSyaUser> GeteSyaUserByUserID(int userID)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var ds = db.GtEuuscgs
                         .Where(w=>w.UserId == userID)
                         .Select(r => new DO_eSyaUser
                         {
                             UserID = r.UserId,
                             LoginID = r.LoginId,
                             LoginDesc = r.LoginDesc,
                             UserGroup = r.UserGroup,
                             UserType = r.UserType,
                             ActiveStatus = r.ActiveStatus

                         }).FirstOrDefaultAsync();

                    return await ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DO_eSyaUser> GeteSyaUserByLoginID(string loginID)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var ds = db.GtEuuscgs
                         .Where(w => w.LoginId == loginID)
                         .Select(r => new DO_eSyaUser
                         {
                             UserID = r.UserId,
                             LoginID = r.LoginId,
                             LoginDesc = r.LoginDesc,
                             UserGroup = r.UserGroup,
                             UserType = r.UserType,
                             ActiveStatus = r.ActiveStatus

                         }).FirstOrDefaultAsync();

                    return await ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //SNO-1
        //public async Task<List<DO_ApplicationCodes>> GetUserTypeByGroup(int userGroup)
        //{
        //    try
        //    {
        //        using (var db = new eSyaEnterprise())
        //        {
        //            var ut = db.GtEuusgrs
        //                    .Join(db.GtEcapcds,
        //                    u => new { u.UserType },
        //                    a => new { UserType = a.ApplicationCode},
        //                    (u, a) => new { u, a})
        //                 .Select(r => new DO_ApplicationCodes
        //                 {
        //                     ApplicationCode = r.a.ApplicationCode,
        //                     CodeDesc = r.a.CodeDesc,
        //                 }).Distinct().ToListAsync();

        //            return await ut;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<DO_UserAccount> GetBusinessLocation()
        {
            using (var db = new eSyaEnterprise())
            {
                DO_UserAccount us = new DO_UserAccount();

                var ub = await db.GtEcbslns
                            .Where(w => w.ActiveStatus).ToListAsync();

                us.l_BusinessKey = ub.Select(x => new KeyValuePair<int, string>(x.BusinessKey, x.BusinessName + "-" + x.LocationDescription))
                   .ToDictionary(x => x.Key, x => x.Value);

                return us;
            }
        }

    }
}
