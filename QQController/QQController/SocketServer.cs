using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQController
{
    public class SocketServer
    {
        public string Address { set; get; }
        public int Port { set; get; }
        public SocketServer(string addr = "127.0.0.1", int port = 50001)
        {
            Address = addr;
            Port = port;
        }

        public ConcurrentQueue<Socket> ClientList = new ConcurrentQueue<Socket>();

        public void StartServer(Form2 form2)
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(Address), Port));
            socket.Listen(1000);
            // 接受消息
            new Thread(() =>
            {
                while (true)
                {
                    if (ClientList.TryDequeue(out var result))
                    {
                        Thread.Sleep(50);
                        try
                        {
                            if (result.Available > 0)
                            {
                                MessageProcessor.ReceiveMsg(result);
                            }
                            ClientList.Enqueue(result);
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                }
            })
            {
                IsBackground = true
            }.Start();
            // 发送消息
            new Thread(() =>
            {
                MessageProcessor.DealWidthSendMessage(form2);
            })
            {
                IsBackground = true
            }.Start();
            while (true)
            {
                var client = socket.Accept();
                ClientList.Enqueue(client);
            }
        }
    }
}
