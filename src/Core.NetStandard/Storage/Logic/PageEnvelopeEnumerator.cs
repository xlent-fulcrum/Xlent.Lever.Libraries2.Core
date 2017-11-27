using System.Collections;
using System.Collections.Generic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// An <see cref="IEnumerator{T}"/> for methods that return data packaged in a <see cref="PageEnvelope{TStorableItem,TId}"/>./>
    /// </summary>
    /// <typeparam name="TStorableItem">The type for the items that are returned in the PageEnvelope.</typeparam>
    /// <typeparam name="TId">The type for the identifier of the items.</typeparam>
    public class PageEnvelopeEnumerator<TStorableItem, TId> : IEnumerator<TStorableItem> where TStorableItem : IStorableItem<TId>
    {
        private readonly ReadMethod _readMethod;
        private PageEnvelope<TStorableItem, TId> _currentPageEnvelope;
        private int? _currentOffset;
        private IEnumerator<TStorableItem> _dataEnumerator;
        private bool _endOfData;

        /// <summary>
        /// How to get new page envelopes when required.
        /// </summary>
        /// <param name="offset"></param>
        public delegate PageEnvelope<TStorableItem, TId> ReadMethod(int offset);

        /// <summary>
        /// Create a new PageEnvelopeEnumerator which will get its values by calling the <paramref name="readMethod"/> method.
        /// </summary>
        /// <param name="readMethod">A method that returns a new page of answers for a specific offset.</param>
        public PageEnvelopeEnumerator(ReadMethod readMethod)
        {
            _readMethod = readMethod;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (_endOfData) return false;
            if (_currentOffset == null)
            {
                _currentOffset = 0;
                return ReadAndMoveNext(_currentOffset.Value);
            }
            _currentOffset++;
            if (_dataEnumerator.MoveNext()) return true;
            if (_currentOffset >= (_currentPageEnvelope.PageInfo.Total ?? int.MaxValue))
            {
                _endOfData = true;
                return false;
            }
            return ReadAndMoveNext(_currentOffset.Value);
        }

        private bool ReadAndMoveNext(int offset)
        {
            _currentPageEnvelope = _readMethod(offset);
            if (_currentPageEnvelope?.PageInfo == null || _currentPageEnvelope.Data == null || _currentPageEnvelope.PageInfo.Returned == 0)
            {
                _endOfData = true;
                return false;
            }
            _dataEnumerator = _currentPageEnvelope.Data.GetEnumerator();
            return _dataEnumerator.MoveNext();
        }

        /// <inheritdoc />
        public void Reset()
        {
            _currentOffset = null;
            _currentPageEnvelope = null;
            _dataEnumerator = null;
            _endOfData = false;
        }

        /// <inheritdoc />
        public TStorableItem Current => _dataEnumerator == null ? default(TStorableItem) : _dataEnumerator.Current;

        object IEnumerator.Current => Current;
    }
}
