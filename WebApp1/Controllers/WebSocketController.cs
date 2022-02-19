using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Text;

namespace WebApp1.Controllers
{
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly WebSocketManager webSocketManager;

        public WebSocketController(WebSocketManager webSocketManager)
        {
            this.webSocketManager = webSocketManager;
        }

        [Route("ws/broadcast")]
        public async Task BroadCast(string message)
        {
            MessageInfo messageInfo = new MessageInfo
            {
                FromUser = "SystemAdmin",
                IsToAll = true,
                Message = message
            };
            await this.webSocketManager.SendToAll(messageInfo);
        }
    }
}
