using System.Collections;
using System.Collections.Generic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// An <see cref="IEnumerable{T}"/> for methods that return data packaged in a <see cref="PageEnvelope{T}"/>./>
    /// </summary>
    /// <typeparam name="T">The type for the items that are returned in the PageEnvelope.</typeparam>
    public class PageEnvelopeEnumerable<T> : IEnumerable<T> 
    {
        private readonly PageEnvelopeEnumerator<T> _enumerator;

        /// <summary>
        /// Create a new PageEnvelopeEnumerable which will get its values by calling the <paramref name="readMethod"/> method.
        /// </summary>
        /// <param name="readMethod">A method that returns a new page of answers for a specific offset.</param>
        public PageEnvelopeEnumerable(PageEnvelopeEnumerator<T>.ReadMethod readMethod)
        {
            _enumerator = new PageEnvelopeEnumerator<T>(readMethod);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => _enumerator;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
