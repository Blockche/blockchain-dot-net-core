using Blockche.Blockchain.Web.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System;

using System.Threading.Tasks;

namespace Blockche.Blockchain.Web.Hubs
{
    public class ExplorerHub : Hub
    {
        public async Task SendMessage( string message)
        {
         

            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            
            await base.OnConnectedAsync();
            var viewModel = GetData();
            await Clients.All.SendAsync("ReceiveMessage", viewModel);
        }

        public async Task CheckForNewData()
        {
        
            var viewModel = GetData();

            await Clients.All.SendAsync("ReceiveMessage", viewModel);
        }

        private object GetData()
        {
            var req = Context.GetHttpContext().Request;
            var node = Helper.GetNodeSingleton(req);
            var viewModel = new
            {
                node.Peers,
                node.Chain.Blocks,
                node.Chain.PendingTransactions,
                ConfirmedTran = node.Chain.GetConfirmedTransactions()
            };


            return viewModel;
        }

        //public override async Task OnConnectedAsync()
        //{
        //    await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "joined");
        //}

        //public override async Task OnDisconnectedAsync(Exception ex)
        //{
        //    await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "left");
        //}

        //public async Task Send(string message)
        //{
        //    await Clients.All.SendAsync("SendMessage", Context.User.Identity.Name, message);
        //}
    }
}
