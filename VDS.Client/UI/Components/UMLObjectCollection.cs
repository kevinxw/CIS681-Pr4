/*
 * A merged collection of models and connections
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using CIS681.Fall2012.VDS.Data.Objects;

namespace CIS681.Fall2012.VDS.Data {
    public class UMLObjectCollection : ICollection {
        private List<Model> models;
        private List<Connection> connections;
        private Diagram diagram;

        public UMLObjectCollection(Diagram diagram) {
            if (diagram == null)
                throw new ArgumentNullException();
            this.diagram = diagram;
            models = diagram.Models;
            connections = diagram.Connections;
        }

        /// <summary>
        /// Re-draw items on canvas
        /// </summary>
        public void Sync() {
            diagram.Control.Children.Clear();
            models.ForEach(item => {
                diagram.Control.Children.Add(item.Control);
                item.Control.ContainerCanvas = diagram.Control;
                item.Owner = diagram;
            });
            connections.ForEach(item => {
                diagram.Control.Children.Add(item.Control);
                item.Control.ContainerCanvas = diagram.Control;
                item.Owner = diagram;
            });
        }

        #region Add
        /// <summary>
        /// Add one element to current canvas
        /// </summary>
        /// <param name="value"></param>
        public void Add(object value) {
            Model m; Connection c;
            if ((m = value as Model) != null) Add(m);
            else if ((c = value as Connection) != null) Add(c);
        }
        public void Add(Model model) {
            models.Add(model);
            diagram.Control.Children.Add(model.Control);
            model.Control.ContainerCanvas = diagram.Control;
            model.Owner = diagram;
            Canvas.SetZIndex(model.Control, Count);
        }
        public void Add(Connection conn) {
            connections.Add(conn);
            diagram.Control.Children.Add(conn.Control);
            conn.Control.ContainerCanvas = diagram.Control;
            conn.Owner = diagram;
            Canvas.SetZIndex(conn.Control, Count);
        }
        #endregion

        #region Remove
        /// <summary>
        /// Remove one element from canvas
        /// </summary>
        /// <param name="value"></param>
        public void Remove(object value) {
            Model m; Connection c;
            if ((m = value as Model) != null) Remove(m);
            else if ((c = value as Connection) != null) Remove(c);
        }
        public void Remove(Connection conn) {
            conn.Sink = null;
            conn.Source = null;
            conn.Owner = null;
            conn.Control.ContainerCanvas = null;
            connections.Remove(conn);
            diagram.Control.Children.Remove(conn.Control);
        }
        public void Remove(Model model) {
            // clean relationships
            foreach (Connector connector in model.Connectors)
                connector.Connections.ForEach(item => Remove(item));
            model.Owner = null;
            model.Control.ContainerCanvas = null;
            models.Remove(model);
            diagram.Control.Children.Remove(model.Control);
        }
        #endregion

        /// <summary>
        /// Delete all objects, useful when delete a diagram
        /// </summary>
        public void Clear() {
            models.ForEach(item => { item.Owner = null; item.Control.ContainerCanvas = null; });
            connections.ForEach(item => { item.Owner = null; item.Control.ContainerCanvas = null; });
            models.Clear();
            connections.Clear();
            diagram.Control.Children.Clear();
        }
        /// <summary>
        /// Contains one object??
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(object value) {
            Model m; Connection c;
            if ((m = value as Model) != null)
                return models.Contains(m);
            else if ((c = value as Connection) != null)
                return connections.Contains(c);
            else
                return false;
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        public int Count { get { return models.Count + connections.Count; } }

        public bool IsSynchronized { get { return false; } }
        public object SyncRoot { get { return null; } }

        /// <summary>
        /// Return all children
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() {
            ArrayList arr = new ArrayList();
            models.ForEach(item => arr.Add(item));
            connections.ForEach(item => arr.Add(item));
            return arr.GetEnumerator();
        }
    }
}
