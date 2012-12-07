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
        /// time of last modification
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
            get { return string.IsNullOrWhiteSpace(title) ? "(Untitled)" : title; }
            set {
                if (title == value) return;
                if (string.IsNullOrWhiteSpace(value))
                    title = null;
                else
                    title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// If the object is initialized
        /// </summary>
        public bool IsInitialized { get; private set; }
        #endregion

        #region Serialization
        /// <summary>
        /// Before serializing
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        private void OnDataDeserializing(StreamingContext context) {
            InitializingData();
        }
        /// <summary>
        /// After deserialized
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDataDeserialized(StreamingContext context) {
            InitializedData();
        }

        [OnSerializing]
        private void OnDataSerializing(StreamingContext context) {
            FinalizingData();
        }
        [OnSerialized]
        private void OnDataSerialized(StreamingContext context) {
            FinalizedData();
        }
        #endregion

        #region Constructors
        public BaseData() {
            ID = Guid.NewGuid();
            InitializingData();
            InitializedData();
        }
        /// <summary>
        /// Should be implemented by server/client, used to load additional data
        /// </summary>
        protected virtual void InitializingData() { InitializingBaseData(); }
        protected virtual void InitializedData() { InitializedBaseData(); IsInitialized = true; }
        protected virtual void FinalizingData() { FinalizingBaseData(); }
        protected virtual void FinalizedData() { FinalizedBaseData(); }
        protected virtual void InitializingBaseData() { }
        protected virtual void InitializedBaseData() { }
        protected virtual void FinalizingBaseData() { }
        protected virtual void FinalizedBaseData() { }
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
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            // change last modify time
            lastModifyTime = DateTime.UtcNow;
            OnDataUpdated();
        }
        #endregion

        #region Data Updated Notifier
        public delegate void DataUpdatedEventHandler(BaseData data);
        public event DataUpdatedEventHandler DataUpdated = null;
        /// <summary>
        /// When data is updated, trigger this event
        /// </summary>
        public void OnDataUpdated() {
            if (IsInitialized && DataUpdated != null) DataUpdated(this);
        }
        #endregion
    }
}
