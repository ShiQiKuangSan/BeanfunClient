using Beanfun.Models.Config;

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Beanfun.Services
{
    internal interface IConfigService
    {
        /// <summary>
        /// 保存配置
        /// </summary>
        public Task SaveConfig(Config config);

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        public Config GetConfig();
    }

    internal class ConfigService : IConfigService
    {
        private Config _config = new();

        private readonly string _path;
        private readonly string _filePath;
        private JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        private readonly IMessageService _messageService;

        public ConfigService(IMessageService messageService)
        {
            _path = Environment.CurrentDirectory;
            _filePath = _path + "\\config.json";
            _messageService = messageService;
            ReadConfig();
        }

        public Config GetConfig()
        {
            return _config;
        }

        public async Task SaveConfig(Config config)
        {
            _config = config;
            await SaveConfig();
        }

        private async Task SaveConfig()
        {
            //根据路径序列化类，查找是否有配置文件
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    using FileStream fs = new(_filePath, FileMode.Create, FileAccess.ReadWrite);
                    ;
                    var json = JsonSerializer.SerializeToUtf8Bytes(_config, options);
                    fs.Write(json, 0, json.Length);
                }
                catch (Exception)
                {
                    _messageService.Show("保存config.json失败");
                }
            });
        }

        private void ReadConfig()
        {
            if (Directory.Exists(_path) && File.Exists(_filePath))
            {
                //序列化内容

                using var fStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);

                try
                {
                    _config = JsonSerializer.Deserialize<Config>(fStream) ?? new Config();
                }
                catch (Exception)
                {
                    _messageService.Show("解析config.json文件失败");
                }
            }
            else
            {
                CreateConfig();
            }
        }

        private async void CreateConfig()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            await SaveConfig();
        }
    }
}