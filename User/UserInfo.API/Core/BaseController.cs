using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace UserInfo.API.Core
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 返回登录用户ID
        /// </summary>
        /// <returns></returns>
        protected long GetUserId()
        {
            var user = from c in User.Claims where c.Type == "sub" select new { c.Value }.Value;
            return Convert.ToInt64(user.FirstOrDefault());
        }
    }
}
