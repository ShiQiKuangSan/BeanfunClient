using System.Diagnostics;

namespace Beanfun.Common
{
    public static class WindowManager
    {

        /// <summary>
        /// 结束NGS进程
        /// </summary>
        /// <returns></returns>
        public static void KillBlackXchg()
        {
            Cmd("taskkill /f /im BlackXchg.aes");
        }


        public static async void Cmd(string cmd)
        {

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
            catch (Exception)
            {
            }
        }


        public static void OpenApp(string path, string args = "")
        {
            try
            {
                Process process = new();

                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = path,
                    Arguments = args,
                    Verb = "Open",
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                process.Start();

                process.WaitForExit();

                process.Close();
            }
            catch (Exception)
            {


            }
        }


        /// <summary>
        /// 关闭谷歌浏览器进程
        /// </summary>
        public static void CloseChrome()
        {
            var list = Process.GetProcessesByName("chrome");

            foreach (var item in list)
            {
                item.Kill();
            }
        }

    }
}
