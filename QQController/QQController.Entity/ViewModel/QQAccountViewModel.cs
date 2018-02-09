using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQController.Entity.ViewModel
{
    public class QQAccountViewModel
    {
        public int ID { set; get; }

        public string QQAccount { set; get; }

        public string Password { set; get; }

        public string StatusDesc => IsLogin ? "已登陆" : "未登录";

        public bool IsLogin { set; get; }
    }
}
