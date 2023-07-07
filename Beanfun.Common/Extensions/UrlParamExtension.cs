using Beanfun.Common.Models;

using System.Text;

namespace Beanfun.Common.Extensions
{
    public static class UrlParamExtension
    {
        public static UrlParams ToParam(this string url)
        {
            return new UrlParams(url);
        }

        public static string ToParam(this Dictionary<string, string> param)
        {
            var str = new StringBuilder();

            foreach (var data in param)
            {
                str.Append($"{data.Key}={data.Value}&");
            }

            var data1 = str.ToString().TrimEnd('&');

            return data1.ToString();
        }
    }
}