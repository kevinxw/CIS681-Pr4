/*
 * Types of connectors, which indicate the location of one connector
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System;
using System.Runtime.Serialization;
namespace CIS681.Fall2012.VDS.Data.Objects {
    [DataContract(Name = "ConnectorType")]
    public enum ConnectorType {
        // notice the value are carefully assigned, we can get opposite position by (type-2), then negative the value
        [EnumMember]
        Top = 0,
        [EnumMember]
        Left = 1,
        [EnumMember]
        Center = 2,
        [EnumMember]
        Right = 3,
        [EnumMember]
        Bottom = 4
    }
}
