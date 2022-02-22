using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class MySocketClient
    {
        public string Ip { get; }
        public int Port { get; }
        private Socket socket = null;
        private IPEndPoint endpoint = null;

        public MySocketClient(string ip,int port)
        {
            Ip = ip;
            Port = port;
            if (endpoint == null)
            {
                endpoint =new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            }
            if (socket == null)
            {
                socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                socket.ReceiveBufferSize = 1024*10;
                socket.SendBufferSize = 1024*10;
                socket.NoDelay = true;
                //socket.SendTimeout = 1000 * 3;
                //socket.ReceiveTimeout = 1000 * 3;
            }

            Initlize();
        }

        private void Initlize()
        {
            try
            {
                // 连接服务器端
                socket.Connect(endpoint);
                // 保持接收数据
                //Task.Run(() => { KeepReceiving(); });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void KeepReceiving()
        {
            IList<byte> byteList = new List<byte>();
            while (true)
            {
                byte[] buffer = new byte[1024 * 8];
                int result = socket.Receive(buffer);
                if (result > 0)
                {
                    for(int i = 0; i < result; i++)
                    {
                        byteList.Add(buffer[i]);
                    }
                    continue;
                }

                // result=0，表示内容接收完毕了
                var messageInfo = JsonConvert.DeserializeObject<MessageInfo>(Encoding.UTF8.GetString(byteList.ToArray()));
                Console.WriteLine(messageInfo.FromUser + ":" + messageInfo.Message);
            }
        }

        public void Send(MessageInfo messageInfo)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageInfo));
            int len = bytes.Length;
            byte[] bytesHeader = BitConverter.GetBytes(len);
            socket.Send(bytesHeader);// 头部用于标志内容的长度，先发头部，再发内容。这样可以防止数据包粘连
            socket.Send(bytes);
        }

        public void Close()
        {
            socket.Close();
            socket.Dispose();
        }
    }
}
