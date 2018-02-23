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

        public SendedMessageOutViewModel GetSendedMessage(ICollection<string> qq)
        {
            // AddSendedMessage();
            var target = m_MainDbContext.SendedMessageSet.Where(e => !e.IsD && !e.IsSend && qq.Contains(e.From)).OrderBy(e => e.CreateTime).FirstOrDefault();
            if (target == null)
            {
                return null;
            }
            var ret = new SendedMessageOutViewModel()
            {
                Message = target.Message,
                From = target.From,
                To = target.To,
                Id = target.Id
            };
            target.IsSend = true;
            target.IsD = true;
            m_MainDbContext.SendedMessageSet.AddOrUpdate(target);
            m_MainDbContext.SaveChanges();
            return ret;
        }
        public bool UpdateSendedMessage(int id)
        {
            var target = m_MainDbContext.SendedMessageSet.Where(e => e.Id == id).FirstOrDefault();
            if (target == null)
            {
                return false;
            }
            
            target.IsSend = false;
            target.IsD = false;
            target.CreateTime = DateTime.Now;
            m_MainDbContext.SendedMessageSet.AddOrUpdate(target);
            m_MainDbContext.SaveChanges();
            return true;
        }

        public bool AddSendedMessage()
        {
            // 测试
            var target = new SendedMessage();
            target.CreateTime=DateTime.Now;
            target.From = "1561322601";
            target.To = "1311949161";
            target.Message  = "使得房价过快的是的力量";
            target.Owner = "1561322601";
            m_MainDbContext.SendedMessageSet.Add(target);
            m_MainDbContext.SaveChanges();
            return true;
        }

        public bool AddQQFriends(FriendViewModel model)
        {
            var t = DateTime.Now;
            var res = m_MainDbContext.QQFriendsSet.Where(e => !e.IsD && e.Owner == model.Owner && e.QQNum == model.Qq)
                .FirstOrDefault();
            if (res != null)
            {
                res.Nick = model.Nick;
                res.UpdateTime = t;
                m_MainDbContext.QQFriendsSet.AddOrUpdate(res);
            }
            else
            {
                var target = new QQFriend()
                {
                    CreateTime = t,
                    UpdateTime = t,
                    IsD = false,
                    Nick = model.Nick,
                    Owner = model.Owner,
                    QQNum = model.Qq
                };
                m_MainDbContext.QQFriendsSet.Add(target);
            }
            
            return m_MainDbContext.SaveChanges()>0;
        }
    }
}
