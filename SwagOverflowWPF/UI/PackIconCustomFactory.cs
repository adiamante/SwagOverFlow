using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflowWPF.UI
{
    public enum PackIconCustomKind
    {
        Custom1,
        Custom2
    }

    public static class PackIconCustomFactory
    {
        public static Lazy<IDictionary<PackIconCustomKind, string>> DataIndex { get; }

        static PackIconCustomFactory()
        {
            if (DataIndex == null)
            {
                DataIndex = new Lazy<IDictionary<PackIconCustomKind, string>>(PackIconCustomFactory.Create);
            }
        }

        public static IDictionary<PackIconCustomKind, string> Create()
        {
            return new Dictionary<PackIconCustomKind, string>
                   {
                       {PackIconCustomKind.Custom1, ""},
                       {PackIconCustomKind.Custom2, "M256 512c134 0 244 -103 255 -234h-32c-7 80 -57 147 -127 180l-29 -28l-81 81zM353 260c0 34.9356 -13.6691 57 -47 57h-20v-123h19c25.6434 0 40.2719 13.9518 46 34c2 7 2 15 2 24v8zM306 341c38.1819 0 60.9327 -19.3318 72 -47c4 -10 5 -22 5 -34v-8 c0 -26.4057 -7.57104 -45.571 -21 -59c-13.4122 -13.4122 -30.5519 -22 -57 -22h-49v170h50zM207 258c15.3033 -6.12131 28 -18.2465 28 -39c0 -8 -2 -14 -5 -20s-6 -12 -11 -16c-9.37815 -7.50252 -23.5479 -12 -40 -12c-31.0959 0 -54 15.7441 -54 47h27 c0 -15.4518 11.6022 -25 27 -25c17.8699 0 28 8.53939 28 27c0 18.8434 -12.3951 27 -31 27h-16v22h16c17.3039 0 29 8.03066 29 25c0 16.5246 -8.84017 25 -26 25c-14.4441 0 -25 -8.58131 -25 -23h-28c0 14.8936 7.55163 24.5516 15 32c9.92709 7.94167 20.8987 13 38 13 c25.6475 0 40.3859 -9.7718 49 -27c3 -6 4 -12 4 -20c0 -18.1878 -12.3353 -29.6677 -25 -36zM160 54l29 28l81 -81l-14 -1c-134 0 -244 104 -255 235h32c8 -80 57 -148 127 -181z"}
                   };
        }
    }
}
