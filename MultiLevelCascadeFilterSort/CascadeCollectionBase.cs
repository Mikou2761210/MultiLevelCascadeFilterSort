using MikouTools.Collections.Optimized;
using MikouTools.Utils;
using MultiLevelCascadeFilterSort.CascadeViews;

namespace MultiLevelCascadeFilterSort
{
    /// <summary>
    /// Base collection for managing multi-level cascade views.
    /// Each item is assigned a unique integer ID and is linked with associated child views.
    /// </summary>
    /// <typeparam name="CascadeKey">Type used to identify child views (must be non-null).</typeparam>
    /// <typeparam name="ItemValue">Type of items stored in the collection (must be non-null).</typeparam>
    public abstract class CascadeCollectionBase<CascadeKey, ItemValue>
        where CascadeKey : notnull
        where ItemValue : notnull
    {
        private readonly UniqueNumberGenerator _numberGenerator = new();

        // Dictionary to manage items using unique integer IDs.
        protected internal DualKeyDictionary<int, ItemValue> BaseList;

        // Collection of child views (CascadeView) associated with each filter key.
        protected internal readonly Dictionary<CascadeKey, CascadeViewBase<CascadeKey, ItemValue>> Children;

        /// <summary>
        /// Factory method to create the base item dictionary.
        /// Override this in derived classes to provide specific initialization.
        /// </summary>
        protected internal virtual DualKeyDictionary<int, ItemValue> CreateBaseItemDictionary()
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
        /// Initializes a new instance of CascadeCollectionBase.
        /// This constructor sets up the base item dictionary and child views.
        /// </summary>
        public CascadeCollectionBase()
        {
            BaseList = CreateBaseItemDictionary();
            Children = CreateChildViews();
        }

        /// <summary>
        /// Gets all unique integer IDs of items in the base collection.
        /// </summary>
        public IEnumerable<int> GetIDs() => BaseList.Keys;

        /// <summary>
        /// Gets all items stored in the base collection.
        /// </summary>
        public IEnumerable<ItemValue> GetValues() => BaseList.Values;

        /// <summary>
        /// Gets or sets the item associated with the specified unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the item.</param>
        /// <returns>The item corresponding to the specified ID.</returns>
        public ItemValue this[int id]
        {
            get => BaseList[id];
            set => BaseList[id] = value;
        }

        /// <summary>
        /// Retrieves the unique ID assigned to the specified item.
        /// Returns -1 if the item is not found.
        /// </summary>
        /// <param name="item">The item for which the ID is requested.</param>
        /// <returns>The unique ID, or -1 if not found.</returns>
        public int GetID(ItemValue item)
        {
            if (BaseList.TryGetKey(item, out int id))
            {
                return id;
            }
            return -1;
        }

        /// <summary>
        /// Adds a new item to the base collection, assigns a unique ID, and propagates the addition to all child views.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The unique ID assigned to the item, or -1 if the addition fails.</returns>
        public int Add(ItemValue item)
        {
            int id = _numberGenerator.GenerateUniqueNumber();
            if (!BaseList.TryAdd(id, item))
            {
                _numberGenerator.ReleaseUniqueNumber(id);
                return -1;
            }
            // Propagate the addition to each child view.
            foreach (var child in Children)
            {
                child.Value.Add(id);
            }
            return id;
        }

        /// <summary>
        /// Adds a range of items to the base collection.
        /// Each item is assigned a unique ID, and the additions are reflected in all child views.
        /// </summary>
        /// <param name="items">A collection of items to add.</param>
        public void AddRange(IEnumerable<ItemValue> items)
        {
            List<int> ids = new(items.Count());
            foreach (ItemValue item in items)
            {
                int id = _numberGenerator.GenerateUniqueNumber();
                if (BaseList.TryAdd(id, item))
                {
                    ids.Add(id);
                }
                else
                {
                    _numberGenerator.ReleaseUniqueNumber(id);
                }
            }
            foreach (var child in Children.Values)
            {
                child.AddRange(ids);
            }
        }

        /// <summary>
        /// Adds a new item to the base collection and inserts it into all child views in sorted order
        /// based on the last used comparer.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>
        /// The unique ID assigned to the item if successful; otherwise, -1.
        /// </returns>
        public int InsertItemInOrder(ItemValue item)
        {
            int id = _numberGenerator.GenerateUniqueNumber();
            // Attempt to add the item to the base collection.
            if (!BaseList.TryAdd(id, item))
            {
                // Release the generated ID if addition fails and return an error code.
                _numberGenerator.ReleaseUniqueNumber(id);
                return -1;
            }
            // Propagate the addition to each child view to insert the item in sorted order.
            foreach (var child in Children)
            {
                child.Value.InsertItemInOrder(id);
            }
            return id;
        }

        /// <summary>
        /// Removes the specified item from the base collection and its associated child views.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the removal is successful; otherwise, false.</returns>
        public bool Remove(ItemValue item)
        {
            if (BaseList.TryGetKey(item, out int id))
            {
                return RemoveId(id);
            }
            return false;
        }

        /// <summary>
        /// Removes the item identified by the specified unique ID.
        /// The removed ID is released for reuse, and the removal is propagated to all child views.
        /// </summary>
        /// <param name="id">The unique ID of the item to remove.</param>
        /// <returns>True if removal is successful; otherwise, false.</returns>
        public bool RemoveId(int id)
        {
            if (BaseList.Remove(id))
            {
                _numberGenerator.ReleaseUniqueNumber(id);
                // Propagate the removal to each child view.
                foreach (var child in Children)
                {
                    child.Value.Remove(id);
                }
                return true;
            }
            return false;
        }

        #region Cascade Methods

        /// <summary>
        /// Retrieves the child view associated with the specified cascade key.
        /// </summary>
        /// <param name="cascadeKey">The key identifying the child view.</param>
        /// <returns>The child view instance if found; otherwise, null.</returns>
        public CascadeViewBase<CascadeKey, ItemValue>? GetCascadeView(CascadeKey cascadeKey)
        {
            if (Children.TryGetValue(cascadeKey, out CascadeViewBase<CascadeKey, ItemValue>? value))
                return value;
            return null;
        }

        /// <summary>
        /// Adds a new child view to the collection.
        /// </summary>
        /// <param name="cascadeKey">The key that identifies the child view.</param>
        /// <param name="addItem">The instance of the child view to add.</param>
        /// <returns>True if the view is added successfully; otherwise, false.</returns>
        public bool AddCascadeView(CascadeKey cascadeKey, CascadeViewBase<CascadeKey, ItemValue> addItem)
        {
            return Children.TryAdd(cascadeKey, addItem);
        }

        /// <summary>
        /// Removes the child view associated with the specified cascade key.
        /// </summary>
        /// <param name="cascadeKey">The key identifying the child view to remove.</param>
        /// <returns>True if removal is successful; otherwise, false.</returns>
        public bool RemoveCascadeView(CascadeKey cascadeKey)
        {
            return Children.Remove(cascadeKey);
        }

        #endregion Cascade Methods
    }
}
