using System;


namespace Dreamporter.Core
{
    public class InstructionCacheProperties
    {
        public Boolean Enabled { get; set; }
        public String AddressPattern { get; set; } = "";
        public String KeyPattern { get; set; } = "";
        public String VersionPattern { get; set; } = "";
        public Int32 ExpiresInMinutes { get; set; } = 0;
    }
}
