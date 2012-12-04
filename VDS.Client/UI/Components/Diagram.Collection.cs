/*
 * Special collection type for diagram
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CIS681.Fall2012.VDS.Data.Client {
    public class DiagramCollection : IList<Diagram> {
        private List<Diagram> list;
        private TabControl tabs;
        public DiagramCollection(List<Diagram> d, TabControl t) {
            if (d == null || t == null)
                throw new ArgumentNullException();
            list = d;
            tabs = t;
        }
        /// <summary>
        /// By default, activate the new created diagram
        /// </summary>
        public bool ActivateNewDiagram { get; set; }

        /// <summary>
        /// Find a diagram element
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public Diagram Find(Predicate<Diagram> match) {
            return list.Find(match);
        }
        public Diagram FindByCanvas(Canvas c) {
            return list.Find(item => item.Control.Equals(c));
        }

        /// <summary>
        /// Return index
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Diagram item) {
            return list.IndexOf(item);
        }

        /// <summary>
        /// Insert one diagram to the project
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, Diagram item) {
            if (ActivateNewDiagram)
                item.IsOpen = true;
            list.Insert(index, item);
            tabs.Items.Insert(index, item.Tab);
        }

        /// <summary>
        /// Remove one diagram
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) {
            if (index < Count) {
                this[index].IsOpen = false;
                tabs.Items.RemoveAt(index);
                list.RemoveAt(index);
            }
        }

        public Diagram this[int index] {
            get { return list[index]; }
            set {
                TabItem tab = tabs.Items[index] as TabItem;
                ScrollViewer scroll = tab.Content as ScrollViewer;
                scroll.Content = list[index] = value;
            }
        }

        /// <summary>
        /// Attach one new diagram to current project
        /// </summary>
        /// <param name="item"></param>
        public void Add(Diagram item) {
            list.Add(item);
            tabs.Items.Add(item.Tab);
            // if there is only one tab, activate it
            if (ActivateNewDiagram)
                item.IsOpen = true;
        }

        public void Clear() {
            list.Clear();
            tabs.Items.Clear();
        }

        /// <summary>
        /// Contains one diagram?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Diagram item) {
            return list.Contains(item);
        }

        public void CopyTo(Diagram[] array, int arrayIndex) {
            list.CopyTo(array, arrayIndex);
        }

        public int Count { get { return list.Count; } }
        public bool IsReadOnly { get { return false; } }

        public bool Remove(Diagram item) {
            int i = IndexOf(item);
            if (i == -1)
                return false;
            RemoveAt(i);
            return true;
        }

        /// <summary>
        /// synchronize data and tabs
        /// </summary>
        public void Sync() {
            tabs.Items.Clear();
            list.FindAll(item => item.IsOpen).ForEach(item => tabs.Items.Add(item.Tab));
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Diagram> GetEnumerator() {
            return list.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return list.GetEnumerator();
        }
    }
}
