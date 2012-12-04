using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIS681.Fall2012.VDS.Communication {
    public class SessionManager {
        public List<Session> Sessions { get; private set; }

        public SessionManager() {
            Sessions = new List<Session>();
        }
    }
}
