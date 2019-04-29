using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Tweezers.Api.DataHolders
{
    public class TweezersMultipleResults : TweezersMultipleResults<object>
    {
        public new static TweezersMultipleResults Create(IEnumerable<object> items)
        {
            IEnumerable<object> itemsArr = items as object[] ?? items.ToArray();
            return new TweezersMultipleResults()
            {
                Items = itemsArr,
                Count = itemsArr.Count()
            };
        }
    }

    public class TweezersMultipleResults<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int Count { get; set; }

        protected TweezersMultipleResults()
        {
        }

        public static TweezersMultipleResults<T> Create(IEnumerable<T> items)
        {
            IEnumerable<T> itemsArr = items as T[] ?? items.ToArray();
            return new TweezersMultipleResults<T>()
            {
                Items = itemsArr,
                Count = itemsArr.Count()
            };
        }
    }
}
