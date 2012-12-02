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
using CIS681.Fall2012.VDS.Data.Objects;

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
        #endregion

        #region Serialization
        /// <summary>
        /// Before serializing
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        private void OnDataDeserializing(StreamingContext context) {
            InitBaseData();
            InitData();
        }
        /// <summary>
        /// After deserialized
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDataDeserialized(StreamingContext context) {
            RefreshBaseData();
            RefreshData();
        }
        #endregion

        #region Constructors
        public BaseData() {
            ID = Guid.NewGuid();
            InitBaseData();
            InitData();
            RefreshBaseData();
            RefreshData();
        }
        /// <summary>
        /// Should be implemented by server/client, used to load additional data
        /// </summary>
        protected virtual void InitData() { }
        protected virtual void RefreshData() { }
        /// <summary>
        /// Should be implemented by inherited children, used to initialize basic data
        /// </summary>
        protected virtual void InitBaseData() { }
        protected virtual void RefreshBaseData() { }
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
            LastModifyTime = DateTime.Now;
        }
        #endregion
    }
}
