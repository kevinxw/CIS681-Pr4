/*
 * The adroner makes an object draggable and resizable
 */

//#define DEBUG_ON

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CIS681.Fall2012.VDS.Data;
using CIS681.Fall2012.VDS.UI.Objects;
using CIS681.Fall2012.VDS.UI.Promot;

namespace CIS681.Fall2012.VDS.UI.Adorner {
    internal class ModelOperationAdorner : System.Windows.Documents.Adorner {
        private const double RESIZE_MINIMAL_SIZE = 30;
        private Thumb tl, tr, bl, br, handler;
        private VisualCollection collection;

        public ModelOperationAdorner(ModelItem adorned)
            : base(adorned) {
            collection = new VisualCollection(this);
            collection.Add(tl = GetResizeThumb("LT"));
            collection.Add(tr = GetResizeThumb("RT"));
            collection.Add(bl = GetResizeThumb("LB"));
            collection.Add(br = GetResizeThumb("RB"));
            collection.Add(handler = GetMoveThumb());
        }

        /// <summary>
        /// Re-arrange points
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize) {
            ModelItem elem = AdornedElement as ModelItem;
            if (elem == null)
                return finalSize;
            tl.Arrange(new Rect(new Point(-tl.ActualWidth / 2, -tl.ActualHeight / 2), new Size(tl.ActualWidth, tl.ActualHeight)));
            tr.Arrange(new Rect(new Point(elem.Size.Width - tr.ActualWidth / 2, -tr.ActualHeight / 2), new Size(tr.ActualWidth, tr.ActualHeight)));
            bl.Arrange(new Rect(new Point(-bl.ActualWidth / 2, elem.Size.Height - bl.ActualHeight / 2), new Size(bl.ActualWidth, bl.ActualHeight)));
            br.Arrange(new Rect(new Point(elem.Size.Width - br.ActualWidth / 2, elem.Size.Height - br.ActualHeight / 2), new Size(br.ActualWidth, br.ActualHeight)));
            handler.Arrange(new Rect(new Point(-handler.ActualWidth * 1.5, -handler.ActualHeight * 1.5), new Size(handler.ActualWidth, handler.ActualHeight)));

            return finalSize;
        }

        /// <summary>
        /// Get a handler thumb
        /// </summary>
        /// <returns></returns>
        private Thumb GetMoveThumb() {
            Thumb thumb = new Thumb();
            thumb.Style = FindResource("MoveThumbStyle") as Style;
            // move operation
            thumb.DragDelta += (s, e) => {
                ModelItem elem = AdornedElement as ModelItem;
                if (elem == null) return;
#if DEBUG_ON
                System.Console.WriteLine("{0},Drag Test: elem.Positon {1}, Change {2},{3}", System.DateTime.Now.Millisecond, elem.Position, e.HorizontalChange, e.VerticalChange);
#endif
                elem.Position += new Vector(e.HorizontalChange, e.VerticalChange);
            };
            // remove operation
            thumb.MouseDoubleClick += (s, e) => {
                ModelItem elem = AdornedElement as ModelItem;
                if (elem == null) return;
                // confirm
                QuestionDialog dialog = new QuestionDialog("Are you sure to remove this model from current diagram?");
                dialog.ShowDialog();
                if (!dialog.Answer) return;
                Diagram diagram = Project.CurrentProject.Children.FindByCanvas(elem.ContainerCanvas);
                diagram.Children.Remove(elem.ContentObject);
            };
            return thumb;
        }

        /// <summary>
        /// Get a resize framework
        /// </summary>
        /// <param name="thumbType"></param>
        /// <returns></returns>
        private Thumb GetResizeThumb(string thumbType) {
            Thumb thumb = new Thumb();
            thumb.Style = FindResource("ResizeThumbStyle_" + thumbType) as Style;
            // bind drag event
            thumb.DragDelta += OnDragDeltaEventHandler;
            return thumb;
        }

        private void OnDragDeltaEventHandler(object sender, DragDeltaEventArgs e) {
            ModelItem elem = AdornedElement as ModelItem;
            if (elem == null)
                return;
            Thumb thumb = sender as Thumb;
            Size size = elem.Size;
            Point position = elem.Position;

            switch (thumb.VerticalAlignment) {
                case VerticalAlignment.Bottom:
                    if (size.Height + e.VerticalChange > RESIZE_MINIMAL_SIZE)
                        size.Height += e.VerticalChange;
                    break;
                case VerticalAlignment.Top:
                    if (size.Height - e.VerticalChange > RESIZE_MINIMAL_SIZE) {
                        size.Height -= e.VerticalChange;
                        position.Y += e.VerticalChange;
                    }
                    break;
            }
            switch (thumb.HorizontalAlignment) {
                case HorizontalAlignment.Left:
                    if (size.Width - e.HorizontalChange > RESIZE_MINIMAL_SIZE) {
                        size.Width -= e.HorizontalChange;
                        position.X += e.HorizontalChange;
                    }
                    break;
                case HorizontalAlignment.Right:
                    if (size.Width + e.HorizontalChange > RESIZE_MINIMAL_SIZE)
                        size.Width += e.HorizontalChange;
                    break;
            }
            elem.Size = size;
            elem.Position = position;
            e.Handled = true;
#if DEBUG_ON
            System.Console.WriteLine("{0} Dragging - size {1} position {2}", System.DateTime.Now.Millisecond, size.ToString(), position.ToString());
#endif
        }

        protected override Visual GetVisualChild(int index) {
            return collection[index];
        }

        protected override int VisualChildrenCount {
            get {
                return collection.Count;
            }
        }

    }
}
