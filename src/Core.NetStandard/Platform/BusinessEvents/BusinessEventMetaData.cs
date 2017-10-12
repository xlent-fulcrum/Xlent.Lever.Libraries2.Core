using System;

namespace Xlent.Lever.Libraries2.Core.Platform.BusinessEvents
{
    /// <summary>
    /// Meta data present at publication of all Business Events
    /// </summary>
    public class BusinessEventMetaData
    {
        /// <summary>The technical name of the publishing client</summary>
        public string PublisherName { get; set; }

        /// <summary>The name of the entity</summary>
        public string EntityName { get; set; }

        /// <summary>The name of the event, in the context of the <see cref="EntityName"/></summary>
        public string EventName { get; set; }

        /// <summary>The major version of the event</summary>
        public int MajorVersion { get; set; }

        /// <summary>The minor version of the event</summary>
        public int MinorVersion { get; set; }

        /// <summary>The timestamp for when the event was published</summary>
        public DateTimeOffset PublishedAt { get; set; }
    }
}
