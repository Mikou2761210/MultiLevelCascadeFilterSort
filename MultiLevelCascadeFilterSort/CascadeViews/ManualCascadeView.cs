namespace MultiLevelCascadeFilterSort.CascadeViews
{
    public class ManualCascadeView<CascadeKey, ItemValue>(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent = null) : CascadeViewBase<CascadeKey, ItemValue>(@base, parent) where CascadeKey : notnull where ItemValue : notnull
    {
        #region Disable

        /// <summary>
        /// It is disabled
        /// </summary>
        /// <param name="id"></param>
        internal static new void Add(int _)
        {
            return;
        }
        /// <summary>
        /// It is disabled
        /// </summary>
        /// <param name="id"></param>
        internal static new void AddRange(IEnumerable<int> _)
        {
            return;
        }
        /// <summary>
        /// It is disabled
        /// </summary>
        /// <param name="id"></param>
        internal static new int InsertItemInOrder(int _)
        {
            return -1;
        }
        #endregion Disable


        internal void ManualAdd(int id)
        {
            base.Add(id);
        }

        internal void ManualAddRange(IEnumerable<int> ids)
        {
            base.AddRange(ids);
        }

        internal int ManualInsertItemInOrder(int id)
        {
            return base.InsertItemInOrder(id);
        }

    }
}
