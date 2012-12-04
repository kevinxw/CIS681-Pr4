using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace CIS681.Fall2012.VDS.Communication {
    [ServiceContract]
    public interface IMessenger<T> {


        [OperationContract]
        public void Send(T obj);
        [OperationContract]
        public void Send(T obj, Session session);
        [OperationContract]
        public void Broadcast();
    }
}
