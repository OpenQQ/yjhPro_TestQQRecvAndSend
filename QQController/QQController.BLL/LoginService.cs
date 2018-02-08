using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QQController.Common;
using QQController.DAL;

namespace QQController.BLL
{
    public class LoginService
    {
        private readonly MainDbContext m_MainDbContext;
        public LoginService()
        {
            m_MainDbContext = new MainDbContext();
        }
        public bool CheckAccount(string account, string password)
        {
            var ret = m_MainDbContext.AccountSet.Any(e => e.AccountName == account && e.Password == password && !e.IsD);
            Logger.Info(ret ? $"验证用户({account})登陆成功" : $"验证用户({account})登陆失败");
            return ret;
        }

    }
}
