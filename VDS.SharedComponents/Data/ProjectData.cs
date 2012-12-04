/*
 * A project contains diagrams
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CIS681.Fall2012.VDS.Data {
    [DataContract(Name = "Project", IsReference = true)]
    public class ProjectData : BaseData {

        #region Properties
        // the diagrams contained in the project
        [DataMember(Name = "Diagrams", EmitDefaultValue = false, Order = 0)]
        public List<DiagramData> Children { get; private set; }

        // the activated diagram in this project
        [DataMember(Name = "ActivatedDiagram", EmitDefaultValue = false, Order = 1)]
        private DiagramData activatedDiagram = null;
        public DiagramData ActivatedDiagram {
            get {
                if (activatedDiagram == null) {
                    // if no diagram is activated, find the first opened one
                    for (int i = 0; i < Children.Count && activatedDiagram == null; i++)
                        if (Children[i].IsOpen) activatedDiagram = Children[i];
                    // the value is changed
                    if (activatedDiagram != null) OnPropertyChanged("ActivatedDiagram");
                }
                return activatedDiagram;
            }
            set {
                if (activatedDiagram == value) return;
                activatedDiagram = value;
                OnPropertyChanged("ActivatedDiagram");
            }
        }
        #endregion

        /// <summary>
        /// Initialize data
        /// </summary>
        protected override void AfterInitializingData() {
            base.AfterInitializingData();
            if (Children == null) Children = new List<DiagramData>();
        }
    }
}
