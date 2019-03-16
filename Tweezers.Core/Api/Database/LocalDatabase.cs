using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Exceptions;
using Tweezers.Api.Interfaces;

namespace Tweezers.Api.Database
{
    public class LocalDatabase : IDatabaseProxy
    {
        private static LocalDatabase instance;
        private Dictionary<Type, Dictionary<object, object>> localDb = new Dictionary<Type, Dictionary<object, object>>();

        public static LocalDatabase Instance => instance = instance ?? new LocalDatabase();

        private LocalDatabase()
        {
        }

        public T Get<T>(object id)
        {
            if (localDb.ContainsKey(typeof(T)) && (localDb[typeof(T)]?.ContainsKey(id) ?? false))
            {
                return (T) localDb[typeof(T)][id];
            }

            throw new ItemNotFoundException(id.ToString());
        }

        public T Add<T>(object id, T data)
        {
            if (!localDb.ContainsKey(typeof(T)))
            {
                localDb[typeof(T)] = new Dictionary<object, object>();
            }

            localDb[typeof(T)][id] = data;
            return data;
        }

        public T Edit<T>(object id, T data)
        {
            if (localDb.ContainsKey(typeof(T)) && (localDb[typeof(T)]?.ContainsKey(id) ?? false))
            {
                T dbObj = (T) localDb[typeof(T)][id];
                foreach (PropertyInfo pi in typeof(T).GetProperties())
                {
                    object value = pi.GetValue(data);
                    if (value != null)
                        pi.SetValue(dbObj, value);
                }
                
                return dbObj;
            }

            throw new ItemNotFoundException(id.ToString());
        }

        public void Delete<T>(object id)
        {
            if (localDb.ContainsKey(typeof(T)) && (localDb[typeof(T)]?.ContainsKey(id) ?? false))
            {
                localDb[typeof(T)].Remove(id);
                return;
            }

            throw new ItemNotFoundException(id.ToString());
        }

        public IEnumerable<T> List<T>(FindOptions<T> opts)
        {
            return localDb.ContainsKey(typeof(T)) 
                ? localDb[typeof(T)].Values.Cast<T>()
                    .Where(opts.Predicate)
                    .Skip(opts.Skip)
                    .Take(opts.Take) 
                : Enumerable.Empty<T>();
        }
    }
}