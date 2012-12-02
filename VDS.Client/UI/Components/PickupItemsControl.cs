/*
 * Where the items can be pick up
 */

//////
/// Nothing to be TESTED here!
//////

using System.Windows;
using System.Windows.Controls;

namespace CIS681.Fall2012.VDS.UI.Components {
    internal class PickupItemsControl : ItemsControl {
        /// <summary>
        /// The size of wrapper panel
        /// </summary>
        public Size ItemSize { get; set; }

        public PickupItemsControl() {
            ItemSize = new Size(60, 50);
        }

        // Creates or identifies the element that is used to display the given item.        
        protected override DependencyObject GetContainerForItemOverride() {
            return new PickupItem();
        }

        // Determines if the specified item is (or is eligible to be) its own container.        
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return (item is PickupItem);
        }
    }
}
