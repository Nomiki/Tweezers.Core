using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tweezers.DBConnector
{
    public class TweezersMultipleResults<T>
    {
        public IEnumerable<T> Items { get; set; }

        public long Count { get; set; }

        protected TweezersMultipleResults()
        {
        }

        public static TweezersMultipleResults<T> Create(IEnumerable<T> items, long? count = null)
        {
            IEnumerable<T> itemsArr = items as T[] ?? items.ToArray();
            return new TweezersMultipleResults<T>()
            {
                Items = itemsArr,
                Count = count ?? itemsArr.Count()
            };
        }
    }
}
