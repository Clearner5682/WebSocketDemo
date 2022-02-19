using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApp1
{
    public class WebSocketManager
    {
        private readonly object lockObj = new object();
        public IList<WebSocketInfo> WebSockets { get; set; }

        public WebSocketManager()
        {
            if (WebSockets == null)
            {
                WebSockets = new List<WebSocketInfo>();
            }
        }

        // WebSocket的连接建立了
        public async Task OnWebSocketConnected(HttpContext httpContext,WebSocket webSocket)
        {
            // 这里实际上应该用HttpContext中的登录信息来赋值WebSocket用户
            // 还需要考虑，当用户退出登录之后，服务器端主动断开WebSocket连接
            // 为了简单起见，这里就用Cookies中的用户信息来模拟
            WebSocketInfo webSocketInfo = null;
            lock (lockObj)
            {
                Guid sessionId = Guid.NewGuid();
                webSocketInfo = new WebSocketInfo 
                { 
                    SessionId = sessionId, 
                    WebSocket = webSocket,
                    UserId=httpContext.Request.Cookies["UserId"]
                };
                WebSockets.Add(webSocketInfo);
            }
            if (webSocketInfo != null)
            {
                await Receive(webSocketInfo);
            }
        }

        // WebSocket连接关闭了
        public async Task OnWebSocketClosed(WebSocketInfo webSocketInfo)
        {
            await webSocketInfo.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            if (webSocketInfo.WebSocket != null)
            {
                webSocketInfo.WebSocket.Dispose();
            }
            lock (lockObj)
            {
                WebSockets.Remove(webSocketInfo);
            }
        }

        // 保持接收数据
        public async Task Receive(WebSocketInfo webSocketInfo)
        {
            await Task.Run(async () =>
            {
                IList<byte> byteList = new List<byte>();
                while (true)
                {
                    var buffer = new byte[4*1024];

                    WebSocketReceiveResult result = await webSocketInfo.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.CloseStatus.HasValue)
                    {
                        break;
                    }

                    foreach (byte b in buffer)
                    {
                        byteList.Add(b);
                    }

                    if (result.EndOfMessage)//是消息结尾了，表示这条消息接收完了
                    {
                        var jsonStr = Encoding.UTF8.GetString(byteList.ToArray());
                        byteList.Clear();
                        MessageInfo messageInfo = JsonConvert.DeserializeObject<MessageInfo>(jsonStr);
                        if (messageInfo.IsToAll)
                        {
                            await SendToAll(messageInfo);
                        }
                        else
                        {
                            var targetWebSocket = WebSockets.FirstOrDefault(o => o.UserId == messageInfo.ToUser);
                            if (targetWebSocket != null)
                            {
                                await Send(targetWebSocket, messageInfo);
                            }
                        }
                    }
                }


                await OnWebSocketClosed(webSocketInfo);
            });
        }

        // 发送消息到指定的WebSocket连接
        public async Task Send(WebSocketInfo webSocketInfo,MessageInfo messageInfo)
        {
            await Task.Run(async () =>
            {
                var byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageInfo));
                var arraySegment = new ArraySegment<byte>(byteArray);

                try
                {
                    await webSocketInfo.WebSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            });
        }

        // 发送消息到所有的WebSocket连接
        public async Task SendToAll(MessageInfo messageInfo)
        {
            // 转发到所有的WebSocket连接
            foreach (var websocketInfo in WebSockets)
            {
                await Send(websocketInfo, messageInfo);
            }
        }
    }

    public class WebSocketInfo
    {
        // 每建立一个WebSocket连接就生成一个SessionId
        public Guid SessionId { get; set; }
        // WebSocket对象
        public WebSocket WebSocket { get; set; }
        public string UserId { get; set; }
    }

    public class MessageInfo
    {
        public string FromUser { get; set; }
        public string Message { get; set; }
        public bool IsToAll { get; set; }
        public string ToUser { get; set; }
    }
}
