using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApp1
{
    public class MyWebSocketChatMiddleware
    {
        private readonly WebSocketManager webSocketManager;
        private readonly RequestDelegate next;

        public MyWebSocketChatMiddleware(WebSocketManager webSocketManager,RequestDelegate next)
        {
            this.webSocketManager = webSocketManager;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws/chat")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket =await context.WebSockets.AcceptWebSocketAsync();
                    await this.webSocketManager.OnWebSocketConnected(context, webSocket);//这里建立连接之后应该就要一直阻塞接收消息，否则这个方法执行完毕连接就断开了
                    return;
                }
            }

            await next(context);
        }
    }
}
