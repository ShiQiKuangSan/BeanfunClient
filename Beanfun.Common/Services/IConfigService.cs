using Beanfun.Common.Models.Config;

namespace Beanfun.Common.Services
{
    public interface IConfigService
    {

        public void InitConfig();

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

}