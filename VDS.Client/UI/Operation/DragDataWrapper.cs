/*
 * A wrapper that encapsulate data
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Windows;

namespace CIS681.Fall2012.VDS.UI.Operation {
    internal class DragDataWrapper {
        /// <summary>
        /// Main content
        /// </summary>
        public object Content { get; set; }
        /// <summary>
        /// Operation Type, what is the data used for
        /// </summary>
        public DragOperationType Type { get; set; }
        /// <summary>
        /// The position where the object is being dragged from
        /// </summary>
        public Point DragStartPosition { get; set; }

        #region Constructors
        public DragDataWrapper() {
            DragStartPosition = new Point(0, 0);
            Type = DragOperationType.None;
        }
        public DragDataWrapper(object data) :this(){
            Content = data;
        }
        public DragDataWrapper(object data, DragOperationType type)
            : this(data) {
            Type = type;
        }
        public DragDataWrapper(object data, DragOperationType type, Point dragStartPosition)
            : this(data, type) {
            DragStartPosition = dragStartPosition;
        }
        #endregion
    }
}
