using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using CIS681.Fall2012.VDS.Communication;

namespace CIS681.Fall2012.VDS.Server {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    internal class SyncService : ISyncService {

        private static ConcurrentDictionary<string, SessionInfo> Sessions = new ConcurrentDictionary<string, SessionInfo>();
        /// <summary>
        /// Record every synced object in this stack, use TryPeek to Pick, use TryPop to rollback history
        /// </summary>
        private static ConcurrentStack<string> syncHistory = new ConcurrentStack<string>();

        private IPushMessager<string> client = null;

        public SyncService() {
            client = OperationContext.Current.GetCallbackChannel<IPushMessager<string>>();
        }

        public void SignIn(SessionInfo session) {
            session.Callback = client;
            session.SessionId = OperationContext.Current.SessionId;
            Console.WriteLine("{0} User {1} signs in", DateTime.Now.Millisecond, session.UserName);
            Sessions.AddOrUpdate(session.SessionId, session, (key, value) => session);
            string res = null;
            if (syncHistory.TryPeek(out res))  // get the latest object and then push it to client
                client.Push(res);
        }

        public void SignOut() {
            SessionInfo info = new SessionInfo();
            if (Sessions.TryRemove(OperationContext.Current.SessionId, out info))
                Console.WriteLine("{0} User {1} signs out", DateTime.Now.Millisecond, info.UserName);
        }

        public void Broadcast(string obj) {
            SessionInfo info = Sessions[OperationContext.Current.SessionId];
            Console.WriteLine("{0} Broadcast request received from user {1}", DateTime.Now.Millisecond, info.UserName);
            syncHistory.Push(obj);
            // broadcast it
            foreach (string key in Sessions.Keys)
                if (key != OperationContext.Current.SessionId)
                    try {
                        Sessions[key].Callback.Push(obj);
                    }
                    catch (Exception e) {   // remove the channels not existed anymore
                        Sessions.TryRemove(key, out info);
                    }
        }

        // push only, do not broadcast
        public void Push(string obj) {
            SessionInfo info = Sessions[OperationContext.Current.SessionId];
            Console.WriteLine("{0} Push request received from user {1}", DateTime.Now.Millisecond, info.UserName);
            syncHistory.Push(obj);
        }
    }
}
