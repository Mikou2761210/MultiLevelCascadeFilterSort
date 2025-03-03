using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLevelCascadeFilterSort
{
    public class CascadeCollection<CascadeKey, ItemValue> : CascadeCollectionBase<CascadeKey, ItemValue>
        where CascadeKey : notnull
        where ItemValue : notnull
    {

    }
}
