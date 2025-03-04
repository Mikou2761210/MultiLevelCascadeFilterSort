namespace MultiLevelCascadeFilterSort.CascadeViews
{
    public class FilteredCascadeView<CascadeKey, ItemValue>(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent = null)  : CascadeViewBase<CascadeKey, ItemValue>(@base,parent) where CascadeKey : notnull where ItemValue : notnull
    {
        public Func<ItemValue, bool>? FilterFunc { get; private set; } = null;


        protected internal bool FilterCheck(int id) => (FilterFunc == null || FilterFunc(Base[id]));


        protected internal new bool Add(int id)
        {
            if (FilterCheck(id))
            {
                base.Add(id);
                return true;
            }
            return false;
        }

        protected internal new bool AddRange(IEnumerable<int> ids)
        {
            bool result = false;
            if (FilterFunc == null)
            {
                base.AddRange(ids);
                result = true;
            }
            else
            {
                foreach (int id in ids)
                {
                    if (Add(id))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        protected internal override int InsertItemInOrder(int id)
        {
            if (FilterCheck(id))
            {
                return base.InsertItemInOrder(id);
            }
            return -1;
        }

        private bool _currentFilterAll = false;
        public virtual bool ChangeFilter(Func<ItemValue, bool>? filterFunc)
        {
            if ((filterFunc == null && !_currentFilterAll) || FilterFunc != filterFunc)
            {
                FilterFunc = filterFunc;

                // Create a set of IDs from the base collection (or parent's filtered list if available).
                HashSet<int> parentIdHashSet = Parent == null ? [.. Base.BaseList.Keys] : [.. Parent.IdList];
                // Create a set of IDs that are currently in this view.
                HashSet<int> idHashSet = [.. IdList];

                if (FilterFunc == null)
                {
                    _currentFilterAll = true;
                    // If no filter is specified, add all items from the base that are not already in the view.
                    AddRange(parentIdHashSet.Except(idHashSet));
                }
                else
                {
                    _currentFilterAll = false;
                    // Update the view by removing items that do not satisfy the filter and adding those that do.
                    foreach (int baseId in parentIdHashSet)
                    {
                        if (idHashSet.Contains(baseId))
                        {
                            Remove(baseId);
                        }
                        else
                        {
                            Add(baseId);
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}