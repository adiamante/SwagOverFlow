using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Caching
{
    public interface IResultCacheProvider
    {
        CacheRecord Find(String cacheAddress, String cacheKey, String version = null);

        void Save(CacheRecord cacheRecord);
    }
}
