using System.Collections.Generic;
using System.Threading;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// An <see cref="IEnumerable{T}"/> for methods that return data packaged in a <see cref="PageEnvelope{T}"/>./>
    /// </summary>
    /// <typeparam name="T">The type for the items that are returned in the PageEnvelope.</typeparam>
    public class PageEnvelopeEnumerableAsync<T>
    {
        private readonly PageEnvelopeEnumeratorAsync<T> _enumerator;

        /// <summary>
        /// Create a new PageEnvelopeEnumerable which will get its values by calling the <paramref name="readMethodDelegate"/> method.
        /// </summary>
        /// <param name="readMethodDelegate">A method that returns a new page of answers for a specific offset.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public PageEnvelopeEnumerableAsync(PageEnvelopeEnumeratorAsync<T>.ReadMethodDelegate readMethodDelegate, CancellationToken token = default(CancellationToken))
        {
            _enumerator = new PageEnvelopeEnumeratorAsync<T>(readMethodDelegate, token);
        }

        /// <summary>
        /// Get the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        public PageEnvelopeEnumeratorAsync<T> GetEnumerator() => _enumerator;
    }
}
