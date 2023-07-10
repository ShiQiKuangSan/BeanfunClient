using CommunityToolkit.Mvvm.ComponentModel;

namespace Beanfun.Models.Main
{
    public partial class AccountInfoModel : ObservableObject
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 动态密码
        /// </summary>
        public string PassWord { get; set; } = string.Empty;

        /// <summary>
        /// 乐豆
        /// </summary>
        public string Point { get; set; }

        /// <summary>
        /// 账号状态（正常，锁定）
        /// </summary>
        public string StatusStr { get; set; }


        public bool Status { get; set; }

        public string Sn { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }


    }
}
