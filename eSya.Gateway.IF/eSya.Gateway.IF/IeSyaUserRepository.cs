using eSya.Gateway.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.IF
{
    public interface IeSyaUserRepository
    {
        Task<DO_UserAccount> ValidateUserPassword(string loginID, string password);
        Task<List<DO_MainMenu>> GeteSyaUserMenulist(int userID);
        Task<DO_ReturnParameter> InsertIntoeSyaUser(DO_eSyaUser obj);
        Task<DO_ReturnParameter> UpdateeSyaUser(DO_eSyaUser obj);
        Task<List<DO_eSyaUser>> GeteSyaUser();
        Task<DO_eSyaUser> GeteSyaUserByUserID(int userID);
        Task<DO_eSyaUser> GeteSyaUserByLoginID(string loginID);

        Task<List<DO_ApplicationCodes>> GetUserTypeByGroup(int userGroup);

        Task<DO_UserAccount> GetBusinessLocation();

    }
}
