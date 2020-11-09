using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMCS.Common.WebUtilities.Objects
{
    public class SitemapEntry
    {
        public enum ChangeFrequencies
        {
            Never = 1,
            Yearly = 2,
            Monthly = 3,
            Weekly = 4,
            Daily = 5,
            Hourly = 6,
            Always = 7
        }

        public SitemapEntry(string location)
        {
            this.Location = location;
        }

        public SitemapEntry(string location, DateTime? lastModified, ChangeFrequencies? changeFrequency, decimal? priority) : this(location)
        {
            this.LastModified = lastModified;
            this.ChangeFrequency = changeFrequency;
            if (priority.HasValue && (priority.Value < 0 || priority.Value > 1))
                throw new ArgumentOutOfRangeException("priority", "If priority is set, it must be a decimal value between 0 and 1 inclusive");
            this.Priority = priority;

        }

        public string Location { get; private set; }

        public DateTime? LastModified { get; private set; }

        public ChangeFrequencies? ChangeFrequency { get; private set; }

        public decimal? Priority { get; private set; }

        internal void SetLocation(string location)
        {
            this.Location = location;
        }
    }
}
