using System.ServiceModel;
using CIS681.Fall2012.VDS.Communication;

namespace CIS681.Fall2012.VDS.Server {

    /// <summary>
    /// A default messenger will use string as default type
    /// </summary>
    [ServiceContract(Name = "StringMessenger", SessionMode = SessionMode.Required, CallbackContract = typeof(IPushMessager<string>))]
    internal interface ISyncService : IMessenger<string> {
    }
}
