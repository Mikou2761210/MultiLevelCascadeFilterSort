using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLevelCascadeFilterSort.CascadeViews.WPF
{

    public class WPFFilteredCascadeView<CascadeKey, ItemValue>(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent = null, Func<ItemValue, bool>? filterFunc = null) : FilteredCascadeView<CascadeKey, ItemValue>(@base, parent, filterFunc) where CascadeKey : notnull where ItemValue : notnull
    {
        protected bool _suppressNotification = false;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        protected virtual internal void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suppressNotification)
                return;
            // If the UI context has not been captured, call it directly.
            if (UIContext == null || SynchronizationContext.Current == UIContext)
            {
                CollectionChanged?.Invoke(this, e);
            }
            else
            {
                // If the current thread is not a UI thread, send to the UI context.
                UIContext.Send(_ => CollectionChanged?.Invoke(this, e), null);
            }
        }
        protected override void Initialize(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent)
        {
            Base = @base;
            Parent = parent;
            AddRange(parent?.IdList ?? @base.BaseList.Keys.ToList());
        }
        /// <summary>
        /// Assign a UI thread context to update the UI after an asynchronous operation
        /// </summary>
        public SynchronizationContext? UIContext;


        /// <summary>
        /// Suppress notifications during multiple updates
        /// </summary>
        public virtual void BeginBulkUpdate() => _suppressNotification = true;

        /// <summary>
        /// Cancels notification suppression and issues a Reset notification
        /// </summary>
        public virtual void EndBulkUpdate()
        {
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }



        protected virtual internal new bool Add(int id)
        {
            if (base.Add(id))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Base[id], IdList.Count - 1));
                return true;
            }
            return false;
        }
        protected internal override bool AddRange(IEnumerable<int> ids)
        {
            if (base.AddRange(ids))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                return true;
            }
            return false;
        }

        protected virtual internal new int InsertItemInOrder(int id)
        {
            int index = base.InsertItemInOrder(id);
            if (index != -1)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Base[id], index));
                return index;
            }
            return index;
        }

        protected virtual internal new bool Remove(int id)
        {
            int index = IdList.IndexOf(id);
            if (index != -1)
            {
                base.RemoveAt(index);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Base[id], index));
                return true;
            }
            return false;

        }

        protected virtual internal new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Base[IdList[index]], index));
        }


        public virtual new int Move(int fromIndex, int toIndex)
        {
            int result = base.Move(fromIndex, toIndex);
            if (result != -1)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, Base[IdList[result]], result, fromIndex));
            }
            return result;
        }


        public override bool Sort(int index, int count, IComparer<ItemValue>? comparer)
        {
            if (base.Sort(index, count, comparer))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                return true;
            }
            return false;
        }

        public override bool RedoLastSort()
        {
            if (base.RedoLastSort())
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                return true;
            }
            return false;
        }

        public virtual new bool ChangeFilter(Func<ItemValue, bool>? filterFunc)
        {
            if (base.ChangeFilter(filterFunc))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                return true;
            }
            return false;
        }
    }
}
