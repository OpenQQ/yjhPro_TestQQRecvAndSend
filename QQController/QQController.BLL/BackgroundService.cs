using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QQController.Common;
using QQController.Common.Enum;
using QQController.DAL;
using QQController.Entity;
using QQController.Entity.ViewModel;

namespace QQController.BLL
{
    public class BackgroundService
    {
        private readonly MainDbContext m_MainDbContext;
        public BackgroundService()
        {
            m_MainDbContext = new MainDbContext();
        }

        public bool SaveRecevicedMessage(string from, string to, string owner, ReceviceMessageTypeEnum type,
            string message, string groupNum)
        {
            var t = DateTime.Now;
            var msg = new ReceivcedMessage()
            {
                CreateTime = t,
                UpdateTime = t,
                From = from,
                GroupNum = groupNum,
                Owner = owner,
                To = to,
                Type = (int)type,
                Message = message
            };
            m_MainDbContext.ReceivcedMessageSet.Add(msg);
            var num = m_MainDbContext.SaveChanges();
            if (num > 0)
            {
                Logger.Info($"({from})发给({to})的消息接收成功！消息类型是：{type.ToString()}");
                return true;
            }
            else
            {
                Logger.Info($"({from})发给({to})的消息接收失败！消息类型是：{type.ToString()}");
                return false;
            }
        }

        public SendedMessageOutViewModel GetSendedMessage()
        {
            var target = m_MainDbContext.SendedMessageSet.Where(e => !e.IsD).OrderBy(e => e.CreateTime).FirstOrDefault();
            if (target == null)
            {
                return null;
            }
            var ret = new SendedMessageOutViewModel()
            {
                Message =  target.Message,
                From = target.From,
                To = target.To
            };
            target.IsSend = true;
            target.IsD = true;
            m_MainDbContext.SendedMessageSet.AddOrUpdate(target);
            m_MainDbContext.SaveChanges();
            return ret;
        }
    }
}
