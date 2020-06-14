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
            if (!_utilContextDict.ContainsKey(cacheAddress))
            {
                DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
                string connectionString = $"Data Source={cacheAddress}";
                optionsBuilder.UseSqlite(connectionString);

                if (!File.Exists(cacheAddress))
                {
                    string dir = Path.GetDirectoryName(cacheAddress);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    //File.Create(cacheAddress);
                }

                UtilContext context = new UtilContext(optionsBuilder.Options);
                context.Database.EnsureCreated();

                _utilContextDict.TryAdd(cacheAddress, context);
            }

            return _utilContextDict[cacheAddress];
        }
    }
}
