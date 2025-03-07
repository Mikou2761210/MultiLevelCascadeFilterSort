using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLevelCascadeFilterSort.CascadeViews
{
    public class CascadeView<CascadeKey, ItemValue>(CascadeCollectionBase<CascadeKey, ItemValue> @base, CascadeViewBase<CascadeKey, ItemValue>? parent) : CascadeViewBase<CascadeKey, ItemValue>(@base,parent)
        where CascadeKey : notnull
        where ItemValue : notnull
    {
    }
}
