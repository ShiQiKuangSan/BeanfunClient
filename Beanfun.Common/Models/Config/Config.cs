﻿namespace Beanfun.Common.Models.Config
{
    public class Config
    {
        /// <summary>
        /// 账号集合
        /// </summary>
        public List<ConfigAccount> Accounts { get; set; } = new List<ConfigAccount>();

        public AccountType StartType { get; set; }

        /// <summary>
        /// 游戏路径
        /// </summary>
        public string? GamePath { get; set; }

        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool RecordActPwd { get; set; }

        /// <summary>
        /// 阻止游戏自动更新
        /// </summary>
        public bool KillGamePatcher { get; set; }

        /// <summary>
        /// 自动跳过Play窗口
        /// </summary>
        public bool KillStartPalyWindow { get; set; }

        /// <summary>
        /// 是否自动输入账号密码
        /// </summary>
        public bool AutoInput { get; set; }
    }
}