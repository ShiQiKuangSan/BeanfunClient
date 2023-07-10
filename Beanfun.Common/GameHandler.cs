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
        public static async void StartGame(string gamePath, string? accountId, string? password)
        {
            if (BeanfunConst.Instance.ConfigService == null)
                return;

            var config = BeanfunConst.Instance.ConfigService.GetConfig();

            if (config == null)
                return;

            string runParam = gamePath;

            if (config.AutoInput && !string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(password))
            {
                runParam = $"{runParam} tw.login.maplestory.beanfun.com 8484 BeanFun {accountId} {password}";
            }

            string lrExe = BeanfunConst.Instance.LocaleRemulatorDir + "\\LRProc.exe";
            string lrDll = BeanfunConst.Instance.LocaleRemulatorDir + "\\LRHookx64.dll";

            string cmd = $"\"{lrExe}\" \"{lrDll}\" tms {runParam}";

            try
            {
                Process process = new();

                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                await process.StandardInput.WriteAsync(cmd);

                process.WaitForExit();

                process.Close();
            }
            catch (Exception e)
            {
                BeanfunConst.Instance.MessageService?.Show($"启动游戏异常 {e.Message}");
            }
        }
    }
}