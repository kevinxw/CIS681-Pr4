/*
 * Describe a item that can be drawn on a canvas
 */

//////
/// Nothing to be TESTED here!
//////

using System.Windows.Controls;

namespace CIS681.Fall2012.VDS.UI {
    public interface IDrawable {
        Canvas ContainerCanvas { get; set; }
        void Draw();
    }
}
