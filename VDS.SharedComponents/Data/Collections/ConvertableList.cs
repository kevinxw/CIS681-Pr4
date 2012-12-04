/*
 * This list actually do not store anything, but instead building a convertable relationship between two types
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIS681.Fall2012.VDS.Data.Collections {
    public class ConvertableList<T,B>:IList<T> where T:B {
        /// <summary>
        /// Current type
        /// </summary>
        private IList<T> type;
        /// <summary>
        /// The base type where current type is inhrited from
        /// </summary>
        private IList<B> baseType;

        public ConvertableList(IList<T> type, IList<B> baseType) {
            this.type = type;
            this.baseType = baseType;
        }

        public int IndexOf(T item) {
            return type.IndexOf(item);
        }

        public void Insert(int index, T item) {
            type.Insert(index, item);
            baseType.Insert(index, item);
        }

        public void RemoveAt(int index) {
            type.RemoveAt(index);
            baseType.RemoveAt(index);
        }

        public T this[int index] {
            get { return type[index]; }
            set { baseType[index]= type[index] = value; }
        }

        public void Add(T item) {
            type.Add(item);
            baseType.Add(item);
        }

        public void Clear() {
            type.Clear();
            baseType.Clear();
        }

        public bool Contains(T item) {
            return type.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public int Count {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item) {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
