using System;
using System.ServiceModel;
using System.Threading;
using CIS681.Fall2012.VDS.Data;
using CIS681.Fall2012.VDS.UI;

namespace CIS681.Fall2012.VDS.Communication {

    internal class SyncServiceClient {

        private ISyncServiceClient clientProxy;
        private MainWindow window;

        private bool threadRunningFlag = false;

        private Thread thread;

        public SyncServiceClient(MainWindow win) {
            window = win;
            thread = new Thread(new ThreadStart(UpdateProject));
            thread.Priority = ThreadPriority.Normal;
            thread.IsBackground = true;
            State = ConnectionState.Uninitialized;
        }

        public void Start() {
            // return when the thread is running
            if (thread.ThreadState == (ThreadState.Unstarted | ThreadState.Background)) thread.Start();
#if DEBUG_ON
            Console.WriteLine("thread state {0}",thread.ThreadState);
#endif
            threadRunningFlag = true;
        }

        public void Connect() {
            if (clientProxy == null) {  // create channel here
                State = ConnectionState.Connecting;
                SyncServiceClientCallback callback = new SyncServiceClientCallback(window);
                // Read server address from configuration
                string serverAddress = System.Configuration.ConfigurationManager.AppSettings["SyncServiceAddress"];
#if DEBUG_ON
            Console.WriteLine("serverAddress {0}",serverAddress);
#endif
                DuplexChannelFactory<ISyncServiceClient> factory = new DuplexChannelFactory<ISyncServiceClient>(callback, new NetTcpBinding(), new EndpointAddress(serverAddress));
                factory.Open();
                clientProxy = factory.CreateChannel();
                // try sign in
                try {
                    SessionInfo info = new SessionInfo();
                    info.UserName = Environment.UserName;
                    clientProxy.SignIn(info);
                    State = ConnectionState.Connected;
                }
                catch (Exception ex) {  // unable to connect
                    clientProxy = null;
                    State = ConnectionState.Disconnected;
                    new ErrorHandler(ex);
                }
            }
        }

        public ConnectionState State { get; private set; }

        public void UpdateProject() {
            while (true) {
                Thread.Sleep(30);  // this sleep time actually controls sync frequency
                if (!threadRunningFlag || !window.IsProjectSynchronizationEnabled) continue;
                threadRunningFlag = false;
                Connect();
                if (State == ConnectionState.Connected)
                    // try to push update to server
                    try {
                        if (Project.Current == null) continue;
                        string data = DataSerializer<Project>.Serialize(Project.Current);
                        clientProxy.Broadcast(data);
                    }
                    catch (Exception ex) {  // unable to connect
                        clientProxy = null;
                        State = ConnectionState.Disconnected;
                        new ErrorHandler(ex);   // log error
                        continue;
                    }
            }
        }

    }


    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    internal partial class SyncServiceClientCallback : IPushMessager<string> {
        private MainWindow window;
        public SyncServiceClientCallback(MainWindow win) {
            window = win;
        }

        /// <summary>
        /// Update local project
        /// </summary>
        /// <param name="obj"></param>
        public void Push(string obj) {
#if DEBUG_ON
            Console.WriteLine("Push request received from server.");
#endif
            try {
                window.ReloadProject(DataSerializer<Project>.Deserialize(obj));
            }
            catch (Exception ex) { new ErrorHandler(ex); }
        }
    }
}
