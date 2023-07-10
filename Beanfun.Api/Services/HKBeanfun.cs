using Beanfun.Api.Models;
using Beanfun.Common;
using Beanfun.Common.Extensions;

using HtmlAgilityPack;

using Masuit.Tools.DateTimeExt;

using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Beanfun.Api.Services
{
    internal sealed class HKBeanfun : BaseBeanfunService
    {
        private readonly BeanfunPage _beanfunPage;

        public HKBeanfun()
        {
            _beanfunPage = BeanfunConst.Instance.Page;
        }

        /// <summary>
        /// 获得账户列表
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public override async Task<BeanfunResult<BeanfunAccountResult>> GetAccountList(string token)
        {
            BeanfunResult<BeanfunAccountResult> result = new();

            string url = "https://bfweb.hk.beanfun.com/beanfun_block/auth.aspx";

            url = url.ToParam()
                .AddParam("channel", "game_zone")
                .AddParam("page_and_query", "game_start.aspx?service_code_and_region=610074_T9")
                .AddParam("web_token", token)
                .Build();

            var html = await _beanfunPage.Get("GetAccountList", url, true);

            if (string.IsNullOrEmpty(html))
            {
                return result.Error("获取账号失败，请检查您的网络是否通畅");
            }

            result.Data = new BeanfunAccountResult();

            HtmlDocument document = new();
            document.LoadHtml(html);

            //是否可以继续创建账号
            var accountAmountLimitNotice = document.DocumentNode.SelectSingleNode("//*[@id='divServiceAccountAmountLimitNotice']");

            if (accountAmountLimitNotice != null)
            {
                var number = Regex.Replace(accountAmountLimitNotice.InnerText, @"[^0-9]+", "") ?? string.Empty;

                var status = int.TryParse(number, out var maxNumber);
                if (status)
                {
                    result.Data.MaxActNumber = maxNumber;
                }
            }
            else
            {
                return result.Error("获取允許新增帳號數失败");
            }

            var accountList = document.DocumentNode.SelectSingleNode("//*[@id='ulServiceAccountList']/li");

            if (!accountList.ChildNodes.Any())
            {
                //没有账号的情况下，判断是否没有进阶认证
                if (accountAmountLimitNotice.OuterHtml.Contains("進階認證"))
                {
                    result.Data.CertStatus = false;
                }
            }

            foreach (var node in accountList.ChildNodes)
            {
                if (node.Name == "div")
                {
                    var account = new BeanfunAccount();

                    var id = node.GetAttributeValue("id", string.Empty);
                    var sn = node.GetAttributeValue("sn", string.Empty);
                    var name = node.GetAttributeValue("name", string.Empty);
                    var status = node.GetAttributeValue("onclick", string.Empty);

                    account.Id = id;
                    account.Name = GetAccountName(name);
                    account.Sn = sn;
                    account.Status = !string.IsNullOrEmpty(status);
                    account.CreateTime = await GetAccountCreateTime(sn);

                    result.Data.AccountList.Add(account);
                }
            }

            _beanfunPage.ClosePage("GetAccountCreateTime");

            return result.Success();
        }

        /// <summary>
        /// 获取动态密码
        /// </summary>
        /// <param name="account">账户</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        public override async Task<BeanfunResult<string>> GetDynamicPassword(BeanfunAccount account, string token)
        {
            BeanfunResult<string> result = new();
            if (account == null || string.IsNullOrEmpty(account.Id))
            {
                return result.Error("获取动态密码失败,账号信息不存在!");
            }

            string strDateTime = GetDate();

            string url = "https://bfweb.hk.beanfun.com/beanfun_block/game_zone/game_start_step2.aspx";

            url = url.ToParam()
                 .AddParam("service_code", "610074")
                 .AddParam("service_region", "T9")
                 .AddParam("sotp", account.Sn)
                 .AddParam("dt", strDateTime)
                 .Build();

            var html = await _beanfunPage.Get("GetDynamicPassword", url);

            if (string.IsNullOrEmpty(html))
            {
                return result.Error("获取动态密码失败1000");
            }

            var dataList = Regex.Match(html, "GetResultByLongPolling&key=(.*?)\"");

            if (dataList == null)
            {
                return result.Error("获取pollingKey失败");
            }

            if (dataList.Groups.Count != 2)
            {
                return result.Error("获取pollingKey失败1000");
            }

            var pollingKey = dataList.Groups[1].Value ?? string.Empty;

            url = "https://login.hk.beanfun.com/generic_handlers/get_cookies.ashx";

            html = await _beanfunPage.Get("GetDynamicPassword", url);

            if (string.IsNullOrEmpty(html))
            {
                return result.Error("获取动态密码失败2000");
            }

            dataList = Regex.Match(html, "var\\sm_strSecretCode\\s=\\s'(.*)'");

            if (dataList == null)
            {
                return result.Error("获取secret失败");
            }

            if (dataList.Groups.Count != 2)
            {
                return result.Error("获取secret失败1000");
            }

            string secret = dataList.Groups[1].Value ?? string.Empty;

            url = "https://bfweb.hk.beanfun.com/beanfun_block/generic_handlers/get_webstart_otp.ashx";

            url = url.ToParam()
                .AddParam("sn", pollingKey)
                .AddParam("WebToken", token)
                .AddParam("SecretCode", secret)
                .AddParam("ppppp", "F9B45415B9321DB9635028EFDBDDB44B4012B05F95865CB8909B2C851CFE1EE11CB784F32E4347AB7001A763100D90768D8A4E30BCC3E80C")
                .AddParam("ServiceCode", "610074")
                .AddParam("ServiceRegion", "T9")
                .AddParam("ServiceAccount", account.Id)
                .AddParam("CreateTime", account.CreateTime)
                .AddParam("d", DateTime.Now.GetTotalMilliseconds().ToString())
                .Build();

            html = await _beanfunPage.Get("GetDynamicPassword", url, true);

            if (string.IsNullOrEmpty(html))
            {
                return result.Error("获取动态密码失败3000");
            }

            var pwd = DecrDesPkcs5Hex(html);

            if (string.IsNullOrEmpty(pwd))
            {
                return result.Error("解析动态密码失败");
            }

            result.Data = pwd;

            return result.Success();
        }

        /// <summary>
        /// 获取游戏点数
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public override async Task<int> GetGamePoints(string token)
        {
            string url = "https://bfweb.hk.beanfun.com/beanfun_block/generic_handlers/get_remain_point.ashx";
            var time = DateTime.Now.ToString("yyyyMMddHHmmss.fff");

            url = url.ToParam()
                .AddParam("webtoken", "1")
                .AddParam("noCacheIE", time)
                .Build();

            var html = await _beanfunPage.Get("GetGamePoints", url, true);

            var pointMatch = Regex.Match(html, "\"RemainPoint\"\\s:\\s\"(\\d+)\"");

            if (pointMatch == null)
                return 0;

            if (!pointMatch.Success)
                return 0;

            if (pointMatch.Groups.Count != 2)
                return 0;

            var status = int.TryParse(pointMatch.Groups[1].Value, out int point);

            if (!status)
                return 0;

            return point;
        }

        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="newName">新名字</param>
        /// <returns></returns>
        public override async Task<BeanfunResult> AddAccount(string newName)
        {
            BeanfunResult result = new();

            string url = "https://bfweb.hk.beanfun.com/generic_handlers/gamezone.ashx";

            var html = await _beanfunPage.Post("AddAccount", url, new Dictionary<string, string>
            {
                {"strFunction", "AddServiceAccount" } ,
                {"npsc", ""} ,
                {"npsr", "" },
                {"sc", "610074" } ,
                { "sr", "T9" }  ,
                { "sadn", newName.Trim()},
                {"sag", "" }
            });

            if (string.IsNullOrEmpty(html))
            {
                return result.Error("添加账号失败1000");
            }

            try
            {
                var json = JsonNode.Parse(html);

                if (json == null)
                    return result.Error("添加账号失败3000");

                var intResult = json?["intResult"]?.GetValue<int>() ?? 0;

                if (intResult != 1)
                {
                    var strOutstring = json?["strOutstring"]?.GetValue<string>() ?? string.Empty;
                    return result.Error($"账号操作异常! ---> {strOutstring}");
                }
            }
            catch (Exception)
            {
                return result.Error("添加账号失败2000");
            }

            return result.Success();
        }

        /// <summary>
        /// 更改账户名称
        /// </summary>
        /// <param name="accountId">账户ID</param>
        /// <param name="newName">新名字</param>
        /// <returns></returns>
        public override async Task<BeanfunResult> ChangeAccountName(string accountId, string newName)
        {
            BeanfunResult result = new();

            string url = "https://bfweb.hk.beanfun.com/generic_handlers/gamezone.ashx";

            var html = await _beanfunPage.Post("ChangeAccountName", url, new Dictionary<string, string>
            {
                {"strFunction", "ChangeServiceAccountDisplayName"},
                {"sl", "610074_T9" },
                {"said", accountId },
                {"nsadn", newName.Trim() }
            });

            if (string.IsNullOrEmpty(html))
            {
                return result.Error("更改账户名称失败1000");
            }

            try
            {
                var json = JsonNode.Parse(html);

                if (json == null)
                    return result.Error("更改账户名称失败3000");

                var intResult = json?["intResult"]?.GetValue<int>() ?? 0;

                if (intResult != 1)
                {
                    var strOutstring = json?["strOutstring"]?.GetValue<string>() ?? string.Empty;
                    return result.Error($"账号操作异常! ---> {strOutstring}");
                }
            }
            catch (Exception)
            {
                return result.Error("更改账户名称失败2000");
            }

            return result.Success();
        }

        private async Task<string> GetAccountCreateTime(string sn)
        {
            string url = "https://bfweb.hk.beanfun.com/beanfun_block/game_zone/game_start_step2.aspx";

            string strDateTime = GetDate();

            url = url.ToParam()
                 .AddParam("service_code", "610074")
                 .AddParam("service_region", "T9")
                 .AddParam("sotp", sn)
                 .AddParam("dt", strDateTime)
                 .Build();

            var html = await _beanfunPage.Get("GetAccountCreateTime", url);

            //var MyAccountData = {ServiceCode: "610074", ServiceRegion: "T9", ServiceAccountID: "T9418fd9e30001451740", ServiceAccountSN: "1287588", ServiceAccountDisplayName: "冒險初心", ServiceAccountAuthType: "N", ServiceAccountCreateTime: "2020-02-10 19:37:11", RemovedServiceFriendlyRemindersCookieName: "RSFR_23891490", HasFormContract: false, IfShowServiceFriendlyReminder: false, IfShowCheckIP: false, GamePrepareUrl: "/beanfun_block/game_zone/game_prepare.aspx?service_code=610074&service_region=T9" };
            //var intFriendlyReminderOKCounter = 6;

            Regex regex = new Regex("ServiceAccountCreateTime:\\s\\\"([^\\\"]+)\\\"");

            var data = regex.Matches(html).FirstOrDefault();
            if (data == null)
                return string.Empty;

            if (data.Groups.Count != 2)
                return string.Empty;

            var dataStr = data.Groups[1].Value;
            var status = DateTime.TryParse(dataStr, out _);

            if (status)
            {
                return dataStr;
            }

            return string.Empty;
        }

        private static string GetDate()
        {
            var dateTime = DateTime.Now;

            return "" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond;
        }
    }
}