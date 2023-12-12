using System;

namespace CnC.Core.Caching
{
    public interface ICacheManager : IDisposable
    {
        T Get<T>(string key);
        void Set(string key, object data);
        bool IsSet(string key);
        void Remove(string key);
        void Clear();
    }
}
