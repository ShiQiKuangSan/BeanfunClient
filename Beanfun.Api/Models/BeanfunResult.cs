namespace Beanfun.Api.Models
{
    public class BeanfunResult
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        public BeanfunResult Success()
        {
            IsSuccess = true;

            return this;
        }

        public BeanfunResult Error(string msg = "")
        {
            IsSuccess = false;

            Message = msg;

            return this;
        }
    }

    public class BeanfunResult<T> : BeanfunResult
    {
        public T? Data { get; set; }

        public new BeanfunResult<T> Success()
        {
            IsSuccess = true;

            return this;
        }

        public new BeanfunResult<T> Error(string msg = "")
        {
            IsSuccess = false;

            Message = msg;

            return this;
        }
    }
}