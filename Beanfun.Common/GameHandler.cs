using System.Diagnostics;

namespace Beanfun.Common
{
    public static class GameHandler
    {
        /// <summary>
        /// 启动游戏
        /// </summary>
        /// <param name="gamePath">游戏路径</param>
        /// <param name="accountId">账号id</param>
        /// <param name="password">密码</param>
        public static void StartGame(string gamePath, string? accountId, string? password)
        {
            if (BeanfunConst.Instance.ConfigService == null)
                return;

            var config = BeanfunConst.Instance.ConfigService.GetConfig();

            if (config == null)
                return;

            var lrExe = BeanfunConst.Instance.LocaleRemulatorDir;

            if (!File.Exists(lrExe))
                return;


            string commandLine = gamePath;

            if (config.AutoInput && !string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(password))
            {
                commandLine = $"{commandLine} tw.login.maplestory.beanfun.com 8484 BeanFun {accountId} {password}";
            }

            string arg = $"{BeanfunConst.Instance.RlConfigGuid} {commandLine}";

            try
            {
                WindowManager.OpenApp(lrExe, arg);
                if (config.KillStartPalyWindow)
                {
                    //跳过启动窗口
                    WindowManager.CloseMapleStoryStart();
                }

                if (config.KillGamePatcher)
                {
                    //阻止游戏更新
                    WindowManager.StopAutoPatcher();
                }
            }
            catch (Exception e)
            {
                BeanfunConst.Instance.MessageService?.Show($"启动游戏异常 {e.Message}");
            }
        }
    }
}