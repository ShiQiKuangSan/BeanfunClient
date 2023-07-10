using Beanfun.Api.Models;
using Beanfun.Api.Services;
using Beanfun.Common;
using Beanfun.Common.Models;

using Masuit.Tools.Security;

using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Beanfun.Api
{
    public static class BeanfunApi
    {
        private static Lazy<HKBeanfun>? _hKBeanfun;

        private static Lazy<HKBeanfun> HKBeanfun
        {
            get
            {
                _hKBeanfun ??= new Lazy<HKBeanfun>();

                return _hKBeanfun;
            }
        }

        private static Lazy<TWBeanfun>? _twBeanfun;

        private static Lazy<TWBeanfun> TWBeanfun
        {
            get
            {
                _twBeanfun ??= new Lazy<TWBeanfun>();

                return _twBeanfun;
            }
        }

        public static BaseBeanfunService Client(AccountType type = AccountType.HongKong)
        {
            return type switch
            {
                AccountType.HongKong => HKBeanfun.Value,
                AccountType.TaiWang => TWBeanfun.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }

    public abstract class BaseBeanfunService
    {
        /// <summary>
        /// 获得账户列表
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract Task<BeanfunResult<BeanfunAccountResult>> GetAccountList(string token);

        /// <summary>
        /// 获取动态密码
        /// </summary>
        /// <param name="account">账户</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        public abstract Task<BeanfunResult<string>> GetDynamicPassword(BeanfunAccount account, string token);

        /// <summary>
        /// 获取游戏点数
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract Task<int> GetGamePoints(string token);

        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="newName">新名字</param>
        /// <returns></returns>
        public abstract Task<BeanfunResult> AddAccount(string newName);

        /// <summary>
        /// 更改账户名称
        /// </summary>
        /// <param name="accountId">账户ID</param>
        /// <param name="newName">新名字</param>
        /// <returns></returns>
        public abstract Task<BeanfunResult> ChangeAccountName(string accountId, string newName);

        /// <summary>
        /// 获取web url会员充值
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract string GetWebUrlMemberTopUp(string token);

        /// <summary>
        /// 获取web url成员中心
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract string GetWebUrlMemberCenter(string token);

        /// <summary>
        /// 解密des pkcs5 hex密文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected static string DecrDesPkcs5Hex(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var split = text.Split(",");

            if (!split.Any() || split.Length < 2)
            {
                return "";
            }

            var key = split[1][..8];
            var deVal = split[1][8..];

            return DesTools.Decrypt(deVal, key);
        }


        protected static string GetAccountName(string name)
        {
            var list = name.Split(";");

            StringBuilder builder = new StringBuilder();

            foreach (var item in list)
            {
                var val = item.Replace("&#", "");


                var status = int.TryParse(val, out var num);

                if (!status)
                    continue;

                builder.Append($"%u{string.Format("{0:X2}", num)}");
            }

            if (builder.Length > 0)
            {
                return HttpUtility.UrlDecode(builder.ToString(), Encoding.UTF8);
            }

            return string.Empty;
        }
    }
}