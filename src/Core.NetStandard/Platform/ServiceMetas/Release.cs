using System.Collections.Generic;
using Newtonsoft.Json;

namespace Xlent.Lever.Libraries2.Core.Platform.ServiceMetas
{
    /// <summary>
    /// The relase notes for a service
    /// </summary>
    public class Release
    {
        /// <inheritdoc />
        public Release(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the release, e.g. "1.3.2"
        /// </summary>
        /// <remarks>Use semantic versioning: Major.Minor.Patch</remarks>
        [JsonProperty(Order = 0)]
        public string Name { get; set; }

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
            /// Represents a change in a current feature that is backwards compatible
            /// </summary>
            Change,

            /// <summary>
            /// Represents a change in a current feature that is not backwards compatible
            /// </summary>
            BreakingChange,

            /// <summary>
            /// Represents a fix, suchs a bug fix or cosmetic change
            /// </summary>
            Fix
        }

        /// <inheritdoc />
        public Note(TypeEnum type)
        {
            Type = type.ToString();
        }

        /// <summary>
        /// Convenience for creating a Change that is non-breaking, i.e. backwards compatible
        /// </summary>
        public static Note Change(string description, string issueReference = null)
        {
            return new Note(TypeEnum.Change)
            {
                Description = description,
                IssueReference = issueReference
            };
        }

        /// <summary>
        /// Convenience for creating a Change that is not backwards compatible
        /// </summary>
        public static Note BreakingChange(string description, string issueReference = null)
        {
            return new Note(TypeEnum.BreakingChange)
            {
                Description = description,
                IssueReference = issueReference
            };
        }

        /// <summary>
        /// Convenience for creating a Feature
        /// </summary>
        public static Note Feature(string description, string issueReference = null)
        {
            return new Note(TypeEnum.Feature)
            {
                Description = description,
                IssueReference = issueReference
            };
        }


        /// <summary>
        /// Convenience for creating a Fix
        /// </summary>
        public static Note Fix(string description, string issueReference = null)
        {
            return new Note(TypeEnum.Fix)
            {
                Description = description,
                IssueReference = issueReference
            };
        }

        /// <summary>
        /// The type of change
        /// </summary>
        [JsonProperty(Order = 0)]
        public string Type { get; set; }

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
    }
}
