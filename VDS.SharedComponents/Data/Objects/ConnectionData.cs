/*
 * Data struct of connection
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;

namespace CIS681.Fall2012.VDS.Data {
    [DataContract(Name = "Connection", IsReference = true)]
    public class ConnectionData : BaseObject {

        #region Properties
        /// <summary>
        /// Source Connector
        /// </summary>
        [DataMember(Name = "Source", EmitDefaultValue = false)]
        protected ConnectorData source = null;
        public ConnectorData Source {
            get { return source; }
            set {
                if (source != null) {   // bind events
                    source.Owner.PropertyChanged -= OnSourcePositionChanged;
                    source.Owner.PropertyChanged -= OnSourceSizeChanged;
                    source.Connections.Remove(this);
                }
                if ((source = value) != null) {
                    source.Connections.Add(this);
                    source.Owner.PropertyChanged += OnSourcePositionChanged;
                    source.Owner.PropertyChanged += OnSourceSizeChanged;
                    sourcePosition = source.Position;
                }
                OnPropertyChanged("Source");
                // trigger when source is changed
                OnPropertyChanged("Position");
            }
        }

        /// <summary>
        /// Record the position of source, in case source connector is deleted
        /// </summary>
        [DataMember(Name = "SourcePosition", EmitDefaultValue = false)]
        protected Point sourcePosition = new Point(double.NaN, double.NaN);
        public Point SourcePosition {
            get { if (Source != null) return Source.Position; else return sourcePosition; }
            set { sourcePosition = value; }
        }

        /// <summary>
        /// Sink Connector
        /// </summary>
        [DataMember(Name = "Sink", EmitDefaultValue = false)]
        protected ConnectorData sink = null;
        public ConnectorData Sink {
            get { return sink; }
            set {
                if (sink != null) {   // bind events
                    sink.Owner.PropertyChanged -= OnSourcePositionChanged;
                    sink.Owner.PropertyChanged -= OnSourceSizeChanged;
                    sink.Connections.Remove(this);
                }
                if ((sink = value) != null) {
                    sink.Connections.Add(this);
                    sink.Owner.PropertyChanged += OnSourcePositionChanged;
                    sink.Owner.PropertyChanged += OnSourceSizeChanged;
                    sinkPosition = sink.Position;
                }
                OnPropertyChanged("Sink");
            }
        }

        /// <summary>
        /// Record the position of sink, in case source connector is deleted
        /// </summary>
        [DataMember(Name = "SinkPosition", EmitDefaultValue = false)]
        protected Point sinkPosition = new Point(double.NaN, double.NaN);
        public Point SinkPosition {
            get { if (Sink != null) return Sink.Position; else return sinkPosition; }
            set { sinkPosition = value; }
        }

        /// <summary>
        /// For orthogonal style connections, the position of stops of current connection
        /// </summary>
        [DataMember(Name = "Stops")]
        protected List<Point> Stops { get; private set; }

        /// <summary>
        /// Which diagram does this connection belongs to
        /// </summary>
        [DataMember(Name = "Owner")]
        public DiagramData Owner { get; set; }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When source's position / size is changed, the connection item may be updated (recaculate path maybe)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSourcePositionChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Position") return;
            if (source != null)
                sourcePosition = source.Position;
            OnPropertyChanged("Source.Position");
            OnPropertyChanged("Position");
        }
        private void OnSourceSizeChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Size") return;
            OnPropertyChanged("Source.Size");
        }
        private void OnSinkPositionChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Position") return;
            if (sink != null)
                sinkPosition = sink.Position;
            OnPropertyChanged("Sink.Position");
        }
        private void OnSinkSizeChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Size") return;
            OnPropertyChanged("Sink.Size");
        }
        #endregion

        protected override void AfterInitializingData() {
            base.AfterInitializingData();
            if (Stops == null)
                Stops = new List<Point>();
            InitEventHandler();
        }

        /// <summary>
        /// Bind event handlers, etc.
        /// </summary>
        public void InitEventHandler() {
            if (source != null) {
                source.Owner.PropertyChanged += OnSourcePositionChanged;
                source.Owner.PropertyChanged += OnSourceSizeChanged;
            }
            if (sink != null) {
                sink.Owner.PropertyChanged += OnSinkPositionChanged;
                sink.Owner.PropertyChanged += OnSinkSizeChanged;
            }
        }

    }
}
