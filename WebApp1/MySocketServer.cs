using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WebApp1
{
    public class MySocketServer
    {
        public string Ip { get; }
        public int Port { get; }
        private IPEndPoint endpoint = null;
        private Socket socket = null;
        private IList<Socket> connectedSockets = new List<Socket>();
        public MySocketServer(string ip,int port)
        {
            Ip = ip;
            Port = port;

            Initlize();
        }

        private void Initlize()
        {
            if (endpoint == null)
            {
                endpoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
            }
            if (socket == null)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveBufferSize = 1024*10;
                socket.SendBufferSize = 1024*10;
                socket.NoDelay = true;
                //socket.SendTimeout = 1000 * 3;
                //socket.ReceiveTimeout = 1000 * 3;
            }
            socket.Bind(endpoint);
            socket.Listen(100);
            // 这里Accept之后会产生一个新的Socket连接（表示一个客户端和服务器端连接了，这个Socket有客户端的地址），后面的收发数据都用这个Socket
            // 上面Listen指定了最大监听队列是100，则这里就可以建立100个连接
            //socket.Accept();
        }

        public async Task KeepAccepting()
        {
            await Task.Run(() => {
                while (true)
                {
                    var clientSocket = socket.Accept();
                    clientSocket.NoDelay = true;
                    clientSocket.SendBufferSize = 1024*10;
                    clientSocket.ReceiveBufferSize = 1024*10;
                    //clientSocket.ReceiveTimeout = 1000 * 3;
                    //clientSocket.SendTimeout = 1000 * 3;
                    connectedSockets.Add(clientSocket);
                    Console.WriteLine($"客户端{clientSocket.RemoteEndPoint.ToString()}已经连接");
                    ThreadPool.QueueUserWorkItem(KeepReceiving, clientSocket);
                }
            });
        }

        public void KeepReceiving(object socket)
        {
            // 1-接收到的字节比消息内容多时，表示多出来的部分
            // 2-接收到的字节不够一条完整的消息内容时，表示接收到的字节
            var clientSocket = socket as Socket;
            byte[] bytesExtra = null;
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 10];
                    int len = clientSocket.Receive(buffer);// 通过读取的数据长度为0，或Connected=false可以判断连接断开了
                    if (len == 0)
                    {
                        // 客户端调用Close()断开连接了
                        clientSocket.Close();
                        clientSocket.Dispose();
                        connectedSockets.Remove(clientSocket);
                        break;
                    }

                    //if (len > 0)// 注意，这里只考虑了发送的消息小于缓冲区的情况，这时可以一次性将发送的消息读取出来
                    //{
                    //    // 实际使用中，可能存在一条消息分多个包来发送的情况。这时就要自定义规则来读取了
                    //    var messageInfo = JsonConvert.DeserializeObject<MessageInfo>(Encoding.UTF8.GetString(buffer,0,len));
                    //    Console.WriteLine(messageInfo.FromUser + ":" + messageInfo.Message);
                    //}

                    Console.WriteLine($"接收数据大小为:{len}");

                    byte[] receive = buffer.Take(len).ToArray();
                    byte[] bytesContacted = null;
                    if (bytesExtra != null && bytesExtra.Length > 0)
                    {
                        bytesContacted = bytesExtra.Concat(receive).ToArray();
                    }
                    else
                    {
                        bytesContacted = receive;
                    }


                    bytesExtra = AnalizeContent(bytesContacted);
                }
                catch(SocketException ex)
                {
                    // 这里报异常是由于接收超时或客户端意外断开了（不是Close），服务器断开这个客户端连接
                    clientSocket.Close();
                    clientSocket.Dispose();
                    connectedSockets.Remove(clientSocket);
                    break;
                }
            }
        }

        // 将每次发送的消息大小放在4个字节的Header中先发送，每次先拿到消息大小再通过该方法来解析
        public byte[] AnalizeContent(byte[] toAnalizeBytes)
        {
            if (toAnalizeBytes.Length < 4)
            {
                return toAnalizeBytes;
            }

            // 先取出4个字节的头部，根据头部得到一个完整的内容长度
            byte[] bytesHeader = toAnalizeBytes.Take(4).ToArray();
            int contentLen = BitConverter.ToInt32(bytesHeader);
            if (toAnalizeBytes.Length - 4 >= contentLen)
            {
                // 说明本次接收到的数据比一个完整的消息内容还要多，拿出来解析，多的字节与下一次读到的字节拼到一起
                byte[] bytesContent = new byte[contentLen];
                Array.Copy(toAnalizeBytes, 4, bytesContent, 0, contentLen);
                var messageInfo = JsonConvert.DeserializeObject<MessageInfo>(Encoding.UTF8.GetString(bytesContent));
                Console.WriteLine(messageInfo.FromUser + ":" + messageInfo.Message);
                var bytesExtra = toAnalizeBytes.Skip(4 + contentLen).Take(toAnalizeBytes.Length - 4 - contentLen).ToArray();

                return AnalizeContent(bytesExtra);
            }

            // 说明本次接收到的数据不足一个完整的消息内容，把本次的数据和下次接收的数据拼到一起再来处理
            return toAnalizeBytes;
        }
    }
}
