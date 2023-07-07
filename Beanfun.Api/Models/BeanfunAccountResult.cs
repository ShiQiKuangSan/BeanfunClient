namespace Beanfun.Api.Models
{
    public class BeanfunAccountResult
    {
        /// <summary>
        /// 账号列表
        /// </summary>
        public List<BeanfunAccount> AccountList { get; set; } = new();

        /// <summary>
        /// 新账号
        /// </summary>
        public bool NewAccount { get; set; } = false;

        /// <summary>
        /// 进阶认证状态
        /// </summary>
        public bool CertStatus { get; set; } = true;

        /// <summary>
        /// 最大创建账号数量
        /// </summary>
        public int MaxActNumber { get; set; } = 0;
    }
}