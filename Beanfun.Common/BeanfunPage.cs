using Beanfun.Common.Extensions;

using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.BlockResources;
using PuppeteerExtraSharp.Plugins.ExtraStealth;

using PuppeteerSharp;

namespace Beanfun.Common
{
    public class BeanfunPage : IDisposable
    {
        public PuppeteerExtra Extra { get; set; }
        public IBrowser? Browser { get; set; }

        private Dictionary<string, IPage> _pages = new();

        public BeanfunPage()
        {
            BlockResourcesPlugin plugin = new();

            plugin.AddRule(builder => builder.BlockedResources(ResourceType.Image));

            Extra = new PuppeteerExtra().Use(plugin).Use(new StealthPlugin());
            LaunchAsync();
        }

        public async void LaunchAsync(bool headless = true)
        {
            Browser = await Extra.LaunchAsync(new LaunchOptions
            {
                Headless = headless,
            });
        }

        public async Task<string> Post(string pageName, string url, Dictionary<string, string> param)
        {
            var page = await GetPage(pageName);

            if (page == null)
                return string.Empty;

            return await Post(page, url, param);
        }

        public async Task<string> Post(IPage page, string url, Dictionary<string, string> param)
        {
            if (page.IsClosed)
            {
                ClosePage(page);
                return string.Empty;
            }

            await page.SetRequestInterceptionAsync(true);

            var paramUrl = param.ToParam();

            var _postAction = new EventHandler<RequestEventArgs>(async (sender, e) =>
             {
                 await e.Request.ContinueAsync(new Payload
                 {
                     Method = HttpMethod.Post,
                     Headers = new Dictionary<string, string>
                     {
                        { "Content-Type", "application/x-www-form-urlencoded" }
                     },
                     PostData = paramUrl
                 });
             });

            await page.SetUserAgentAsync(BeanfunHeader.UserAgent);

            page.Request += _postAction;

            var respose = await page.GoToAsync(url);

            await Task.Delay(500);

            var html = await respose.TextAsync();

            await page.CloseAsync();

            return html.Trim();
        }

        public async Task<string> Get(string pageName, string url, bool isClose = false)
        {
            var page = await GetPage(pageName);

            if (page == null)
                return string.Empty;

            return await Get(page, url, isClose);
        }

        public async Task<string> Get(IPage page, string url, bool isClose = false)
        {
            if (page.IsClosed)
            {
                ClosePage(page);
                return string.Empty;
            }

            await page.SetUserAgentAsync(BeanfunHeader.UserAgent);

            var respose = await page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded);

            await Task.Delay(500);

            var html = await respose.TextAsync();

            if (isClose)
            {
                ClosePage(page);
            }

            return html.Trim();
        }

        public async void ClosePage(string pageName)
        {
            IPage? page = null;

            foreach (var item in _pages)
            {
                if (item.Key == pageName)
                {
                    page = item.Value;
                    break;
                }
            }

            if (page != null)
            {
                await page.CloseAsync();
                _pages.Remove(pageName);
            }
        }

        public async void ClosePage(IPage page)
        {
            await page.CloseAsync();

            string? pageName = null;

            foreach (var item in _pages)
            {
                if (item.Value == page)
                {
                    pageName = item.Key;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(pageName))
                _pages.Remove(pageName);
        }

        public async Task<IPage?> GetPage(string pageName)
        {
            if (Browser == null)
                return null;

            if (string.IsNullOrEmpty(pageName))
                return null;

            foreach (var item in _pages)
            {
                if (item.Key == pageName)
                {
                    return item.Value;
                }
            }

            var page = await Browser.NewPageAsync();

            _pages.Add(pageName, page);

            return page;
        }

        public async void Dispose()
        {
            foreach (var item in _pages)
                item.Value.Dispose();

            if (Browser != null)
                await Browser.CloseAsync();

            Browser = null;

            GC.SuppressFinalize(this);
        }
    }
}