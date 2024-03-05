using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DMS_Authontication1.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DMS_Authontication1
{
    public class ProposalNotificationHub : Hub
    {

        [HubMethodName("sendProposalMessages")]
        public  void SendProposalMessages()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ProposalNotificationHub>();
            context.Clients.All.updateMessages();
        }
        //public void Hello()
        //{
        //    Clients.All.hello();
        //}
        private static readonly ConcurrentDictionary<string, UserHubModels> Users =
        new ConcurrentDictionary<string, UserHubModels>(StringComparer.InvariantCultureIgnoreCase);


        private ApplicationDbContext context = new ApplicationDbContext();

        //Logged Use Call  
        public void GetNotification()
        {
            try
            {
                string loggedUser = Context.User.Identity.Name;

                //Get TotalNotification  
                string totalNotif = LoadNotifData(loggedUser);

                //Send To  
                UserHubModels receiver;
                if (Users.TryGetValue(loggedUser, out receiver))
                {
                    var cid = receiver.ConnectionIds.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<ProposalNotificationHub>();
                    context.Clients.Client(cid).broadcaastNotif(totalNotif);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        //Specific User Call  
        public void SendNotification(string SentTo)
        {
            try
            {
                //Get TotalNotification  
                string totalNotif = LoadNotifData(SentTo);

                //Send To  
                UserHubModels receiver;
                if (Users.TryGetValue(SentTo, out receiver))
                {
                    var cid = receiver.ConnectionIds.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<ProposalNotificationHub>();
                    context.Clients.Client(cid).broadcaastNotif(totalNotif);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public string LoadNotifData(string userId)
        {
            int total = 0;
            var query = (from t in context.ProposalNotifications
                         where t.SentTo == userId
                         select t)
                        .ToList();
            total = query.Count;
            return total.ToString();
        }
        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new UserHubModels
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userName);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            UserHubModels user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        UserHubModels removedUser;
                        Users.TryRemove(userName, out removedUser);
                        Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}