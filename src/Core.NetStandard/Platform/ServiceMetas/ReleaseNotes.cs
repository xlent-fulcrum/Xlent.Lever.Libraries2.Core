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
        public List<Note> Notes { get; set; }
    }

    /// <summary>
    /// Represents a single note
    /// </summary>
    public class Note
    {
        /// <summary>
        /// 
        /// </summary>
        public enum TypeEnum
        {
            /// <summary>
            /// Represents a new feature
            /// </summary>
            Feature,

            /// <summary>
            /// Represents a change in a current feature
            /// </summary>
            Change,

            /// <summary>
            /// Represents a fix, suchs a bug fix or cosmetic change
            /// </summary>
            Fix
        }

        /// <inheritdoc />
        public Note(TypeEnum type, bool breakingChange = false)
        {
            Type = type;
            BreakingChange = breakingChange;
        }

        /// <summary>
        /// The type of change
        /// </summary>
        [JsonProperty(Order = 0)]
        public TypeEnum Type { get; set; }

        /// <summary>
        /// The description of a change
        /// </summary>
        [JsonProperty(Order = 1)]
        public string Description { get; set; }

        /// <summary>
        /// Optional reference for the change, preferrably an url
        /// </summary>
        [JsonProperty(Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public string IssueReference { get; set; }

        /// <summary>
        /// Tells if this change is a breaking change, i.e. not backwards compatible
        /// </summary>
        public bool BreakingChange { get; set; }
    }
}
