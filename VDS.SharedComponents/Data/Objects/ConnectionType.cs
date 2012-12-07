/*
 * Relation types, currently only implement these four
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Runtime.Serialization;
namespace CIS681.Fall2012.VDS.Data.Objects {
    [DataContract(Name = "ConnectorType", Namespace = "http://VDS.Data")]
    public enum ConnectionType {
        [EnumMember]
        Composition = 0,
        [EnumMember]
        Inheritance = 1,
        [EnumMember]
        Using = 2,
        [EnumMember]
        Aggregation = 3
    }
}
