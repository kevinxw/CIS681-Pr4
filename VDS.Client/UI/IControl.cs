/*
 * The control object (responsible for displaying) of one data structure
 */

//////
/// Nothing to be TESTED here!
//////

namespace CIS681.Fall2012.VDS.UI {
    public interface IControl<T> {
        T Control { get; set; }
    }
}
