using MikouTools.Collections.DirtySort;
using MultiLevelCascadeFilterSort.CascadeViews.Helper;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MultiLevelCascadeFilterSort.CascadeViews
{
    /// <summary>
    /// Base class for a cascade view that works in conjunction with the underlying collection.
    /// Supports filtering, sorting, and other view-related operations.
    /// </summary>
    /// <typeparam name="CascadeKey">Type used to identify the view (must be non-null).</typeparam>
    /// <typeparam name="ItemValue">Type of the items displayed in the view (must be non-null).</typeparam>
    public abstract class CascadeViewBase<CascadeKey, ItemValue> : IEnumerable<ItemValue>
        where CascadeKey : notnull
        where ItemValue : notnull
    {
        #region internal

        // Reference to the underlying base collection.
        protected internal CascadeCollectionBase<CascadeKey, ItemValue> Base { get; set; }
        // Reference to the parent view (if any).
        protected internal CascadeViewBase<CascadeKey, ItemValue>? Parent { get; set; }

        // List of item IDs managed internally.
        protected internal DirtySortList<int> IdList { get; set; }

        // Collection of child views.
        protected internal Dictionary<CascadeKey, CascadeViewBase<CascadeKey, ItemValue>> Children { get; set; }

        /// <summary>
        /// Factory method to create the internal list of item IDs.
        /// Override this in derived classes to provide specific initialization.
        /// </summary>
        protected internal virtual DirtySortList<int> CreateIdList()
        {
            return [];
        }

        /// <summary>
        /// Factory method to create the collection of child views.
        /// Override this in derived classes to provide specific initialization.
        /// </summary>
        protected internal virtual Dictionary<CascadeKey, CascadeViewBase<CascadeKey, ItemValue>> CreateChildViews()
        {
            return [];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the items in the view.
        /// </summary>
        public IEnumerator<ItemValue> GetEnumerator() => new CascadeEnumerator<CascadeKey, ItemValue>(Base, IdList);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets a read-only dictionary of the child views.
        /// </summary>
        public IReadOnlyDictionary<CascadeKey, CascadeViewBase<CascadeKey, ItemValue>> GetChildren => Children;
        /// <summary>
        /// Gets a read-only list of item IDs managed by the view.
        /// </summary>
        public IReadOnlyList<int> GetIDs() => IdList;

        /// <summary>
        /// Initializes a new instance of CascadeViewBase.
        /// Sets up the base collection, parent view, ID list, and child views.
        /// </summary>
        /// <param name="base">The underlying base collection.</param>
        /// <param name="parent">The parent view (if any).</param>
        public CascadeViewBase(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent)
        {
            IdList = CreateIdList();
            Children = CreateChildViews();
            Initialize(@base, parent);
            if (Base is null) throw new NullReferenceException(nameof(Base));
            if (Parent is null) throw new NullReferenceException(nameof(Parent));
        }

        protected virtual void Initialize(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent)
        {
            Base = @base;
            Parent = parent;
            IdList.AddRange(parent?.IdList ?? @base.BaseList.Keys.ToList());
        }

        /// <summary>
        /// Gets or sets the item at the specified index within the view.
        /// The item is resolved based on the internal ID list.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item at the specified index.</returns>
        public virtual ItemValue this[int index]
        {
            get
            {
                if ((uint)index >= (uint)IdList.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return Base[IdList[index]];
            }
            set
            {
                if ((uint)index >= (uint)IdList.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                Base[IdList[index]] = value;
            }
        }

        /// <summary>
        /// Adds the specified unique ID to the internal list and propagates the addition to all child views.
        /// </summary>
        /// <param name="id">The unique ID to add.</param>
        protected internal virtual void Add(int id)
        {
            IdList.Add(id);
            foreach (var child in Children.Values)
            {
                child.Add(id);
            }
        }

        /// <summary>
        /// Adds a range of unique IDs to the internal list and updates all child views.
        /// </summary>
        /// <param name="ids">The collection of unique IDs to add.</param>
        protected internal virtual void AddRange(IEnumerable<int> ids)
        {
            IdList.AddRange(ids);
            foreach (var child in Children.Values)
            {
                child.AddRange(ids);
            }
        }

        /// <summary>
        /// Inserts the specified unique ID into the internal list at the correct sorted position.
        /// </summary>
        /// <param name="id">The unique ID to insert.</param>
        /// <returns>The index at which the ID was inserted.</returns>
        protected internal virtual int InsertItemInOrder(int id)
        {
            int index = IdList.IsDirty ? IdList.Count : IdList.BinarySearch(id, IdList.LastComparer);
            if (index < 0)
                index = ~index;
            bool dirtySave = IdList.IsDirty;
            IdList.Insert(index, id);
            IdList.IsDirty = dirtySave;
            foreach (var child in Children)
            {
                child.Value.InsertItemInOrder(id);
            }
            return index;
        }

        /// <summary>
        /// Removes the specified unique ID from the internal list and all child views.
        /// </summary>
        /// <param name="id">The unique ID to remove.</param>
        /// <returns>True if removal is successful; otherwise, false.</returns>
        protected internal virtual bool Remove(int id)
        {
            if (IdList.Remove(id))
            {
                foreach (var child in Children)
                {
                    child.Value.Remove(id);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the item at the specified index from the internal list and propagates the removal to all child views.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        protected internal virtual void RemoveAt(int index)
        {
            if ((uint)index >= (uint)IdList.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            int removeId = IdList[index];
            IdList.RemoveAt(index);
            foreach (var child in Children)
            {
                child.Value.Remove(removeId);
            }
        }

        #endregion internal

        #region public

        /// <summary>
        /// Determines whether the specified unique ID exists in the internal ID list.
        /// </summary>
        /// <param name="id">The unique ID to check.</param>
        /// <returns>True if the ID exists; otherwise, false.</returns>
        public virtual bool Contains(int id)
        {
            return IdList.Contains(id);
        }

        /// <summary>
        /// Attempts to retrieve the unique ID associated with the specified item.
        /// </summary>
        /// <param name="item">The item to look up.</param>
        /// <param name="id">
        /// When this method returns, contains the unique ID associated with the item if found;
        /// otherwise, the default value of int.
        /// </param>
        /// <returns>True if the item is found and an ID is assigned; otherwise, false.</returns>
        public virtual bool GetID(ItemValue item, [MaybeNullWhen(false)] out int id)
        {
            return Base.BaseList.TryGetKey(item, out id);
        }

        /// <summary>
        /// Retrieves the unique ID at the specified index within the internal ID list.
        /// If the index is out of range, returns -1.
        /// </summary>
        /// <param name="index">The index of the ID to retrieve.</param>
        /// <returns>
        /// The unique ID at the given index if it exists; otherwise, -1 if the index is invalid.
        /// </returns>
        public virtual int GetID(int index)
        {
            if ((uint)index >= (uint)IdList.Count)
                return -1;
            return IdList[index];
        }

        /// <summary>
        /// Attempts to retrieve the item associated with the specified unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the item to retrieve.</param>
        /// <param name="value">
        /// When this method returns, contains the item associated with the unique ID if found;
        /// otherwise, the default value for the type.
        /// </param>
        /// <returns>True if the item is found; otherwise, false.</returns>
        public virtual bool GetValue(int id, [MaybeNullWhen(false)] out ItemValue? value)
        {
            return Base.BaseList.TryGetValue(id, out value);
        }

        /// <summary>
        /// Retrieves the index of the specified item within the view.
        /// Searches based on the unique ID in the underlying base collection.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The index of the item if found; otherwise, -1.</returns>
        public virtual int IndexOf(ItemValue item)
        {
            if (Base.BaseList.TryGetKey(item, out int id))
            {
                return IdList.IndexOf(id);
            }
            return -1;
        }

        /// <summary>
        /// Moves an item from one index to another within the view, updating the internal ID list.
        /// </summary>
        /// <param name="fromIndex">The source index.</param>
        /// <param name="toIndex">The destination index.</param>
        /// <returns>The final index after the move, or -1 if no move occurred.</returns>
        public virtual int Move(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= IdList.Count)
                throw new ArgumentOutOfRangeException(nameof(fromIndex), "Source index is out of range.");

            if (toIndex < 0 || toIndex > IdList.Count)
                throw new ArgumentOutOfRangeException(nameof(toIndex), "Destination index is out of range.");

            if (fromIndex == toIndex)
                return -1;

            int id = IdList[fromIndex];
            IdList.RemoveAt(fromIndex);

            // Adjust the destination index if the source index was before the destination.
            if (fromIndex < toIndex)
                toIndex--;

            IdList.Insert(toIndex, id);
            return toIndex;
        }

        #region Sorting Methods

        /// <summary>
        /// Sorts the entire view using the default comparer.
        /// </summary>
        /// <returns>True if sorting is successful; otherwise, false.</returns>
        public bool Sort() => Sort(0, IdList.Count, null);

        /// <summary>
        /// Sorts the view using the specified comparer.
        /// </summary>
        /// <param name="comparer">Comparer to use for sorting.</param>
        /// <returns>True if sorting is successful; otherwise, false.</returns>
        public bool Sort(IComparer<ItemValue>? comparer) => Sort(0, IdList.Count, comparer);

        /// <summary>
        /// Sorts a range of items in the view using the specified comparer.
        /// </summary>
        /// <param name="index">Starting index for sorting.</param>
        /// <param name="count">Number of items to sort.</param>
        /// <param name="comparer">
        /// Comparer used for sorting.
        /// If null, a default comparer will be applied.
        /// </param>
        /// <returns>True if sorting is successful; otherwise, false.</returns>
        public virtual bool Sort(int index, int count, IComparer<ItemValue>? comparer)
        {
            return IdList.Sort(index, count, comparer != null ? new CascadeComparer<CascadeKey, ItemValue>(Base, comparer) : null);
        }

        /// <summary>
        /// Sorts the view using the specified comparison delegate.
        /// </summary>
        /// <param name="comparison">Delegate defining the comparison logic.</param>
        /// <returns>True if sorting is successful; otherwise, false.</returns>
        public bool Sort(Comparison<ItemValue> comparison) => Sort(Comparer<ItemValue>.Create(comparison));

        /// <summary>
        /// Re-applies the last used sorting logic to the view.
        /// </summary>
        /// <returns>True if the re-sort is successful; otherwise, false.</returns>
        public virtual bool RedoLastSort()
        {
            return IdList.RedoLastSort();
        }

        /// <summary>
        /// Recursively re-applies the last sorting logic to this view and all child views.
        /// </summary>
        /// <returns>True if all views are re-sorted successfully; otherwise, false.</returns>
        public virtual bool RedoLastSortRecursively()
        {
            bool result = RedoLastSort();
            foreach (var child in Children)
            {
                child.Value.RedoLastSortRecursively();
            }
            return result;
        }

        #endregion Sorting Methods

        /// <summary>
        /// Adds a new child view.
        /// </summary>
        /// <param name="cascadeKey">Key that identifies the child view.</param>
        /// <param name="cascadeView">The instance of the child view to add.</param>
        /// <returns>True if the view is added successfully; otherwise, false.</returns>
        public virtual bool AddCascadeView(CascadeKey cascadeKey, CascadeViewBase<CascadeKey, ItemValue> cascadeView)
        {
            return Children.TryAdd(cascadeKey, cascadeView);
        }

        /// <summary>
        /// Removes the child view associated with the specified key.
        /// </summary>
        /// <param name="cascadeKey">Key identifying the child view to remove.</param>
        /// <returns>True if removal is successful; otherwise, false.</returns>
        public virtual bool RemoveCascadeView(CascadeKey cascadeKey)
        {
            return Children.Remove(cascadeKey);
        }

        /// <summary>
        /// Retrieves the child view corresponding to the specified key.
        /// </summary>
        /// <param name="cascadeKey">Key identifying the child view.</param>
        /// <param name="cascadeView">The child view instance if found; otherwise, null.</param>
        /// <returns>True if the view is found; otherwise, false.</returns>
        public virtual bool GetCascadeView(CascadeKey cascadeKey, [MaybeNullWhen(false)] out CascadeViewBase<CascadeKey, ItemValue>? cascadeView)
        {
            return Children.TryGetValue(cascadeKey, out cascadeView);
        }

        #endregion public
    }
}
