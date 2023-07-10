using Beanfun.Common.Services;

using ICSharpCode.SharpZipLib.Zip;

namespace Beanfun.Common
{
    public class BeanfunConst
    {
        private static readonly object _lock = new();

        private static BeanfunConst? _instance;

        public static BeanfunConst Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new BeanfunConst();

                    return _instance;
                }
            }
        }

        private BeanfunPage? _beanfunPage;

        public BeanfunPage Page
        {
            get
            {
                lock (_lock)
                {
                    _beanfunPage ??= new BeanfunPage();

                    return _beanfunPage;
                }
            }
        }

        public IConfigService? ConfigService { get; set; }

        public IMessageService? MessageService { get; set; }

        /// <summary>
        /// 登录成功后获取到的令牌
        /// </summary>
        public string Token { get; set; } = string.Empty;

        public string LocaleRemulatorDir { get; set; }

        public string MapleStoryEmulator { get; set; }

        public string WarAllianceHtml { get; set; }

        public void InitApp(IConfigService configService, IMessageService messageService)
        {
            this.ConfigService = configService;

            this.MessageService = messageService;
        }

        public void InitPlugin()
        {
            string unzippath = Environment.CurrentDirectory + "\\xs-data\\";

            if (!Directory.Exists(unzippath))
            {
                Directory.CreateDirectory(unzippath);
            }

            LocaleRemulatorDir = $"{unzippath}\\Locale_Remulator\\";
            MapleStoryEmulator = $"{unzippath}\\MapleStoryEmulator\\MapleStoryEmulator.exe";
            WarAllianceHtml = $"{unzippath}\\WarAllianceHtml\\index.htm";

            var t = new Task(() => { });

            //解压压缩文件

            if (!File.Exists($"{unzippath}\\Locale_Remulator\\LRProc.exe"))
            {
                var localeRemulator = Plugin.ResourceManager.GetObject("Locale_Remulator");

                if (localeRemulator != null && localeRemulator is byte[] lr)
                {
                    t.ContinueWith((ts) =>
                    {
                        Unzip(lr, unzippath);
                    });
                }
            }

            if (!File.Exists($"{unzippath}\\MapleStoryEmulator\\MapleStoryEmulator.exe"))
            {
                var mapleStoryEmulator = Plugin.ResourceManager.GetObject("MapleStoryEmulator");

                if (mapleStoryEmulator != null && mapleStoryEmulator is byte[] mse)
                {
                    t.ContinueWith((ts) =>
                    {
                        Unzip(mse, unzippath);
                    });
                }
            }

            if (!File.Exists($"{unzippath}\\WarAllianceHtml\\index.htm"))
            {
                var warAllianceHtml = Plugin.ResourceManager.GetObject("WarAllianceHtml");

                if (warAllianceHtml != null && warAllianceHtml is byte[] wah)
                {
                    t.ContinueWith((ts) =>
                    {
                        Unzip(wah, unzippath);
                    });
                }
            }

            t.GetAwaiter().OnCompleted(GC.Collect);

            t.Start();
        }

        /// <summary>
        /// 启动纸娃娃
        /// </summary>
        public void StartMapEmul()
        {
            if (string.IsNullOrEmpty(MapleStoryEmulator))
                return;


            if (!File.Exists(MapleStoryEmulator))
                return;

            WindowManager.OpenApp(MapleStoryEmulator);
        }

        /// <summary>
        /// 联盟摆放模拟器
        /// </summary>
        public void StartWarAlliance()
        {
            if (string.IsNullOrEmpty(WarAllianceHtml))
                return;


            if (!File.Exists(WarAllianceHtml))
                return;

            WindowManager.OpenApp(WarAllianceHtml);
        }

        private static void Unzip(byte[] data, string path)
        {
            try
            {
                using ZipInputStream s = new(new MemoryStream(data));

                ZipEntry entry;

                while ((entry = s.GetNextEntry()) != null)
                {
                    var fileName = Path.GetFileName(entry.Name);

                    var dir = $"{path}/{entry.Name}";

                    if (string.IsNullOrEmpty(fileName) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (entry.CompressedSize == 0)
                            continue;

                        using FileStream streamWriter = File.Create(dir);

                        byte[] bytes = new byte[2048];

                        while (true)
                        {
                            int size = s.Read(bytes, 0, bytes.Length);

                            if (size > 0)
                                streamWriter.Write(bytes, 0, size);
                            else
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}