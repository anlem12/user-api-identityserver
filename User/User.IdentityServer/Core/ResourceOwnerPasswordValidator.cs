using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Threading.Tasks;
using User.IRepository;

namespace User.IdentityServer.Core
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserService _userService;

        public ResourceOwnerPasswordValidator(IUserService userService)
        {
            _userService = userService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var userId = await _userService.Login(context.UserName, LoginPassWord(context.Password));
            if (userId != 0)
            {
                context.Result = new GrantValidationResult(userId.ToString(), OidcConstants.AuthenticationMethods.Password);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户名名密码错误");
            }
        }

        private string LoginPassWord(string str)
        {
            return Encrypt.GetMD5(Encrypt.AESEncrypt(Encrypt.GetMD5(str), str));
        }
    }
}
