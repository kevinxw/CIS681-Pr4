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
    public partial class Project : BaseData {

        #region Properties
        // where the project is saved, just for session use
        public string SavePath { get; private set; }

        // the diagrams contained in the project
        [DataMember(Name = "Diagrams", EmitDefaultValue = false, Order = 0)]
        //private UIElementCollection children { get { return Children; } }
        private List<Diagram> children;

        // the activated diagram in this project
        [DataMember(Name = "ActivatedDiagram", EmitDefaultValue = false, Order = 1)]
        private Diagram activatedDiagram = null;
        // activate one diagram (tab)
        public Diagram ActivatedDiagram {
            get {
                if (activatedDiagram == null) {
                    for (int i = 0; i < children.Count && activatedDiagram == null; i++) {
                        if (children[i].IsOpen)
                            activatedDiagram = children[i];
                    }
                    // the value is changed
                    if (activatedDiagram != null)
                        OnPropertyChanged("ActivatedDiagram");
                }
                return activatedDiagram;
            }
            set {
                if (activatedDiagram == value)
                    return;
                activatedDiagram = value;
                OnPropertyChanged("ActivatedDiagram");
            }
        }
        #endregion

        protected override void RefreshBaseData() {
            if (children == null)
                children = new List<Diagram>();
        }
    }
}
