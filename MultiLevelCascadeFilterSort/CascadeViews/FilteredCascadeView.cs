namespace MultiLevelCascadeFilterSort.CascadeViews
{
    public class FilteredCascadeView<CascadeKey, ItemValue> : CascadeViewBase<CascadeKey, ItemValue> where CascadeKey : notnull where ItemValue : notnull
    {
        public FilteredCascadeView(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent, Func<ItemValue, bool>? filterFunc = null) : base(@base, parent)
        {
            ChangeFilter(filterFunc);
        }
        protected override void Initialize(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent)
        {
            Base = @base;
            Parent = parent;
            AddRange(parent?.IdList ?? @base.BaseList.Keys.ToList());
        }

        public virtual Func<ItemValue, bool>? FilterFunc { get; protected set; } = null;


        protected internal bool FilterCheck(int id) => (FilterFunc == null || FilterFunc(Base[id]));


        protected internal new virtual bool Add(int id)
        {
            if (FilterCheck(id))
            {
                base.Add(id);
                return true;
            }
            return false;
        }

        protected internal new virtual bool AddRange(IEnumerable<int> ids)
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

        protected bool _currentFilterAll = false;
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