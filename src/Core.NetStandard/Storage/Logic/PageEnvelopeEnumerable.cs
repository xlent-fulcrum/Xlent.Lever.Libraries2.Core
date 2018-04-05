using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        /// Create a new PageEnvelopeEnumerable which will get its values by calling the <paramref name="readMethodDelegate"/> method.
        /// </summary>
        /// <param name="readMethodDelegate">A method that returns a new page of answers for a specific offset.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public PageEnvelopeEnumerable(PageEnvelopeEnumerator<T>.ReadMethodDelegate readMethodDelegate, CancellationToken token = default(CancellationToken))
        {
            _enumerator = new PageEnvelopeEnumerator<T>(readMethodDelegate, token);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => _enumerator;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
