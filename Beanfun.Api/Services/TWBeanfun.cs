using Beanfun.Api.Models;

namespace Beanfun.Api.Services
{
    public sealed class TWBeanfun : BaseBeanfunService
    {
        public override Task<BeanfunResult> AddAccount(string newName)
        {
            throw new NotImplementedException();
        }

        public override Task<BeanfunResult> ChangeAccountName(string accountId, string newName)
        {
            throw new NotImplementedException();
        }

        public override Task<BeanfunResult<BeanfunAccountResult>> GetAccountList(string token)
        {
            throw new NotImplementedException();
        }

        public override Task<BeanfunResult<string>> GetDynamicPassword(BeanfunAccount account, string token)
        {
            throw new NotImplementedException();
        }

        public override Task<int> GetGamePoints(string token)
        {
            throw new NotImplementedException();
        }
    }
}