namespace MultiLevelCascadeFilterSort.CascadeViews.Helper
{
    public class CascadeComparer<CascadeKey, ItemValue>(CascadeCollectionBase<CascadeKey, ItemValue> @base, IComparer<ItemValue>? comparer) : IComparer<int> where CascadeKey : notnull where ItemValue : notnull
    {
        // The comparer used to compare the actual items.
        private readonly IComparer<ItemValue> _comparer = comparer ?? Comparer<ItemValue>.Default;

        /// <summary>
        /// Compares two item IDs by comparing the corresponding items from the base collection.
        /// </summary>
        /// <param name="x">The first item ID.</param>
        /// <param name="y">The second item ID.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of the items;
        /// less than zero if x is less than y, zero if x equals y, and greater than zero if x is greater than y.
        /// </returns>
        public int Compare(int x, int y)
        {
            return _comparer.Compare(@base.BaseList[x], @base.BaseList[y]);
        }
    }
}
