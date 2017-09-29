using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserInfo.API.FromData
{
    /// <summary>
    /// 修改密码
    /// </summary>
    public class ChangePassWordFromData
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string oldpwd { get; set; }
        /// <summary>
        /// 新密码（长度6至16位）
        /// </summary>
        public string newpwd { get; set; }
    }
    /// <summary>
    /// 修改昵称
    /// </summary>
    public class ChangeNickNameFromData
    {
        /// <summary>
        /// 昵称（昵称长度为3至10位）
        /// </summary>
        public string nickname { get; set; }
    }

    /// <summary>
    /// 修改头像
    /// </summary>
    public class ChangeHeaderPortraitFromData
    {
        /// <summary>
        /// 将图片转成base64后上传(jpg,png)
        /// </summary>
        public string base64 { get; set; }
    }
    /// <summary>
    /// 找回密码请求参数
    /// </summary>
    public class FindPassWordFromData
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 密码（长度6至16位）
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 手机验证码
        /// </summary>
        public string validataCode { get; set; }
    }

    /// <summary>
    /// 获取验证码请求参数
    /// </summary>
    public class MobileValidateCodeFromData
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 动作：注册\找回密码
        /// </summary>
        public string doaction { get; set; }
        /// <summary>
        /// 请求时间：（yyyy-MM-dd HH:mm:ss）（10分钟内有效）
        /// </summary>
        public string requestTime { get; set; }
        /// <summary>
        /// 来源设备：Android\IOS\WEB
        /// </summary>
        public string fromdevice { get; set; }
        /// <summary>
        /// md5(client+请求时间+secret+fromdevice)
        /// </summary>
        public string md5 { get; set; }
    }

    /// <summary>
    /// 用户注册信息
    /// </summary>
    public class UserRegisterFromData
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 密码（长度6至16位）
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string validatecode { get; set; }
        /// <summary>
        /// 用户端IP
        /// </summary>
        public string ip { get; set; }
    }
}
