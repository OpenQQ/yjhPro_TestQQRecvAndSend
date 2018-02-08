using QQController.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QQController.Common;
using QQController.Common.Enum;
using QQController.DAL;

namespace QQController.BLL
{
    public class QQAcountManageService
    {
        private readonly MainDbContext m_MainDbContext;
        public QQAcountManageService()
        {
            m_MainDbContext = new MainDbContext();
        }

        public List<QQAccountViewModel> GetAccountViewModels(int pageIndex = 1, int pageSize = 100)
        {
            var query = m_MainDbContext.QQAccountSet.Where(e =>
                !e.IsD && !e.IsLogin && (e.State == (int) QQStateEnum.正常 || e.State == (int) QQStateEnum.限制));
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(e => new QQAccountViewModel()
            {
                ID = e.Id,
                QQAccount = e.QQNum,
                Password = e.Password,
                StatusDesc = e.StatusDesc
            }).ToList();
        }

        public bool UpdateQQIsLogin(int id, bool islogin)
        {
            var target = m_MainDbContext.QQAccountSet.FirstOrDefault(e => !e.IsD && e.Id == id);
            if (target == null)
            {
                Logger.Error("该QQ账户不存在,修改QQ状态失败!");
                return false;
            }

            target.IsLogin = islogin;
            var num = m_MainDbContext.SaveChanges();
            if (num > 0)
            {
                Logger.Info($"修改QQ({target.QQNum})为登陆成功");
                return true;
            }
            else
            {
                Logger.Info($"修改QQ({target.QQNum})状态为登陆失败");
                return false;
            }
        }

        public bool UpdateStatusDesc(int id, string desc)
        {
            var target = m_MainDbContext.QQAccountSet.FirstOrDefault(e => !e.IsD && e.Id == id);
            if (target == null)
            {
                Logger.Error("该QQ账户不存在,修改QQ状态失败!");
                return false;
            }

            target.StatusDesc = desc;
            var num = m_MainDbContext.SaveChanges();
            if (num > 0)
            {
                Logger.Info($"修改QQ({target.QQNum})登陆状态为{desc}成功");
                return true;
            }
            else
            {
                Logger.Info($"修改QQ({target.QQNum})登陆状态为{desc}失败");
                return false;
            }
        }
    }
}
