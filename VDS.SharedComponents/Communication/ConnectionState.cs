using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIS681.Fall2012.VDS.Communication {
    public enum ConnectionState {
        Uninitialized,
        Connecting,
        Connected,
        Disconnected
    }
}
