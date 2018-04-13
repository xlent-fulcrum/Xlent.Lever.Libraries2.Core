using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Translation
{
    /// <summary>
    /// An important tool for loose coupling. Annotates system specific values with information that makes the value system independent.
    /// </summary>
    public interface IConceptValue : IValidatable
    {
        /// <summary>
        /// The name of the concept that this value belongs to, e.g. "gender".
        /// </summary>
        string ConceptName { get; set; }

        /// <summary>
        /// The name of the client that used this value. If <see cref="ContextName"/> is null, then this property is mandatory.
        /// </summary>
        string ClientName { get; set; }

        /// <summary>
        /// The name of the client that used this value. If <see cref="ClientName"/> is null, then this property is mandatory.
        /// </summary>
        string ContextName { get; set; }

        /// <summary>
        /// The actual value
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Conversion function from a <see cref="IConceptValue"/> to an instance path on the form ({concept}!{context}|{value}) or ({concept}!~{client}|{value}), depending on the values of <see cref="ContextName"/> and <see cref="ClientName"/>.
        /// </summary>
        string ToPath();
    }
}
