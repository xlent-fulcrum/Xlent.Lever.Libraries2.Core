using System.Collections.Generic;
using Newtonsoft.Json;

namespace Xlent.Lever.Libraries2.Core.Platform.ServiceMetas
{
    /// <summary>
    /// The relase notes for a service
    /// </summary>
    public class ReleaseNotes
    {
        /// <inheritdoc />
        public ReleaseNotes(string release)
        {
            Release = release;
        }

        /// <summary>
        /// The name of the release, e.g. "1.3.2"
        /// </summary>
        /// <remarks>Use semantic versioning: Major.Minor.Patch</remarks>
        [JsonProperty(Order = 0)]
        public string Release { get; set; }

        /// <summary>
        /// The changes
        /// </summary>
        [JsonProperty(Order = 1)]
        public Notes Notes { get; set; }
    }

    /// <summary>
    /// Represents the actual changes
    /// </summary>
    public class Notes
    {
        /// <summary>
        /// New features in this release
        /// </summary>
        [JsonProperty(Order = 0, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Features { get; set; }

        /// <summary>
        /// Changes in this release
        /// </summary>
        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Changes { get; set; }

        /// <summary>
        /// Fixes in this release
        /// </summary>
        [JsonProperty(Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Fixes { get; set; }
    }
}
