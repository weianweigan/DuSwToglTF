﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGLTF.Collections
{
    [System.Diagnostics.DebuggerDisplay("{Count}")]
    sealed class ChildrenCollection<T, TParent> : IList<T>, IReadOnlyList<T>
        where T : class, IChildOf<TParent>
        where TParent : class
    {
        #region lifecycle

        public ChildrenCollection(TParent parent)
        {
            Guard.NotNull(parent, nameof(parent));
            _Parent = parent;
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly TParent _Parent;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
        private List<T> _Collection;

        #endregion

        #region properties

        public T this[int index]
        {
            get
            {
                if (_Collection == null) throw new ArgumentOutOfRangeException(nameof(index));
                return _Collection[index];
            }

            set
            {
                // new value must be an orphan
                Guard.NotNull(value, nameof(value));
                Guard.MustBeNull(value.LogicalParent, nameof(value.LogicalParent));

                if (_Collection == null) throw new ArgumentOutOfRangeException(nameof(index));

                if (_Collection[index] == value) return; // nothing to do

                // orphan the current child
                if (_Collection[index] != null)
                {
                    _Collection[index]._SetLogicalParent(null, -1);
                    System.Diagnostics.Debug.Assert(_Collection[index].LogicalParent == null);
                    System.Diagnostics.Debug.Assert(_Collection[index].LogicalIndex == -1);
                }

                _Collection[index] = null;

                // adopt the new child
                _Collection[index] = value;
                if (_Collection[index] != null)
                {
                    _Collection[index]._SetLogicalParent(_Parent, index);
                    System.Diagnostics.Debug.Assert(_Collection[index].LogicalParent == _Parent);
                    System.Diagnostics.Debug.Assert(_Collection[index].LogicalIndex == index);
                }
            }
        }

        public int Count => _Collection == null ? 0 : _Collection.Count;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public bool IsReadOnly => false;

        #endregion

        #region API

        public void Add(T item)
        {
            // new value must be an orphan
            Guard.NotNull(item, nameof(item));
            Guard.MustBeNull(item.LogicalParent, nameof(item.LogicalParent));
            Guard.MustBeEqualTo(-1, item.LogicalIndex, nameof(item.LogicalIndex));

            if (_Collection == null) _Collection = new List<T>();

            item._SetLogicalParent(_Parent, _Collection.Count);
            System.Diagnostics.Debug.Assert(item.LogicalParent == _Parent);
            System.Diagnostics.Debug.Assert(item.LogicalIndex == _Collection.Count);

            _Collection.Add(item);
        }

        public void Clear()
        {
            if (_Collection == null) return;

            foreach (var item in _Collection)
            {
                item._SetLogicalParent(null, -1);
                System.Diagnostics.Debug.Assert(item.LogicalParent == null);
                System.Diagnostics.Debug.Assert(item.LogicalIndex == -1);
            }

            _Collection = null;
        }

        public bool Contains(T item)
        {
            return _Collection == null ? false : _Collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_Collection == null) return;
            _Collection.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return _Collection == null ? -1 : _Collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            // new value must be an orphan
            Guard.NotNull(item, nameof(item));
            Guard.MustBeNull(item.LogicalParent, nameof(item.LogicalParent));
            Guard.MustBeEqualTo(-1, item.LogicalIndex, nameof(item.LogicalIndex));

            if (_Collection == null) _Collection = new List<T>();

            _Collection.Insert(index, item);

            // fix indices of upper items
            for (int i = index; i < _Collection.Count; ++i)
            {
                _Collection[i]._SetLogicalParent(_Parent, i);
                System.Diagnostics.Debug.Assert(_Collection[i].LogicalParent == _Parent);
                System.Diagnostics.Debug.Assert(_Collection[i].LogicalIndex == i);
            }
        }

        public bool Remove(T item)
        {
            var idx = IndexOf(item);

            if (idx < 0) return false;

            RemoveAt(idx);

            return true;
        }

        public void RemoveAt(int index)
        {
            if (_Collection == null) throw new ArgumentOutOfRangeException(nameof(index));
            if (index < 0 || index >= _Collection.Count) throw new ArgumentOutOfRangeException(nameof(index));

            // orphan the current child
            if (_Collection[index] != null) { _Collection[index]._SetLogicalParent(null, -1); }

            _Collection.RemoveAt(index);

            // fix indices of upper items
            for (int i = index; i < _Collection.Count; ++i)
            {
                _Collection[i]._SetLogicalParent(_Parent, i);
                System.Diagnostics.Debug.Assert(_Collection[i].LogicalParent == _Parent);
                System.Diagnostics.Debug.Assert(_Collection[i].LogicalIndex == i);
            }

            if (_Collection.Count == 0) _Collection = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Collection == null ? Enumerable.Empty<T>().GetEnumerator() : _Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Collection == null ? Enumerable.Empty<T>().GetEnumerator() : _Collection.GetEnumerator();
        }

        #endregion
    }
}
