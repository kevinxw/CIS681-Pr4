using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ServiceModel;

namespace CIS681.Fall2012.VDS.Communication {
    class Session {
        public ushort Port { get; set; }
        public IPAddress Address { get; set; }
        public string UserName { get; set; }
        public uint UserID { get; set; }
    }
}
