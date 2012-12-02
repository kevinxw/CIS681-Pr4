/*
 * This class is used to store some more information, it is actually a data structure
 */

//////
/// Nothing to be TESTED here!
//////

using System.Windows;
using System.Windows.Controls.Primitives;

namespace CIS681.Fall2012.VDS.UI.Adorner {
    internal class AdornerDataThumb<P, D> : Thumb where P : UIElement {
        /// <summary>
        /// Which object is this thumb associated to
        /// </summary>
        public D Data { get; set; }
        /// <summary>
        /// The adornedElement in parent
        /// </summary>
        public P AdornedParent { get; private set; }

        public AdornerDataThumb(P parent) {
            AdornedParent = parent;
        }
        public AdornerDataThumb(P parent, D data)
            : this(parent) {
            Data = data;
        }
    }

    internal class AdornerDataThumb<P> : Thumb where P : UIElement {
        /// <summary>
        /// The adornedElement in parent
        /// </summary>
        public P AdornedParent { get; private set; }

        public AdornerDataThumb(P parent) {
            AdornedParent = parent;
        }
    }
}
