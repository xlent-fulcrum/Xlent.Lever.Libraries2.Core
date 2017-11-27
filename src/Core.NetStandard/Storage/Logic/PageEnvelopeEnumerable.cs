using System.Collections;
using System.Collections.Generic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// An <see cref="IEnumerable{T}"/> for methods that return data packaged in a <see cref="PageEnvelope{TStorableItem,TId}"/>./>
    /// </summary>
    /// <typeparam name="TStorableItem">The type for the items that are returned in the PageEnvelope.</typeparam>
    /// <typeparam name="TId">The type for the identifier of the items.</typeparam>
    public class PageEnvelopeEnumerable<TStorableItem, TId> : IEnumerable<TStorableItem> 
        where TStorableItem : IStorableItem<TId>
    {
        private readonly PageEnvelopeEnumerator<TStorableItem, TId> _enumerator;

        /// <summary>
        /// Create a new PageEnvelopeEnumerable which will get its values by calling the <paramref name="readMethod"/> method.
        /// </summary>
        /// <param name="readMethod">A method that returns a new page of answers for a specific offset.</param>
        public PageEnvelopeEnumerable(PageEnvelopeEnumerator<TStorableItem,TId>.ReadMethod readMethod)
        {
            _enumerator = new PageEnvelopeEnumerator<TStorableItem, TId>(readMethod);
        }

        /// <inheritdoc />
        public IEnumerator<TStorableItem> GetEnumerator() => _enumerator;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
