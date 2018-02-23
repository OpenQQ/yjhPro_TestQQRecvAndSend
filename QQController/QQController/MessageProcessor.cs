using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using QQController.BLL;
using QQController.Common;
using QQController.Common.Enum;
using QQController.Entity.ViewModel;

namespace QQController
{
    public class MessageProcessor
    {
        public static ConcurrentDictionary<string, Socket> QqSocketClientDictionary = new ConcurrentDictionary<string, Socket>();
        
        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] ToBytesFromMessageString(string msg)
        {
            int len = Encoding.UTF8.GetByteCount(msg);
            byte[] buf = new byte[4];
            buf[0] = (byte)(len);
            buf[1] = (byte)(len >> 8);
            buf[2] = (byte)(len >> 16);
            buf[3] = (byte)(len >> 24);
            return buf.Concat(Encoding.UTF8.GetBytes(msg)).ToArray();
        }

        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="clientSocket"></param>
        public static void ReceiveMsg(object clientSocket)
        {
            Socket client = clientSocket as Socket;

            int readLen = 0;
            int frameLen = 0;
            byte[] frame = null;
            byte[] buf = new byte[1024 * 1024];
            while (true)
            {
                try
                {
                    int len = 0;
                    len = client.Receive(buf);
                    for (int i = 0; i < len; i++)
                    {
                        if (readLen < 4)
                        {
                            // 计算消息长度
                            switch (readLen)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                    frameLen += buf[i] << (readLen * 8);
                                    break;
                            }
                        }
                        else
                        {
                            if (len - i >= frameLen)
                            {
                                // 成功
                                if (frame == null)
                                {
                                    frame = buf.Skip(i).Take(frameLen).ToArray();
                                    // 接受消息
                                    DealWithReceiveMessage(Encoding.Default.GetString(frame), client);
                                    return;
                                    i += frameLen - 1;
                                    readLen = -1;
                                    frameLen = 0;
                                    frame = null;
                                }
                                else
                                {
                                    int oldLen = frame.Length;
                                    frame = frame.Concat(buf.Skip(i).Take(frameLen - oldLen)).ToArray();
                                    // 接受消息
                                    DealWithReceiveMessage(Encoding.Default.GetString(frame), client);
                                    return;
                                    i += frameLen - oldLen - 1;
                                    readLen = -1;
                                    frameLen = 0;
                                    frame = null;
                                }
                            }
                            else
                            {
                                if (frame == null)
                                {
                                    frame = buf.Skip(i).Take(len - i).ToArray();
                                    i = len - 1;
                                    readLen += len - i - 1;
                                }
                                else
                                {
                                    int oldLen = frame.Length;
                                    if (len - i >= frameLen - oldLen)
                                    {
                                        // 成功
                                        frame = frame.Concat(buf.Skip(i).Take(frameLen - oldLen)).ToArray();
                                        // 接受消息
                                        DealWithReceiveMessage(Encoding.Default.GetString(frame), client);
                                        return;
                                        i += frameLen - oldLen - 1;
                                        readLen = -1;
                                        frameLen = 0;
                                        frame = null;
                                    }
                                    else
                                    {
                                        frame = frame.Concat(buf.Skip(i).Take(len - i)).ToArray();
                                        i = len - 1;
                                        readLen += len - i - 1;
                                    }
                                }
                            }
                        }
                        readLen++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    if (client != null)
                    {
                        client.Close();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 单线程处理接受消息
        /// </summary>
        public static void DealWithReceiveMessage(string msg, Socket client)
        {
            int index = msg.IndexOf(",");
            var type = Convert.ToInt32(new string(msg.Take(index).ToArray()));
            msg = new string(msg.Skip(index + 1).ToArray());
            index = msg.IndexOf(",");
            var qq = new string(msg.Take(index).ToArray());
            var context = new string(msg.Skip(index + 1).ToArray());
            BackgroundService service = new BackgroundService();
            switch (type)
            {
                case (int)(MessageTypeEnum.Cookies):
                    // cookie
                    // TODO：这里获取好友列表(完成)
                    QqSocketClientDictionary[qq] = client;
                    var m = Regex.Match(context, @"skey=(?<skey>[0-9a-zA-Z]*);");
                    if (m.Success)
                    {
                        string sKey = m.Groups["skey"].Value;
                        long g_tk = GetG_tk(sKey);
                        string url =
                            string.Format("http://m.qzone.com/friend/mfriend_list?g_tk={0}&res_uin={1}&res_type=normal&format=json&count_per_page=10&page_index=0&page_type=0&mayknowuin=&qqmailstat=", g_tk, qq);
                        Dictionary<string ,string> headers = new Dictionary<string, string>();
                        headers.Add("Cookie", context);
                        string content = HttpClient.GetHttpResponse(url, headers);
                        var result = JsonConvert.DeserializeObject<dynamic>(content);
                        if (result.code == -3000)
                        {
                            Logger.Error(result.message);
                        }
                        else
                        {
                            HashSet<FriendViewModel> set = new HashSet<FriendViewModel>();
                            var items_list = result.data.list;
                            foreach (var item in items_list)
                            {
                                set.Add(new FriendViewModel()
                                {
                                    Owner = qq,
                                    Qq = item.uin,
                                    Nick = item.nick
                                });
                            }

                            foreach (var viewModel in set)
                            {
                                service.AddQQFriends(viewModel);
                            }
                        }
                    }
                    break;
                case (int)(MessageTypeEnum.CommonMsg):
                    // 消息
                    MessageInViewModel model = JsonConvert.DeserializeObject<MessageInViewModel>(context);
                    // TODO：消息过滤(完成)
                    // [CQ:image,file=1F782FD4E9960B8DC370C995A7C7463D.png]"
                    var matches = Regex.Matches(model.Msg, @"\[CQ:image,file=(?<filename>[0-9a-zA-Z]*\.\w*)\]");
                    foreach (Match match in matches)
                    {
                        var value = match.Groups["filename"].Value;
                        var filename = Path.Combine(Global.AirPath, "data/image", value + ".cqimg");
                        string url = "";
                        foreach (var line in File.ReadAllLines(filename))
                        {
                            if (line.StartsWith("url="))
                            {
                                url = new string(line.Skip(4).ToArray());
                                break;
                            }
                        }

                        model.Msg = model.Msg.Replace(value, url);
                    }
                    service.SaveRecevicedMessage(model.FromQQ, qq, qq, model.SubType, model.Msg, model.GroupNum);
                    break;
            }
        }
        public static long GetG_tk(string sKey)
        {
            int hash = 5381;
            for (int i = 0, len = sKey.Length; i < len; ++i)
            {
                hash += (hash << 5) + (int)sKey[i];
            }
            return (hash & 0x7fffffff);
        }

        /// <summary>
        /// 单线程处理发送消息
        /// </summary>
        public static void DealWidthSendMessage(Form2 form2)
        {
            Message msg = null;
            BackgroundService service = new BackgroundService();
            
            while (true)
            {
                SendedMessageOutViewModel send = null;
                try
                {
                    send = service.GetSendedMessage(QqSocketClientDictionary.Keys);
                    if (send != null)
                    {
                        msg = new Message
                        {
                            Context = send.To + "," + send.Message,
                            QQ = send.From
                        };
                        var client = QqSocketClientDictionary[msg.QQ];
                        client.BeginSend(msg.MsgBody, 0, msg.MsgBody.Length,
                            SocketFlags.None, (obj) => { Logger.Info(msg.QQ + "发送消息"); }, null);
                    }
                }
                catch(Exception ex)
                {
                    try
                    {
                        Logger.Error(ex.ToString());
                        service.UpdateSendedMessage(send.Id);
                        // 关闭，掉线
                        QqSocketClientDictionary.TryRemove(msg.QQ, out var val);
                        var pid = Global.QqProcessDictionary[msg.QQ];
                        var process = Process.GetProcessById(pid);
                        if (process != null)
                        {
                            process.Kill();
                            process.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    form2.UpdateStatusDesc(msg.QQ, "掉线");
                }
                Thread.Sleep(3000);
            }
        }
    }

    public class Message
    {
        public string QQ { set; get; }
        public  string Context { set; get; }

        public MessageTypeEnum Type { get; set; }

        public byte[] MsgBody
        {
            get { return  Encoding.Default.GetBytes(Context); }
        }
    }

    public enum MessageTypeEnum
    {
        Cookies,
        CommonMsg
    }

    public class MessageInViewModel
    {
        public ReceviceMessageTypeEnum SubType { set; get; }

        public string FromQQ { set; get; }

        public string Msg { set; get; }

        public string GroupNum { set; get; }
    }
}
