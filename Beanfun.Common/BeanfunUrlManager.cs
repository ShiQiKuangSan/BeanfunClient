namespace Beanfun.Common
{
    public static class BeanfunHeader
    {
        public static string UserAgent => "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.58";
    }

    public static class HkBeanfunUrl
    {
        /// <summary>
        /// 注册账号链接
        /// </summary>
        public static string RegisteredUrl =>
            "https://bfweb.hk.beanfun.com/beanfun_web_ap/signup/preregistration.aspx?service=999999_T0&dt=20230610170523.367";

        /// <summary>
        /// 忘记密码链接
        /// </summary>
        public static string ForgotPwdUrl => "https://bfweb.hk.beanfun.com/member/forgot_pwd.aspx";

        /// <summary>
        /// 初始化登录地址（会跳转到下面这个链接）
        /// </summary>
        public static string LogintUrl => "https://bfweb.hk.beanfun.com/beanfun_block/bflogin/default.aspx?service=999999_T0";

        /// <summary>
        /// 登录成功后跳转的页面
        /// </summary>
        public static string HomeUrl => "https://bfweb.hk.beanfun.com/index.aspx";

        /// <summary>
        /// 服务中心
        /// </summary>
        public static string ServiceCenter => "https://csp.hk.beanfun.com/";
    }
}