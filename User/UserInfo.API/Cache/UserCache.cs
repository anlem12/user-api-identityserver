using Enyim.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Model;

namespace UserInfo.API.Cache
{
    /// <summary>
    /// 用户缓存
    /// </summary>
    public class UserCache
    {
        private IMemcachedClient _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        public UserCache(IMemcachedClient cache)
        {
            _cache = cache;
        }

        #region

        /// <summary>
        /// 缓存，用户信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task SaveUserInfo(long userid, UserShowModel entity)
        {
            await _cache.AddAsync(GetUserInfoKey(userid), entity, 300);
        }

        /// <summary>
        /// 获取，用户信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<UserShowModel> GetUserInfo(long userid)
        {
            
           
            return await _cache.GetValueAsync<UserShowModel>(GetUserInfoKey(userid));
        }
        /// <summary>
        /// 删除，用户信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task RemoveUserInfo(long userid)
        {
            await _cache.RemoveAsync(GetUserInfoKey(userid));
        }

        private string GetUserInfoKey(long userid)
        {
            return string.Format("/usersystem/userinfo/{0}", userid);
        }

        #endregion


        #region 验证码
        /// <summary>
        /// 缓存，手机验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task SaveMobileValidateCode(string mobile, string code)
        {
            await _cache.AddAsync(GetMobileValidateCodeKey(mobile), code, 30);
        }

        /// <summary>
        /// 获取手机验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task<string> GetMobileValidateCode(string mobile)
        {
            return await _cache.GetValueAsync<string>(GetMobileValidateCodeKey(mobile));
        }
        /// <summary>
        /// 删除手机验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task RemoveMobileValidateCode(string mobile)
        {
            await _cache.RemoveAsync(GetMobileValidateCodeKey(mobile));
        }

        private string GetMobileValidateCodeKey(string mobile)
        {
            return string.Format("/user/mobile/{0}/code", mobile);
        }
        #endregion
    }
}
