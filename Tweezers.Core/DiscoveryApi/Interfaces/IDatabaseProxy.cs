using System;
using System.Collections.Generic;
using DiscoveryApi.DataHolders;

namespace DiscoveryApi.Interfaces
{
    public interface IDatabaseProxy
    {
        T Get<T>(object id);

        T Add<T>(object id, T data);

        T Edit<T>(object id, T data);

        void Delete<T>(object id);

        IEnumerable<T> List<T>(FindOptions<T> opts);
    }
}
