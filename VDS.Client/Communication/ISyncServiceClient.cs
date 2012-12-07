
using System.ServiceModel;
namespace CIS681.Fall2012.VDS.Communication {
    /// <summary>
    /// A default messenger will use string as default type
    /// No callback here
    /// </summary>
    [ServiceContract(Name = "StringMessenger", SessionMode = SessionMode.Required, CallbackContract = typeof(IPushMessager<string>))]
    internal interface ISyncServiceClient : IMessenger<string> { }
}
