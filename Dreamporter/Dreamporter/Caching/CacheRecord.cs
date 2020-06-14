using System;

namespace Dreamporter.Caching
{
    public class CacheRecord
    {
        public String CacheAddress { get; set; }
        public Int32 CacheRecordId { get; set; }
        public String CacheKey { get; set; }
        public String CacheVersion { get; set; }
        public DateTime CacheExpiresIn { get; set; }
        public Byte[] CacheData { get; set; }
    }
}
