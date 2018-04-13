using System;
using System.Text.RegularExpressions;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Translation
{
    /// <summary>
    /// An important tool for loose coupling. Annotates system specific values with information that makes the value system independent.
    /// </summary>
    public class ConceptValue : IConceptValue
    {
        static readonly Regex PathRegex = new Regex(@"^\(([^!]+)!([^!]+)!(.+)\)$");
        /// <inheritdoc />
        public string ConceptName { get; set; }
        /// <inheritdoc />
        public string ClientName { get; set; }
        /// <inheritdoc />
        public string ContextName { get; set; }
        /// <inheritdoc />
        public string Value { get; set; }
        /// <inheritdoc />
        public string ToPath()
        {
            var clientOrContext = ClientName == null ? ContextName : $"~{ClientName}";
            return $"({ConceptName}!{clientOrContext}!{Value})";
        }

        /// <summary>
        /// Parse a string into a <see cref="IConceptValue"/> object.
        /// </summary>
        /// <param name="path">The string to parse</param>
        public static IConceptValue Parse(string path)
        {
            if (!TryParse(path, out var result)) InternalContract.Fail($"The path ({path}) could not be parsed as a concept value path.");
            return result;
        }

        /// <summary>
        /// Parse a string into a <see cref="IConceptValue"/> object.
        /// </summary>
        /// <param name="path">The string to parse</param>
        /// <param name="conceptValue">The concept value that was parsed from <paramref name="path"/>.</param>
        public static bool TryParse(string path, out IConceptValue conceptValue)
        {
            conceptValue = null;
            if (string.IsNullOrWhiteSpace(path)) return false;
            try
            {
                var result = PathRegex.Matches(path);
                if (result.Count != 1) return false;
                var groups = result[0].Groups;
                if (groups.Count != 4) return false;
                conceptValue = new ConceptValue
                {
                    ConceptName = groups[1].Value,
                    ContextName = groups[2].Value.StartsWith("~") ? null : groups[2].Value,
                    ClientName = groups[2].Value.StartsWith("~") ? groups[2].Value.Substring(1) : null,
                    Value = groups[3].Value
                };
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(ConceptName, nameof(ConceptName), errorLocation);
            FulcrumValidate.IsTrue(ClientName == null || ContextName == null, errorLocation, $"One of the properties {nameof(ContextName)} ({ContextName}) and {nameof(ClientName)} ({ClientName}) must be null.");
            FulcrumValidate.IsTrue(!string.IsNullOrWhiteSpace(ContextName) || !string.IsNullOrWhiteSpace(ClientName), errorLocation, $"One of the properties {nameof(ContextName)} ({ContextName}) and {nameof(ClientName)} ({ClientName}) must contain a name.");
        }
    }
}