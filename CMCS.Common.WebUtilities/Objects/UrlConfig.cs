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

        public string GetSecureCanonicalUrl()
        {
            if(HttpsPort.HasValue)
            {
                if (HttpsPort.Value == 443)
                    return $"https://{CanonicalHost}";
                return $"https://{CanonicalHost}:{HttpsPort}";
            }
            throw new ArgumentNullException("Https port is not set. Cannot provide secure Url");
        }

        public string GetCanonicalUrl()
        {
            if (HttpPort.HasValue)
            {
                if (HttpPort.Value == 80)
                    return $"https://{CanonicalHost}";
                return $"https://{CanonicalHost}:{HttpPort}";
            }
            throw new ArgumentNullException("Http port is not set. Cannot provide Url");
        }
    }
}
