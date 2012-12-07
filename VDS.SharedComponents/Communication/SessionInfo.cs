/*
 * Session information
 */

namespace CIS681.Fall2012.VDS.Communication {
    public struct SessionInfo {
        public string UserName { get; set; }
        public string SessionId { get; set; }
        public IPushMessager<string> Callback { get; set; }
    }
}
