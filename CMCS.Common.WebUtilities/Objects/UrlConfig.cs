using System;
using System.Collections.Generic;
using System.Text;

namespace CMCS.Common.WebUtilities.Objects
{
    public class UrlConfig
    {
        public string CanonicalHost { get; set; }

        public int? HttpPort { get; set; }

        public int? HttpsPort { get; set; }
    }
}
