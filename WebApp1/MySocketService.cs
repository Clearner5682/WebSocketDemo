using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp1
{
    public class MySocketService : BackgroundService
    {
        private readonly MySocketServer socketServer;

        public MySocketService(MySocketServer socketServer)
        {
            this.socketServer = socketServer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.socketServer.KeepAccepting();
            var test = 1;
        }
    }
}
