using User.Model;
using System;
using System.Threading.Tasks;

namespace User.IRepository
{
    public interface IUserService
    {
        Task<UserShowModel> GetUserModel(long userid);

        Task<bool> ValidateOldPwd(long userid, string pwd);

        Task<bool> ChangePwd(long userid, string new_pwd);

        Task<bool> ChangePwdByMoible(string mobile, string pwd);

        Task<bool> ChangeNickName(long userid, string nick_name);

        Task<bool> ChangeHeaderPic(long userid, string headerImage);

        Task<bool> CheckMobileIsExist(string mobile);

        Task<bool> RegisterUser(UserRegisterModel model);

        Task<long> GetUserIdRandom();

        Task<bool> ChangeUserIdRandom(long userid);

        Task<long> Login(string user, string pwd);
    }
}
