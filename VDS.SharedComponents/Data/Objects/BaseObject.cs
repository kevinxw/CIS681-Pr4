/*
 * Basic object data structure, both model and connection should implement these feature
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Runtime.Serialization;
using System.Windows;

namespace CIS681.Fall2012.VDS.Data.Objects {
    [DataContract(Name = "Object", IsReference = true)]
    public abstract class BaseObject : BaseData {
        #region Properties
        /// <summary>
        /// Model Type
        /// </summary>
        [DataMember(Name = "Type", EmitDefaultValue = false)]
        private string type = null;
        public string Type {
            get { return type; }
            set {
                if (type == value) return;
                type = value;
                OnPropertyChanged("Type");
            }
        }

        /// <summary>
        /// Position of Object
        /// </summary>
        [DataMember(Name = "Position", EmitDefaultValue = false)]
        private Point position = new Point(double.NaN, double.NaN);
        public Point Position {
            get { return position; }
            set {
                if (position.Equals(value)) return;
                position = value;
                OnPropertyChanged("Position");
            }
        }
        #endregion
    }
}
