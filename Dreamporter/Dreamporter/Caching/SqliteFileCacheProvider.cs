using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dreamporter.Caching
{
    public class SqliteFileCacheProvider : IResultCacheProvider, IDisposable
    {
        ConcurrentDictionary<String, UtilContext> _utilContextDict = new ConcurrentDictionary<String, UtilContext>();

        public SqliteFileCacheProvider()
        {
        }

        public void Dispose()
        {
            foreach (KeyValuePair<String, UtilContext> kvp in _utilContextDict)
            {
                kvp.Value.Dispose();
            }
        }

        public CacheRecord Find(String cacheAddress, String cacheKey, String cacheVersion = null)
        {
            UtilContext context = ResolveUtilContext(cacheAddress);
            CacheRecord cacheRecord = context.CacheRecords.Where(c => c.CacheKey == cacheKey).FirstOrDefault();

            //If expired or version does not match
            if (cacheRecord != null && ((DateTime.Now >= cacheRecord.CacheExpiresIn) || cacheRecord.CacheVersion != cacheVersion))
            {
                return null;
            }

            return cacheRecord;
        }

        public void Save(CacheRecord resultCache)
        {
            UtilContext context = ResolveUtilContext(resultCache.CacheAddress);

            CacheRecord existingResultCache = context.CacheRecords.Where(c => c.CacheKey == resultCache.CacheKey).FirstOrDefault();
            if (existingResultCache != null)
            {
                context.CacheRecords.Remove(existingResultCache);
            }
            context.CacheRecords.Add(resultCache);
            context.SaveChanges();
        }

        private UtilContext ResolveUtilContext(String cacheAddress)
        {
            String fullCacheAddress = Path.Combine("DataCache", cacheAddress);
            if (!_utilContextDict.ContainsKey(fullCacheAddress))
            {
                DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
                string connectionString = $"Data Source={fullCacheAddress}";
                optionsBuilder.UseSqlite(connectionString);

                if (!File.Exists(fullCacheAddress))
                {
                    string dir = Path.GetDirectoryName(fullCacheAddress);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                UtilContext context = new UtilContext(optionsBuilder.Options);
                context.Database.EnsureCreated();

                _utilContextDict.TryAdd(cacheAddress, context);
            }

            return _utilContextDict[cacheAddress];
        }
    }
}
