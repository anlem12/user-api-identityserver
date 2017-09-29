using Enyim.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.DrawingCore;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserInfo.API.Cache;
using UserInfo.API.Core;
using UserInfo.API.FromData;
using User.IRepository;
using User.Model;

namespace UserInfo.API.Controllers
{

    /// <summary>
    /// 用户
    /// </summary>
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private ILogger _logger;
        private IUserService _iuser;
        private SiteConfig _config;
        private UserCache _uCache;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iuser"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        /// <param name="cache"></param>
        public UserController(IUserService iuser, ILoggerFactory loggerFactory, IOptions<SiteConfig> options, IMemcachedClient cache)
        {
            _iuser = iuser;
            _logger = loggerFactory.CreateLogger<UserController>();
            _config = options.Value;
            _uCache = new UserCache(cache);
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="data">用户注册数据</param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(ApiStatusModel<string>), 200)]
        public async Task<ActionResult> Register([FromBody]UserRegisterFromData data)
        {
            ApiStatusModel<string> sysModel = new ApiStatusModel<string>();
            try
            {
                if (data.mobile == null || data.mobile.Length == 0)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "手机号不能为空";
                    return new JsonResult(sysModel);
                }

                if (data.password == null || data.password.Length == 0)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "密码不能为空";
                    return new JsonResult(sysModel);
                }
                if (data.password.Length < 6 || data.password.Length > 16)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "密码（长度6至16位）";
                    return new JsonResult(sysModel);
                }

                if (data.validatecode == null || data.validatecode.Length == 0)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "验证码不能为空";
                    return new JsonResult(sysModel);
                }

                if (await _iuser.CheckMobileIsExist(data.mobile))
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "手机号码已经被注册";
                    return new JsonResult(sysModel);
                }

                string code = await _uCache.GetMobileValidateCode(data.mobile);
                if (code == null)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "验证码过期";
                    return new JsonResult(sysModel);
                }

                if (code != data.validatecode)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "验证码错误";
                    return new JsonResult(sysModel);
                }
                UserRegisterModel entity = new UserRegisterModel();
                entity.LoginName_Mobile = data.mobile;
                entity.nickname = new Random().Next(99999, 999999).ToString();
                entity.UserKey = Guid.NewGuid().ToString().Replace("-", "");
                entity.PassWord = Encrypt.LoginPwd(data.password);
                entity.CreateDate = DateTime.Now;
                entity.RegIP = data.ip;
                entity.UsersID = await _iuser.GetUserIdRandom();
                entity.headerpic = "/content/images/logo1.png";

                if (await _iuser.RegisterUser(entity))
                {
                    await _iuser.ChangeUserIdRandom(entity.UsersID);
                    sysModel.statecode = "ok";
                    sysModel.content = "注册成功";
                    await _uCache.RemoveMobileValidateCode(data.mobile);
                    return new JsonResult(sysModel);
                }
                else
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "注册失败";
                    return new JsonResult(sysModel);
                }
            }
            catch (Exception err)
            {
                _logger.LogError("用户注册:" + err.Message + err.StackTrace);
                sysModel.statecode = "fail";
                sysModel.content = "联系管理员";
                return new JsonResult(sysModel);
            }
        }



        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="data">修改密码</param>
        /// <remarks>
        /// 请求头中需要传入：Authorization:Bearer Token
        /// 
        /// Java Code:
        /// 
        ///     OkHttpClient client = new OkHttpClient();
        ///     MediaType mediaType = MediaType.parse("application/json");
        ///     RequestBody body = RequestBody.create(mediaType, "{\"oldpwd\":\"111111\",\"newpwd\":\"222222\"}");
        ///     Request request = new Request.Builder()
        ///     .url("http://domain/api/user/changepassword")
        ///     .post(body)
        ///     .addHeader("authorization", "Bearer XXXXXXXX")
        ///     .addHeader("content-type", "application/json")
        ///     .addHeader("cache-control", "no-cache")
        ///     .addHeader("postman-token", "33edae10-5ff6-5818-966f-453a09c82477")
        ///     .build();
        ///     Response response = client.newCall(request).execute();
        /// 
        /// 
        /// </remarks>
        /// <response code="401">未授权，请传入access_token</response>
        /// <returns></returns>
        [Route("changepassword")]
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiStatusModel<string>), 200)]
        public async Task<ActionResult> ChangePassWord([FromBody]ChangePassWordFromData data)
        {
            ApiStatusModel<string> status = new ApiStatusModel<string>();
            try
            {
                if (data.newpwd.Trim().Length < 6 || data.newpwd.Trim().Length > 16)
                {
                    status.statecode = "fail";
                    status.content = "密码长度为6至16位";
                    return new JsonResult(status);
                }
                if (data.oldpwd == data.newpwd)
                {
                    status.statecode = "fail";
                    status.content = "旧密码和新密码不能相同";
                    return new JsonResult(status);
                }
                if (!await _iuser.ValidateOldPwd(GetUserId(), Encrypt.LoginPwd(data.oldpwd.Trim())))
                {
                    status.statecode = "fail";
                    status.content = "旧密码不正确";
                    return new JsonResult(status);
                }
                if (await _iuser.ChangePwd(GetUserId(), Encrypt.LoginPwd(data.newpwd.Trim())))
                {
                    status.statecode = "ok";
                    status.content = "密码修改成功";
                    return new JsonResult(status);
                }
                status.statecode = "fail";
                status.content = "密码修改失败";
                return new JsonResult(status);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message + err.StackTrace);

                status.statecode = "error";
                status.content = "联系管理员";
                return new JsonResult(status);
            }
        }

        /// <summary>
        /// 修改昵称
        /// </summary>
        /// <param name="data">昵称（昵称长度为3至10位）</param>
        /// <response code="401">未授权，请传入access_token</response>
        /// <returns></returns>
        [Route("changenick")]
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiStatusModel<string>), 200)]
        public async Task<ActionResult> ChangeNickName([FromBody]ChangeNickNameFromData data)
        {
            ApiStatusModel<string> status = new ApiStatusModel<string>();
            try
            {
                if (data.nickname.Trim().Length < 3 || data.nickname.Trim().Length > 10)
                {
                    status.statecode = "fail";
                    status.content = "昵称长度为3至10位";
                    return new JsonResult(status);
                }
                if (await _iuser.ChangeNickName(GetUserId(), data.nickname.Trim()))
                {
                    status.statecode = "ok";
                    status.content = "昵称修改成功";
                    return new JsonResult(status);
                }
                status.statecode = "fail";
                status.content = "昵称修改失败";
                return new JsonResult(status);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message + err.StackTrace);

                status.statecode = "error";
                status.content = "联系管理员";
                return new JsonResult(status);
            }
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <param name="data">将图片转成base64后上传</param>
        /// <response code="401">未授权，请传入access_token</response>
        /// <returns></returns>
        [Route("changeheadportrait")]
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiStatusModel<string>), 200)]
        public async Task<ActionResult> ChangeHeaderPortrait([FromBody]ChangeHeaderPortraitFromData data)
        {
            ApiStatusModel<string> status = new ApiStatusModel<string>();
            try
            {

                string base64 = data.base64.Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", "");
                string filePath = Base64StringToImage(base64, @"\profile\userheader\" + DateTime.Now.ToString("yyyyMMdd") + "\\", _config.uploadUserHeaderPortraitPhysicsPath);
                if (filePath == "")
                {
                    status.statecode = "fail";
                    status.content = "上传失败,检查头像格式";
                    return new JsonResult(status);
                }
                long userid = GetUserId();
                Thumbnail thm = new Thumbnail();
                if (thm.SetImage(_config.uploadUserHeaderPortraitPhysicsPath + "\\" + filePath))
                {
                    thm.SaveThumbnailImage(250, 250, true);
                }

                if (await _iuser.ChangeHeaderPic(userid, filePath))
                {
                    status.statecode = "ok";
                    status.content = filePath;
                    await _uCache.RemoveUserInfo(userid);
                }
                else
                {
                    status.statecode = "fail";
                    status.content = "上传失败";
                }
                return new JsonResult(status);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message + err.StackTrace);

                status.statecode = "error";
                status.content = "联系管理员";
                return new JsonResult(status);
            }
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="data">找回密码请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [Route("findpassword")]
        [ProducesResponseType(typeof(ApiStatusModel<string>), 200)]
        public async Task<ActionResult> FindPassWord([FromBody]FindPassWordFromData data)
        {
            ApiStatusModel<string> sysModel = new ApiStatusModel<string>();
            try
            {
                if (data.password.Length < 6 || data.password.Length > 16)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "密码（长度6至16位）";
                    return new JsonResult(sysModel);
                }

                string code = await _uCache.GetMobileValidateCode(data.mobile);
                if (code == null)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "验证码过期";
                    return new JsonResult(sysModel);
                }
                if (code == null || code != data.validataCode)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "验证码错误";
                    return new JsonResult(sysModel);
                }

                if (await _iuser.ChangePwdByMoible(data.mobile, Encrypt.LoginPwd(data.password)))
                {
                    sysModel.statecode = "ok";
                    sysModel.content = "修改成功";
                    await _uCache.RemoveMobileValidateCode(data.mobile);
                }
                return new JsonResult(sysModel);
            }
            catch (Exception err)
            {
                _logger.LogError("修改密码:" + err.Message + err.StackTrace);
                sysModel.statecode = "fail";
                sysModel.content = "联系管理员";
                return new JsonResult(sysModel);
            }
        }

        /// <summary>
        /// 获取手机验证码
        /// </summary>
        /// <param name="data">获取验证码请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [Route("mobilevalidatecode")]
        [ProducesResponseType(typeof(ApiStatusModel<string>), 200)]
        public async Task<ActionResult> GetMobileValidateCode([FromBody]MobileValidateCodeFromData data)
        {
            ApiStatusModel<string> sysModel = new ApiStatusModel<string>();
            try
            {
                if (data.mobile == null || data.mobile.Length != 11)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "手机号码不正确";
                    return new JsonResult(sysModel);
                }
                DateTime dtime = DateTime.Now;
                if (!DateTime.TryParse(data.requestTime, out dtime))
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "请求时间格式不正确";
                    return new JsonResult(sysModel);
                }
                TimeSpan tsp = DateTime.Now - dtime;
                if (tsp.TotalMinutes > 10)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "请求时间格,超时";
                    return new JsonResult(sysModel);
                }
                if (Encrypt.GetMD5(_config.client + data.requestTime + _config.secret + data.fromdevice) != data.md5)
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "非法操作";
                    return new JsonResult(sysModel);
                }
                if (data.doaction == "注册")
                {
                    sysModel = await SendValidataCode(data.mobile, "注册验证", "SMS_4735597");
                }
                else if (data.doaction == "找回密码")
                {
                    bool isExist = await _iuser.CheckMobileIsExist(data.mobile);
                    if (!isExist)
                    {
                        sysModel.statecode = "fail";
                        sysModel.content = "账号未注册，请注册";
                        return new JsonResult(sysModel);
                    }
                    sysModel = await SendValidataCode(data.mobile, "乐居乐家", "SMS_4735600");
                }
                else
                {
                    sysModel.statecode = "fail";
                    sysModel.content = "重新获取";
                }
                return new JsonResult(sysModel);
            }
            catch (Exception err)
            {
                _logger.LogError("设备：" + (data != null ? data.fromdevice : "") + "获取验证码:" + err.Message + err.StackTrace);
                sysModel.statecode = "fail";
                sysModel.content = "联系管理员";
                return new JsonResult(sysModel);
            }
        }



        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <remarks>
        /// 请求头中需要传入：Authorization:Bearer Token
        /// 
        /// Java Code:
        /// 
        /// 
        ///     OkHttpClient client = new OkHttpClient();
        ///     Request request = new Request.Builder()
        ///     .url("http://domain/api/user/get")
        ///     .get()
        ///     .addHeader("authorization", "Bearer XXXXXXXXXXXXXXXXXXXXXXXX")
        ///     .addHeader("cache-control", "no-cache")
        ///     .addHeader("postman-token", "d742b31c-7811-40a8-a3df-2a0c3b8bdce4")
        ///     .build();
        ///     Response response = client.newCall(request).execute();
        /// 
        /// 
        /// </remarks>
        ///
        /// <response>
        /// 
        ///     {
        ///     "statecode": "ok",
        ///     "content": {
        ///                 "nickname": "测试一下子就是这",
        ///                 "headportrait": "/profile/userheader/20170525/94afa8c8343148b99bf3a824d0fc2bf5.jpg"
        ///     },
        ///     "errorcode": null
        ///     }
        /// 
        /// 
        /// </response>
        /// <response code="401">未授权，请传入access_token</response>
        [Route("get")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiStatusModel<UserShowModel>), 200)]
        public async Task<ActionResult> GetUserInfo()
        {
            try
            {
                long userid = GetUserId();
                ApiStatusModel<UserShowModel> stasus = new ApiStatusModel<UserShowModel>();

                //object obj = await _uCache.GetUserInfoTest(userid);
                //_logger.LogError(obj.ToString(), obj);

                UserShowModel uEntity = await _uCache.GetUserInfo(userid);
                if (uEntity != null)
                {
                    stasus.content = uEntity;
                    stasus.statecode = "ok";
                    return new JsonResult(stasus);
                }

                uEntity = await _iuser.GetUserModel(userid);
                if (uEntity != null)
                {
                    stasus.content = uEntity;
                    stasus.statecode = "ok";
                    await _uCache.SaveUserInfo(userid, uEntity);
                }
                else
                {
                    stasus.statecode = "fail";
                }

                return new JsonResult(stasus);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message + err.StackTrace);
                ApiStatusModel<string> uEntity = new ApiStatusModel<string>();
                uEntity.statecode = "error";
                uEntity.content = "联系管理员";
                return new JsonResult(uEntity);
            }
        }


        /// <summary>
        /// 将base64转成图片
        /// </summary>
        /// <param name="strbase64"></param>
        /// <param name="filePath">保存的文件夹</param>
        /// <param name="physicsPath">物理路径</param>
        /// <returns></returns>
        private string Base64StringToImage(string strbase64, string filePath, string physicsPath)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(strbase64);
                using (MemoryStream ms = new MemoryStream(arr))
                {
                    using (Bitmap bmp = new Bitmap(ms))
                    {
                        string fp = physicsPath + filePath;// HttpContext.Current.Server.MapPath(filePath);
                        if (!Directory.Exists(fp))
                        {
                            Directory.CreateDirectory(fp);
                        }
                        string fileName = Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
                        fp = fp + "/" + fileName;
                        //bmp.Save(fp, System.Drawing.Imaging.ImageFormat.Jpeg);
                        bmp.Save(fp);
                        bmp.Dispose();
                        ms.Dispose();
                        return filePath.Replace("\\", "/") + fileName;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("base64toimage" + ex.Message);
                return "";
            }
        }

        private async Task<ApiStatusModel<string>> SendValidataCode(string mobile, string signName, string templateCode)
        {

            ApiStatusModel<string> sysModel = new ApiStatusModel<string>();
            int id = new Random().Next(10000, 99999);
            

            string result = "短信api";

            if (result.IndexOf("1,") != -1)
            {
                await _uCache.SaveMobileValidateCode(mobile, id.ToString());
                //ValidateCodeCache.SaveMobileCode(mobile, id);
                sysModel.statecode = "ok";
                sysModel.content = "发送成功";
                return sysModel;
            }
            else
            {
                sysModel.statecode = "fail";
                sysModel.content = result;
                return sysModel;
            }
        }
    }
}
