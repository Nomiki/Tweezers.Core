using System.Collections.Generic;
using Tweezers.Schema.DataHolders.DB;

namespace Tweezers.Schema.Interfaces
{
    public interface IDatabaseProxy
    {
        T Get<T>(object id);

        T Add<T>(object id, T data);

        T Edit<T>(object id, T data);

        bool Delete<T>(object id);

        IEnumerable<T> List<T>(FindOptions<T> opts);
    }
}
