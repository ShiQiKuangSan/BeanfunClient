using IWshRuntimeLibrary;

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

            if (!System.IO.File.Exists(lrExe))
                return;

            var gamePathLnkPath = BeanfunConst.Instance.GamePathLnkPath;

            if (string.IsNullOrEmpty(gamePathLnkPath))
                return;

            string commandLine = gamePath;

            if (config.AutoInput && !string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(password))
            {
                commandLine = $"{commandLine} tw.login.maplestory.beanfun.com 8484 BeanFun {accountId} {password}";
            }

            string arg = $"{BeanfunConst.Instance.RlConfigGuid} {commandLine}";

            try
            {

                if (!System.IO.File.Exists(gamePathLnkPath))
                {
                    CreateLnk(gamePath, lrExe, arg);
                }

                WindowManager.OpenApp(gamePathLnkPath);

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


        private static void CreateLnk(string gamePath,string lRProcPath,string arg)
        {
            string shortcutPath = BeanfunConst.Instance.GamePathLnkPath ?? Path.Combine($"{Environment.CurrentDirectory}\\xs-data\\", $"BeanfunMaplestory.lnk");

            WshShell shell = new ();
            //创建快捷方式对象
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            //指定目标路径
            shortcut.TargetPath = lRProcPath;
            shortcut.Arguments = arg;
            //设置起始位置
            shortcut.WorkingDirectory = Path.GetDirectoryName(gamePath);
            //设置图标路径
            shortcut.IconLocation = gamePath;
            //保存快捷方式
            shortcut.Save();
        }
    }
}