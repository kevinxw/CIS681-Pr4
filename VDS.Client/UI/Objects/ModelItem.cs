/*
 * Make the model a visible item on canvas
 */

//#define DEBUG_ON

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using CIS681.Fall2012.VDS.Data.Client;
using CIS681.Fall2012.VDS.UI;
using CIS681.Fall2012.VDS.UI.Adorner;
using CIS681.Fall2012.VDS.UI.Objects;
using CIS681.Fall2012.VDS.UI.Operation;

namespace CIS681.Fall2012.VDS.UI.Objects {
    public class ModelItem : UserControl, ISelectable, IContentObject<Model>, IDrawable {
        // default model type
        private const string defaultModelType = "Generic";

        private Model contentObject = null;
        public Model ContentObject {
            get { return contentObject; }
            set {
                if (contentObject != null) {
                    contentObject.PropertyChanged -= OnModelPositionChanged;
                    contentObject.PropertyChanged -= OnTypeChanged;
                    contentObject.PropertyChanged -= OnSizeChanged;
                }
                if ((contentObject = value) != null) {
                    contentObject.PropertyChanged += OnModelPositionChanged;
                    contentObject.PropertyChanged += OnTypeChanged;
                    contentObject.PropertyChanged += OnSizeChanged;
                    //if (!this.Equals(contentObject.Control))
                    contentObject.Control = this;
                    // init values
                    if (string.IsNullOrEmpty(contentObject.Type))
                        contentObject.Type = defaultModelType;
                    RefreshModelStyle(contentObject);
                    SetValue(TypeProperty, contentObject.Type);
                    RefreshModelPosition(contentObject);
                    if (!contentObject.Size.IsEmpty) {
                        Width = contentObject.Size.Width;
                        Height = contentObject.Size.Height;
                    }
                }
            }
        }

        #region Properties
        /// <summary>
        /// Is current element selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Model Type
        /// </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(ModelItem), new FrameworkPropertyMetadata(defaultModelType));
        public string Type {
            get { return contentObject.Type = (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, contentObject.Type = value); }
        }

        /// <summary>
        /// Model Title
        /// </summary>
        public string Title { get { return contentObject.Title; } set { contentObject.Title = value; } }

        /// <summary>
        /// Model Position
        /// </summary>
        public Point Position { get { return contentObject.Position; } set { contentObject.Position = value; } }

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty = DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(ModelItem));
        public static ControlTemplate GetDragThumbTemplate(UIElement element) {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }
        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value) {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        /// <summary>
        /// Size of the object
        /// </summary>
        public Size Size {
            get { return new Size(ActualWidth, ActualHeight); }
            set { ContentObject.Size = value; }
        }

        /// <summary>
        /// Get parent canvas
        /// </summary>
        public Canvas ContainerCanvas { get; set; }
        #endregion

        #region Event Handlers
        /// <summary>
        /// On model's size changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSizeChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Size") return;
            Model m = sender as Model;
            m.Control.Width = m.Size.Width;
            m.Control.Height = m.Size.Height;
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} model size changed. size {1}", System.DateTime.Now.Millisecond, m.Size.ToString());
#endif
        }

        /// <summary>
        /// Update model's position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnModelPositionChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Position") return;
            RefreshModelPosition(sender as Model);
        }
        private static void RefreshModelPosition(Model m) {
            if (m == null || double.IsNaN(m.Position.X) || double.IsNaN(m.Position.Y)) return;
            m.Control.SetValue(Canvas.LeftProperty, m.Position.X);
            m.Control.SetValue(Canvas.TopProperty, m.Position.Y);
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} model position changed. position {1}", System.DateTime.Now.Millisecond, m.Position.ToString());
#endif
        }

        /// <summary>
        /// When the type of a class is changed, normally won't happen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTypeChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Type") return;
            RefreshModelStyle(sender as Model);
        }
        private static void RefreshModelStyle(Model m) {
            if (m == null) return;
            // load style
            Style s = m.Control.TryFindResource(m.Type) as Style;
            if (s != null)
                m.Control.Style = s;
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} model type refreshed. type {1}", System.DateTime.Now.Millisecond, m.Type);
#endif
        }

        /// <summary>
        /// Load adorners
        /// </summary>
        private List<System.Windows.Documents.Adorner> adorners = new List<System.Windows.Documents.Adorner>();
        public void Draw() {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(ContainerCanvas);
            if (layer == null) return;
            System.Windows.Documents.Adorner adorner = new ModelOperationAdorner(this);
            if (adorners.FindIndex(item => item is ModelOperationAdorner) > -1)
                return;
            adorners.Add(adorner);
            layer.Add(adorner);
            System.Windows.Documents.Adorner adorner2 = new ConnectorAdorner(this);
            if (adorners.FindIndex(item => item is ConnectorAdorner) > -1)
                return;
            adorners.Add(adorner2);
            layer.Add(adorner2);
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} model refreshed", System.DateTime.Now.Millisecond);
#endif
        }
        #endregion

        #region Constructors
        public ModelItem() {
            ContentObject = new Model();
            Init();
        }
        public ModelItem(Model model) {
            ContentObject = model;
            Init();
        }
        public ModelItem(string type) {
            ContentObject = new Model();
            ContentObject.Type = type;
            Init();
        }
        private void Init() {
            ContentObject.Control = this;
        }
        /// <summary>
        /// Initialize
        /// </summary>
        static ModelItem() {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ModelItem), new FrameworkPropertyMetadata(typeof(ModelItem)));
        }
        #endregion
    }
}

/*
 * An extension to model
 */
namespace CIS681.Fall2012.VDS.Data.Client {
    public partial class Model : IControl<ModelItem> {
        public ModelItem Control { get; set; }

        partial void RefreshControl() {
            // create corresponding model item
            new ModelItem(this);
        }
    }
}
