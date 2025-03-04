using System;
using System.Collections.Generic;

namespace MultiLevelCascadeFilterSort.CascadeViews
{
    public class ManualCascadeView<CascadeKey, ItemValue>(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent = null) : CascadeViewBase<CascadeKey, ItemValue>(@base, parent) where CascadeKey : notnull where ItemValue : notnull
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


        public void ManualAdd(int id)
        {
            if((Parent == null || !Parent.Contains(id)) && !base.GetIdList.Contains(id))
            {
                base.Add(id);
            }
        }
        public void ManualAddSlim(int parentindex)
        {
            if (Parent == null) throw new NullReferenceException(nameof(Parent));
            int id = Parent.GetID(parentindex);
            if (id != -1 && !base.GetIdList.Contains(id))
            {
                base.Add(id);
            }
        }

        public void ManualAddRange(IEnumerable<int> ids)
        {
            HashSet<int>? parentIds = Parent is null ? null : [.. Parent.GetIdList];
            HashSet<int> existIds = [.. base.GetIdList];
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

        public void ManualAddRangeSlim(IEnumerable<int> indexs)
        {
            if (Parent == null) throw new NullReferenceException(nameof(Parent));

            HashSet<int> existIds = [.. base.GetIdList];
            List<int> addItems = new(indexs.Count());
            foreach (int index in indexs)
            {
                int id = Parent.GetID(index);
                if (id != -1 && existIds.Add(id))
                {
                    addItems.Add(id);
                }
            }
            base.AddRange(addItems);
        }

        public int ManualInsertItemInOrder(int id)
        {
            if ((parent == null || !parent.Contains(id)) && !base.GetIdList.Contains(id))
            {
                return base.InsertItemInOrder(id);
            }
            return -1;
        }
        public int ManualInsertItemInOrderSlim(int parentindex)
        {
            if (Parent == null) throw new NullReferenceException(nameof(Parent));

            int id = Parent.GetID(parentindex);
            if (id != -1 && !base.GetIdList.Contains(id))
            {
                return base.InsertItemInOrder(id);
            }
            return -1;
        }

        public void ManualInsertItemInOrderRange(IEnumerable<int> ids)
        {
            HashSet<int>? parentIds = Parent is null ? null : [.. Parent.GetIdList];
            HashSet<int> existIds = [.. base.GetIdList];
            foreach (int id in ids)
            {
                if ((parentIds is null || parentIds.Contains(id)) && !existIds.Contains(id))
                {
                    existIds.Add(id);
                    base.InsertItemInOrder(id);
                }
            }
        }
        public void ManualInsertItemInOrderRangeSlim(IEnumerable<int> indexs)
        {
            if (Parent == null) throw new NullReferenceException(nameof(Parent));

            HashSet<int> existIds = [.. base.GetIdList];

            foreach (int index in indexs)
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
