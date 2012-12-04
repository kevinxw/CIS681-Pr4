/*
 * Basic data structure for most objects in this sytem
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CIS681.Fall2012.VDS.Data {
    [DataContract(Name = "Data", IsReference = true)]
    public abstract class BaseData : INotifyPropertyChanged {
        #region Properties
        /// <summary>
        /// Unique ID of one object
        /// </summary>
        [DataMember(Name = "ID")]
        public Guid ID { get; private set; }

        /// <summary>
        /// Time of last modification
        /// </summary>
        [DataMember(Name = "LastModifyTime", EmitDefaultValue = false)]
        private DateTime lastModifyTime = DateTime.Now.ToUniversalTime();
        public DateTime LastModifyTime {
            get { return lastModifyTime.ToLocalTime(); }
            set { lastModifyTime = value.ToUniversalTime(); }
        }

        /// <summary>
        /// The name of one object
        /// </summary>
        [DataMember(Name = "Title", EmitDefaultValue = false)]
        private string title = null;
        public string Title {
            get { return title; }
            set {
                if (title == value) return;
                if (string.IsNullOrWhiteSpace(value)) title = null; else title = value;
                OnPropertyChanged("Title");
            }
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Before deserializing
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        private void OnDataDeserializing(StreamingContext context) { BeforeInitializingData(); }

        /// <summary>
        /// After deserialized
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDataDeserialized(StreamingContext context) { AfterInitializingData(); }

        /// <summary>
        /// Before data serializing
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing]
        private void OnDataSerializing(StreamingContext context) { BeforeFinalizingData(); }

        /// <summary>
        /// After data is serialized
        /// </summary>
        /// <param name="context"></param>
        [OnSerialized]
        private void OnDataSerialized(StreamingContext context) { AfterFinalizingData(); }
        #endregion

        #region Constructors
        public BaseData() {
            ID = Guid.NewGuid();
            BeforeInitializingData();
            AfterInitializingData();
        }

        /// <summary>
        /// Should be overrided by inherited children, used to initialize basic data
        /// </summary>
        protected virtual void BeforeInitializingData() { }
        protected virtual void AfterInitializingData() { }
        /// <summary>
        /// After data is prepared and before it is serialized
        /// </summary>
        protected virtual void BeforeFinalizingData() { }
        protected virtual void AfterFinalizingData() { }
        #endregion

        /// <summary>
        /// Check whether two objects are the same by comparing ID
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(BaseData obj) {
            return obj.ID.Equals(ID);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = null;
        protected void OnPropertyChanged(string name) {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
            // last modify time
            LastModifyTime = DateTime.Now;
        }
        #endregion
    }
}
