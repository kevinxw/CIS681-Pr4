/*
 * Selected Items Collection
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Collections.Generic;

namespace CIS681.Fall2012.VDS.UI {
    public class SelectCollection : ICollection<ISelectable> {
        private List<ISelectable> list = new List<ISelectable>();

        /// <summary>
        /// Unselect all former selected items and select this one
        /// </summary>
        /// <param name="item"></param>
        public void Set(ISelectable item) {
            Clear();
            Add(item);
        }

        /// <summary>
        /// Select / unselect a unselected / selected item
        /// </summary>
        /// <param name="item"></param>
        public void AddOrRemove(ISelectable item) {
            if (item.IsSelected)
                Remove(item);
            else
                Add(item);
        }

        /// <summary>
        /// Add one item to selection
        /// </summary>
        /// <param name="item"></param>
        public void Add(ISelectable item) {
            item.IsSelected = true;
            list.Add(item);
        }

        /// <summary>
        /// Unselect all
        /// </summary>
        public void Clear() {
            list.ForEach(item => item.IsSelected = false);
            list.Clear();
        }

        /// <summary>
        /// If one is selected
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ISelectable item) {
            return list.Contains(item);
        }

        public void CopyTo(ISelectable[] array, int arrayIndex) {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Selected item count
        /// </summary>
        public int Count { get { return list.Count; } }
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Unselect one item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ISelectable item) {
            item.IsSelected = false;
            return list.Remove(item);
        }

        /// <summary>
        /// Get all selected items
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ISelectable> GetEnumerator() {
            return list.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return list.GetEnumerator();
        }
    }
}
