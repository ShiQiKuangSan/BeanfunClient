using System.Diagnostics;
using System.Runtime.InteropServices;

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

                var info = process.StartInfo;

                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Verb = "runas";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;

                process.Start();

                await process.StandardInput.WriteLineAsync(cmd);

                await process.WaitForExitAsync();

            }
            catch (Exception)
            {
            }
        }


        public static void OpenApp(string appPath, string args = "")
        {
            try
            {
                Process process = new();

                var dir = Path.GetDirectoryName(appPath);

                var info = process.StartInfo;


                process.StartInfo.FileName = appPath;
                process.StartInfo.Arguments = args;
                process.StartInfo.Verb = "runas";
                process.StartInfo.WorkingDirectory = dir;
                process.StartInfo.UseShellExecute = true;
                
                process.Start();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 关闭游戏初始页
        /// </summary>
        public static void CloseMapleStoryStart()
        {

            System.Timers.Timer timer = new()
            {
                Interval = 1000,   //1秒运行一次
                AutoReset = true  //一直循环
            };

            int i = 1;

            timer.Elapsed += (sender, arg) =>
            {
                IntPtr hwnd = IntPtr.Zero;

                try
                {
                    hwnd = Win32Api.FindWindow("StartUpDlgClass", "MapleStory");

                    if (hwnd != IntPtr.Zero)
                    {
                        Win32Api.SetForegroundWindow(hwnd);
                        Win32Api.SendMessage(hwnd, 16, 0, 0);
                        timer.Close();
                    }

                }
                catch (Exception e)
                {
                    timer.Close();
                    BeanfunConst.Instance.MessageService?.Show($"自动关闭Play窗口发生异常! e={e.Message}");
                }
                finally
                {
                    i++;

                    if (hwnd != IntPtr.Zero || i > 5)
                    {
                        timer.Close();
                    }
                }
            };

            timer.Start();
        }

        /// <summary>
        /// 阻止游戏更新
        /// </summary>
        public static void StopAutoPatcher()
        {
            System.Timers.Timer timer = new()
            {
                Interval = 1000,   //1秒运行一次
                AutoReset = true,  //一直循环
            };

            int i = 1;

            timer.Elapsed += (sender, arg) =>
            {
                try
                {

                    var patcher = Process.GetProcessesByName("Patcher")
                      .FirstOrDefault(x => x.Id > 0);

                    if (patcher == null)
                        return;

                    var pid = patcher.Id;

                    Cmd($"taskkill /F /PID {pid}");
                    timer.Close();
                }
                catch (Exception e)
                {
                    timer.Close();
                    BeanfunConst.Instance.MessageService?.Show($"阻止游戏更新发生异常! e={e.Message}");
                }
                finally
                {
                    i++;
                    if (i > 5)
                    {
                        timer.Close();
                    }
                }
            };

            timer.Start();
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

    public class Win32Api
    {

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        /// <summary>    
        /// 该函数将创建指定窗口的线程设置到前台，并且激活该窗口。
        /// 键盘输入转向该窗口，并为用户改各种可视的记号。系统给创建前台窗口的
        /// 线程分配的权限稍高于其他线程。  
        /// </summary>   
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>    
        /// 该函数将一个消息放入（寄送）到与指定窗口创建的线程相联
        /// 系消息队列里
        /// </summary>    
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
    }
}
