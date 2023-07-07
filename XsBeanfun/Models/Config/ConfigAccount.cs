using Beanfun.Common.Models;

namespace Beanfun.Models.Config
{
    public class ConfigAccount
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// 是否是默认账号
        /// </summary>
        public bool IsDefault { get; set; }
    }
}