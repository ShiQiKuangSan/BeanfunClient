namespace Beanfun.Models.Login
{
    public class LoginResult
    {
        public bool Status { get; set; }
        public string Token { get; set; } = string.Empty;

        public string Msg { get; set; } = string.Empty;

        public LoginResult Fail()
        {
            Status = false;

            return this;
        }

        public LoginResult Fail(string msg)
        {
            Status = false;
            this.Msg = msg;

            return this;
        }

        public LoginResult Success()
        {
            Status = true;

            return this;
        }
    }
}