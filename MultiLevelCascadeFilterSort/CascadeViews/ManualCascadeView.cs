using System;
using System.Collections.Generic;

namespace MultiLevelCascadeFilterSort.CascadeViews
{
    public class ManualCascadeView<CascadeKey, ItemValue>(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent) : CascadeViewBase<CascadeKey, ItemValue>(@base, parent) where CascadeKey : notnull where ItemValue : notnull
    {
        #region Disable

        /// <summary>
        /// It is disabled
        /// </summary>
        /// <param name="id"></param>
        protected internal static new void Add(int _)
        {
            return;
        }
        /// <summary>
        /// It is disabled
        /// </summary>
        /// <param name="id"></param>
        protected internal static new void AddRange(IEnumerable<int> _)
        {
            return;
        }
        /// <summary>
        /// It is disabled
        /// </summary>
        /// <param name="id"></param>
        protected internal static new int InsertItemInOrder(int _)
        {
            return -1;
        }
        #endregion Disable

        protected override void Initialize(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent)
        {
            Base = @base;
            Parent = parent;
        }

        /// <summary>
        /// Adds an item by its unique ID if it does not already exist.
        /// </summary>
        public virtual void AddItemById(int id)
        {
            if ((Parent == null || !Parent.Contains(id)) && !base.GetIDs().Contains(id))
            {
                base.Add(id);
            }
        }

        /// <summary>
        /// Adds an item using the parent's index to retrieve its unique ID.
        /// </summary>
        public virtual void AddItemFromParentIndex(int parentIndex)
        {
            if (Parent == null) throw new NullReferenceException(nameof(Parent));
            int id = Parent.GetID(parentIndex);
            if (id != -1 && !base.GetIDs().Contains(id))
            {
                base.Add(id);
            }
        }

        /// <summary>
        /// Adds multiple items by their unique IDs.
        /// </summary>
        public virtual void AddItemsByIds(IEnumerable<int> ids)
        {
            HashSet<int>? parentIds = Parent is null ? null : [.. Parent.GetIDs()];
            HashSet<int> existIds = [.. base.GetIDs()];
            List<int> addItems = new(ids.Count());
            foreach (int id in ids)
            {
                if ((parentIds is null || parentIds.Contains(id)) && !existIds.Contains(id))
                {
                    existIds.Add(id);
                    addItems.Add(id);
                }
            }
            base.AddRange(addItems);
        }

        /// <summary>
        /// Adds multiple items using their parent indices to retrieve unique IDs.
        /// </summary>
        public virtual void AddItemsFromParentIndices(IEnumerable<int> indices)
        {
            if (Parent == null) throw new NullReferenceException(nameof(Parent));
            HashSet<int> existIds = [.. base.GetIDs()];
            List<int> addItems = new(indices.Count());
            foreach (int index in indices)
            {
                int id = Parent.GetID(index);
                if (id != -1 && existIds.Add(id))
                {
                    addItems.Add(id);
                }
            }
            base.AddRange(addItems);
        }

        /// <summary>
        /// Inserts an item by its unique ID in order, if it does not already exist.
        /// </summary>
        public virtual int InsertItemByIdInOrder(int id)
        {
            if ((Parent == null || !Parent.Contains(id)) && !base.GetIDs().Contains(id))
            {
                return base.InsertItemInOrder(id);
            }
            return -1;
        }

        /// <summary>
        /// Inserts an item using the parent's index to retrieve its unique ID in order.
        /// </summary>
        public virtual int InsertItemFromParentIndexInOrder(int parentIndex)
        {
            if (Parent == null) throw new NullReferenceException(nameof(Parent));
            int id = Parent.GetID(parentIndex);
            if (id != -1 && !base.GetIDs().Contains(id))
            {
                return base.InsertItemInOrder(id);
            }
            return -1;
        }

        /// <summary>
        /// Inserts multiple items by their unique IDs in order.
        /// </summary>
        public virtual void InsertItemsByIdsInOrder(IEnumerable<int> ids)
        {
            HashSet<int>? parentIds = Parent is null ? null : [.. Parent.GetIDs()];
            HashSet<int> existIds = [.. base.GetIDs()];
            foreach (int id in ids)
            {
                if ((parentIds is null || parentIds.Contains(id)) && !existIds.Contains(id))
                {
                    existIds.Add(id);
                    base.InsertItemInOrder(id);
                }
            }
        }

        /// <summary>
        /// Inserts multiple items using their parent indices to retrieve unique IDs in order.
        /// </summary>
        public virtual void InsertItemsFromParentIndicesInOrder(IEnumerable<int> indices)
        {
            if (Parent == null) throw new NullReferenceException(nameof(Parent));
            HashSet<int> existIds = [.. base.GetIDs()];
            foreach (int index in indices)
            {
                int id = Parent.GetID(index);
                if (id != -1 && existIds.Add(id))
                {
                    base.InsertItemInOrder(id);
                }
            }
        }

    }
}
