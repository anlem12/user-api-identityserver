using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserInfo.API.Core
{
    /// <summary>
    /// 站点配置信息
    /// </summary>
    public class SiteConfig
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string client { get; set; }
        /// <summary>
        /// 客户端秘钥
        /// </summary>
        public string secret { get; set; }
        /// <summary>
        /// 上传用户头像保存的物理路径
        /// </summary>
        public string uploadUserHeaderPortraitPhysicsPath { get; set; }
    }
}
