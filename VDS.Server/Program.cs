/*
 * 
 */

using System;
using System.ServiceModel;

namespace VDS.Server {
    class Program {
        static void Main(string[] args) {
            using (ServiceHost host = new ServiceHost(typeof(CIS681.Fall2012.VDS.Server.SyncService))) {
                host.Open();
                Console.WriteLine("VDS Sync Service Started...");
                Console.WriteLine("Press enter to stop the service.");
                Console.ReadLine();  // prevent program from stoppings
                host.Close();
            }
        }
    }
}
