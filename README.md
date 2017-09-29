# user-api-identityserver
用户授权系统：使用.netcore2.0基于identityserver4开发

下载代码发布：User.IdentityServer 


用户信息API：使用.netcore2.0，通过授权系统验证权限

包含：登录、注册、找回密码、获取手机验证码、修改密码、修改昵称、修改头像、获取用户信息

下载代码发布：UserInfo.API

在浏览器输入:http://ip:port/swagger/   可以查看整个API文档

表字段

		public long usersid { get; set; }
        public string loginname_mobile { get; set; }
        public string password { get; set; }
        public string userkey { get; set; }
        public string regip { get; set; }
        public string nickname { get; set; }
        public string headerpic { get; set; }
        public DateTime createdate { get; set; }

		