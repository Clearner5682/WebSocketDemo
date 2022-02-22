using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var mySocketClient = new MySocketClient("127.0.0.1", 5008);
            for (int i = 0; i < 20; i++)
            {
                mySocketClient.Send(new MessageInfo { FromUser = "wulinhao", Message = $"你好啊{i}" });
                Thread.Sleep(500);
            }
            mySocketClient.Close();


            Console.ReadKey();
            Console.WriteLine("已经与服务器断开连接，程序关闭");
        }

        private static int Test(int x)
        {
            if (x>2)
            {
                return Test(x-1);
            }

            return x;
        }
    }
}
