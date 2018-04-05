using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// An <see cref="IEnumerator{T}"/> for methods that return data packaged in a <see cref="PageEnvelope{T}"/>./>
    /// </summary>
    /// <typeparam name="T">The type for the items that are returned in the PageEnvelope.</typeparam>
    public class PageEnvelopeEnumerator<T> : IEnumerator<T>
    {
        private readonly ReadMethodDelegate _readMethodDelegate;
        private readonly CancellationToken _token;
        private PageEnvelope<T> _currentPageEnvelope;
        private int? _currentOffset;
        private IEnumerator<T> _dataEnumerator;
        private bool _endOfData;

        /// <summary>
        /// How to get new page envelopes when required.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public delegate PageEnvelope<T> ReadMethodDelegate(int offset, CancellationToken token);

        /// <summary>
        /// Create a new PageEnvelopeEnumerator which will get its values by calling the <paramref name="readMethodDelegate"/> method.
        /// </summary>
        /// <param name="readMethodDelegate">A method that returns a new page of answers for a specific offset.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public PageEnvelopeEnumerator(ReadMethodDelegate readMethodDelegate, CancellationToken token = default(CancellationToken))
        {
            _readMethodDelegate = readMethodDelegate;
            _token = token;
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

            if (_token.IsCancellationRequested) return false;
            return ReadAndMoveNext(_currentOffset.Value);
        }

        private bool ReadAndMoveNext(int offset)
        {
            _currentPageEnvelope = _readMethodDelegate(offset, _token);
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
        public T Current => _dataEnumerator == null ? default(T) : _dataEnumerator.Current;

        object IEnumerator.Current => Current;
    }
}
