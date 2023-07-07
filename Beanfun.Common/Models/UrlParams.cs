using System.Text;

namespace Beanfun.Common.Models
{
    public class UrlParams
    {
        private string _url { get; set; }

        private Dictionary<string, string> _params;

        public UrlParams(string url)
        {
            this._url = url;
            _params = new Dictionary<string, string>();
        }

        public UrlParams AddParam(string key, string value)
        {
            _params.Add(key, value);
            return this;
        }

        public UrlParams AddParam(Dictionary<string, string> pairs)
        {
            _params = pairs;
            return this;
        }

        public string Build()
        {
            _url = _url.TrimEnd('?').TrimEnd('？');

            StringBuilder builder = new(_url);

            int i = 0;
            foreach (var item in _params)
            {
                StringBuilder valParam = new();

                if (i == 0)
                    valParam.Append('?');
                else
                    valParam.Append('&');

                valParam.Append(item.Key).Append('=').Append(item.Value);
                builder.Append(valParam);
                i++;
            }

            return builder.ToString();
        }
    }
}