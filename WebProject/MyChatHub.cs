using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebProject
{
    /// <summary>
    /// The client must use camel-cased names to RPC this Hub and its methods.
    /// JS Example: 
    ///   var chatHub = $.connection.myChatHub;
    ///   chatHub.server.broadcastMessage("dzy", "Hello all!");
    /// </summary>
    public class MyChatHub : Hub
    {
        public async Task BroadcastMessage(string callerName, string message)
        {
            // Case-insensitive when the server RPC the client's methods
            await Clients.All.appendnewmessage(callerName, message);
        }
        public async Task SendLoginMessage(string callerName)
        {
            // Case-insensitive when the server RPC the client's methods

            await Clients.Group("homepage").appendnewmessage("",callerName+": login");
        }
        public async Task SendLogoutMessage(string callerName)
        {
            // Case-insensitive when the server RPC the client's methods

            await Clients.Group("homepage").appendnewmessage("", callerName + ": logout");
        }
        public async Task SendGroupMessage(string callerName, string groupName, string title, string message)
        {
            // Case-insensitive when the server RPC the client's methods
            await Clients.Group(groupName).appendnewmessage(callerName, message);
            if (groupName != "homepage")
            await Clients.Group("homepage").appendnewmessage(title, message);
        }


        public string JoinGroup(string connectionId, string groupName)
        {
            try
            {
            Groups.Add(connectionId, groupName).Wait();
            }
            catch (Exception ex)
            {
               string err= ex.Message;
            }
            return connectionId + " joined " + groupName;
        }

        public string LeaveGroup(string connectionId, string groupName)
        {
            Groups.Remove(connectionId, groupName).Wait();
            return connectionId + " removed " + groupName;
        }

        //public void DisplayMessageAll(string message)
        //{
        //    Clients.All.displayMessage("Clients.All: " + message + " from " + Context.ConnectionId);
        //}

        ////public void DisplayMessageAllExcept(string message, params string[] excludeConnectionIds)
        ////{
        ////    Clients.AllExcept(excludeConnectionIds).displayMessage("Clients.AllExcept: " + message + " from " + Context.ConnectionId);
        ////}

        //public void DisplayMessageOther(string message)
        //{
        //    Clients.Others.displayMessage("Clients.Others: " + message + " from " + Context.ConnectionId);
        //}

        //public void DisplayMessageCaller(string message)
        //{
        //    Clients.Caller.displayMessage("Clients.Caller: " + message + " from " + Context.ConnectionId);
        //}

        //public void DisplayMessageSpecified(string targetConnectionId, string message)
        //{
        //    Clients.Client(targetConnectionId).displayMessage("Clients.Client: " + message + " from " + Context.ConnectionId);
        //}

        //public void DisplayMessageGroup(string groupName, string message)
        //{
        //    Clients.Group(groupName).displayMessage("Clients.Group: " + message + " from " + Context.ConnectionId);
        //}

        //public void DisplayMessageGroupExcept(string groupName, string message, params string[] excludeConnectionIds)
        //{
        //    Clients.Group(groupName, excludeConnectionIds).displayMessage("Clients.Group: " + message + " from " + Context.ConnectionId);
        //}

        //public void DisplayMessageOthersInGroup(string groupName, string message)
        //{
        //    Clients.OthersInGroup(groupName).displayMessage("Clients.OthersInGroup: " + message + " from" + Context.ConnectionId);
        //}

        private static List<string> users = new List<string>();
        public override Task OnConnected()
        {
            users.Add(Context.ConnectionId);
            return base.OnConnected();
        }
        //public override Task OnConnected()
        //{
        //    return Clients.All.displayMessage(Context.ConnectionId + " OnConnected");
        //}

        public override Task OnReconnected()
        {
            return Clients.Caller.displayMessage(Context.ConnectionId + " OnReconnected");
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
        //public override Task OnDisconnected()
        //{
        //    return Clients.All.displayMessage(Context.ConnectionId + " OnDisconnected");
        //}

    }
}