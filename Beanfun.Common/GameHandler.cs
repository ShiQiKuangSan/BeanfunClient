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

            //LRProc.exe LRHookx64.dll tms F:\MapleStory\MapleStory.exe
            //string cmd = $"LRProc.exe LRHookx64.dll tms {runParam}";

            try
            {
                Process process = new();

                process.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    FileName = lrExe,
                    Arguments = $"{lrDll} tms {runParam}",
                    CreateNoWindow = false
                };

                process.Start();

                await process.WaitForExitAsync();
            }
            catch (Exception e)
            {
                BeanfunConst.Instance.MessageService?.Show($"启动游戏异常 {e.Message}");
            }
        }
    }
}