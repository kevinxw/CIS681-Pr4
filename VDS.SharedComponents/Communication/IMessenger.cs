using System;
using System.ServiceModel;
using System.Collections.Generic;

namespace CIS681.Fall2012.VDS.Communication {
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IMessenger<T> : IPushMessager<T> {
        /// <summary>
        /// Login and register it to the session
        /// </summary>
        /// <returns></returns>
        [OperationContract(IsOneWay = true,IsInitiating = true)]
        void SignIn(SessionInfo session);

        /// <summary>
        /// Logout
        /// </summary>
        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = true)]
        void SignOut();

        /// <summary>
        /// Send object to all peers in session list
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="exceptions">The list of sessions will not be sent message</param>
        [OperationContract(IsOneWay = true, IsInitiating = false)]
        void Broadcast(T obj);
    }

    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IPushMessager<T> {
        /// <summary>
        /// Send object to default session
        /// </summary>
        /// <param name="obj"></param>
        [OperationContract(IsOneWay = true)]
        void Push(T obj);
    }
}
