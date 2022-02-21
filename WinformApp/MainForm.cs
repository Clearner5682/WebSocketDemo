using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;
using System.Net;

namespace WinformApp
{
    public partial class MainForm : Form
    {
        private ClientWebSocket client = null;
        public MainForm()
        {
            InitializeComponent();

            this.btnClose.Hide();
            this.btnSend.Enabled = false;
        }

        // 建立连接
        private async void btnConnect_Click(object sender, EventArgs e)
        {
            this.txtServerUrl.Enabled = false;
            Uri serverUrI =new Uri(this.txtServerUrl.Text);
            try
            {
                client = new ClientWebSocket();
                client.Options.Cookies = new CookieContainer();
                client.Options.Cookies.Add(new Cookie("UserId","wulinhao","/","localhost"));
                await client.ConnectAsync(serverUrI, CancellationToken.None);
                // 连接成功了
                this.btnConnect.Hide();
                this.btnClose.Show();
                this.btnSend.Enabled = true;
            }
            catch(Exception ex)
            {
                this.txtServerUrl.Enabled = true;
                this.btnConnect.Show();
                this.btnClose.Hide();
                this.btnSend.Enabled = false;
                MessageBox.Show(ex.Message);
                return;
            }

            KeepReceiving(client);
        }

        // 保持接收消息
        public async void KeepReceiving(ClientWebSocket client)
        {
            // 设置一个10K的缓冲区
            byte[] buffer = new byte[10 * 1024];
            IList<byte> byteList = new List<byte>();
            while (true)
            {
                buffer = new byte[10 * 1024];
                var result =await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.CloseStatus.HasValue)
                {
                    // 收到了关闭连接的消息，断开连接
                    // 被动关闭连接
                    CloseConnection(client, result.CloseStatus.Value, result.CloseStatusDescription);
                    break;
                }

                for (int i = 0; i < result.Count; i++)
                {
                    byteList.Add(buffer[i]);
                }

                if (result.EndOfMessage)
                {
                    // 一条消息接收完了
                    var jsonString = Encoding.UTF8.GetString(byteList.ToArray());
                    var messageInfo = JsonConvert.DeserializeObject<MessageInfo>(jsonString);
                    this.BeginInvoke(new Action<MessageInfo>(ShowMessage),messageInfo);

                    byteList.Clear();
                }
            }
        }

        public async Task CloseConnection(ClientWebSocket client,WebSocketCloseStatus closeStatus=WebSocketCloseStatus.NormalClosure,string statusDescription="")
        {
            await client.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
            client.Dispose();
            this.btnConnect.Show();
            this.btnClose.Hide();
            this.btnSend.Enabled = false;
        }

        private void ShowMessage(MessageInfo messageInfo)
        {
            var fromUser = messageInfo.FromUser;
            var message = messageInfo.Message;
            this.txtContent.AppendText(fromUser + ":" + message + "\r\n");
        }

        // 发送消息
        private async void btnSend_Click(object sender, EventArgs e)
        {
            var message = this.txtInput.Text;
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("发送的消息不能为空");
                return;
            }
            MessageInfo messageInfo = new MessageInfo
            {
                FromUser="wulinhao",
                Message=message,
                IsToAll=true
            };
            await Send(messageInfo);
        }

        private async Task Send(MessageInfo messageInfo)
        {
            var jsonString = JsonConvert.SerializeObject(messageInfo);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            if (client != null && client.State == WebSocketState.Open)
            {
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                MessageBox.Show("WebSocket未连接");
                return;
            }
        }

        private async void btnClose_Click(object sender, EventArgs e)
        {
            await CloseConnection(client);
        }
    }

    public class MessageInfo
    {
        public string FromUser { get; set; }
        public string Message { get; set; }
        public bool IsToAll { get; set; }
        public string ToUser { get; set; }
    }
}
