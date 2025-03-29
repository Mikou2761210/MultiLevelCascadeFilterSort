using MikouTools.Collections.ListEx.DirtySort;

namespace MultiLevelCascadeFilterSort.CascadeViews.Helper
{
    public struct CascadeEnumerator<CascadeKey, ItemValue> : IEnumerator<ItemValue>, System.Collections.IEnumerator where CascadeKey : notnull where ItemValue : notnull
    {
        // Reference to the base collection to access items by ID.
        private readonly CascadeCollectionBase<CascadeKey, ItemValue> _base;
        // The list of item IDs included in the filtered view.
        private readonly DirtySortList<int> _idList;
        // Current index in the enumeration.
        private int _index;
        // Current item being enumerated.
        private ItemValue? _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEnumerator"/> struct.
        /// </summary>
        /// <param name="base">The base collection instance.</param>
        /// <param name="idList">The list of item IDs in the filtered view.</param>
        internal CascadeEnumerator(CascadeCollectionBase<CascadeKey, ItemValue> @base, DirtySortList<int> @idList)
        {
            _base = @base;
            _idList = @idList;
            _index = 0;
            _current = default;
        }

        /// <summary>
        /// Releases any resources used by the enumerator.
        /// </summary>
        public readonly void Dispose() { }

        /// <summary>
        /// Advances the enumerator to the next element.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced; otherwise, false.</returns>
        public bool MoveNext()
        {
            if ((uint)_index < (uint)_idList.Count)
            {
                _current = _base[_idList[_index]];
                _index++;
                return true;
            }
            return MoveNextRare();
        }

        /// <summary>
        /// Handles the termination of the enumeration.
        /// </summary>
        /// <returns>False, indicating the end of the collection.</returns>
        private bool MoveNextRare()
        {
            _index = _idList.Count + 1;
            _current = default;
            return false;
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public readonly ItemValue Current => _current!;

        /// <summary>
        /// Gets the current element in the collection (non-generic).
        /// </summary>
        readonly object? System.Collections.IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _idList.Count + 1)
                    throw new InvalidOperationException();
                return Current;
            }
        }

        /// <summary>
        /// Resets the enumerator to its initial state.
        /// </summary>
        void System.Collections.IEnumerator.Reset()
        {
            _index = 0;
            _current = default;
        }
    }
}
