/*
 * Show Items in pickup box
 */

//////
/// Nothing to be TESTED here!
//////

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CIS681.Fall2012.VDS.UI.Objects;
using CIS681.Fall2012.VDS.UI.Operation;

namespace CIS681.Fall2012.VDS.UI.Components {
    internal class PickupItem : ContentControl {
        static PickupItem() {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PickupItem), new FrameworkPropertyMetadata(typeof(PickupItem)));
        }

        public PickupItem() {
            MouseLeftButtonDown += Drag_LeftMouseDownEventHandler;
        }

        private static void Drag_LeftMouseDownEventHandler(object sender, MouseButtonEventArgs e) {
            PickupItem item = sender as PickupItem;
            ModelItem m = item.Content as ModelItem;
            if (m == null) return;
            m = new ModelItem(m.Type);
            // wrap data
            DragDataWrapper data = new DragDataWrapper(m, DragOperationType.Create);
            data.DragStartPosition = e.GetPosition(item);
            DragDrop.DoDragDrop(m, data, DragDropEffects.Copy);
        }
    }
}

