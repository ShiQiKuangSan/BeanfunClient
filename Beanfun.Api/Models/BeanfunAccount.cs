namespace Beanfun.Api.Models
{
    public record class BeanfunAccount
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 账号状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// 账号名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
}