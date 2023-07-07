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

        private static BeanfunPage? _beanfunPage;

        public static BeanfunPage Page
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

        /// <summary>
        /// 登录成功后获取到的令牌
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}