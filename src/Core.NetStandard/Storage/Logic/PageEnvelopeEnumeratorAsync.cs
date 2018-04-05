using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// An <see cref="IEnumerator{T}"/> for methods that return data packaged in a <see cref="PageEnvelope{T}"/>./>
    /// </summary>
    /// <typeparam name="T">The type for the items that are returned in the PageEnvelope.</typeparam>
    public class PageEnvelopeEnumeratorAsync<T> : IDisposable
    {
        private readonly ReadMethodDelegate _readMethodDelegateAsync;
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
        public delegate Task<PageEnvelope<T>> ReadMethodDelegate(int offset, CancellationToken token);

        /// <summary>
        /// Create a new PageEnvelopeEnumerator which will get its values by calling the <paramref name="readMethodDelegateAsync"/> method.
        /// </summary>
        /// <param name="readMethodDelegateAsync">A method that returns a new page of answers for a specific offset.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public PageEnvelopeEnumeratorAsync(ReadMethodDelegate readMethodDelegateAsync, CancellationToken token = default(CancellationToken))
        {
            _readMethodDelegateAsync = readMethodDelegateAsync;
            _token = token;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <summary>
        /// Move to the next item.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MoveNextAsync()
        {
            if (_endOfData) return false;
            if (_currentOffset == null)
            {
                _currentOffset = 0;
                return await ReadAndMoveNextAsync(_currentOffset.Value);
            }
            _currentOffset++;
            if (_dataEnumerator.MoveNext()) return true;
            if (_currentOffset >= (_currentPageEnvelope.PageInfo.Total ?? int.MaxValue))
            {
                _endOfData = true;
                return false;
            }

            if (_token.IsCancellationRequested) return false;
            return await ReadAndMoveNextAsync(_currentOffset.Value);
        }

        private async Task<bool> ReadAndMoveNextAsync(int offset)
        {
            _currentPageEnvelope = await _readMethodDelegateAsync(offset, _token);
            if (_currentPageEnvelope?.PageInfo == null || _currentPageEnvelope.Data == null || _currentPageEnvelope.PageInfo.Returned == 0)
            {
                _endOfData = true;
                return false;
            }
            _dataEnumerator = _currentPageEnvelope.Data.GetEnumerator();
            return _dataEnumerator.MoveNext();
        }

        /// <summary>
        ///  Reset the enumerator to before the first item.
        /// </summary>
        public void Reset()
        {
            _currentOffset = null;
            _currentPageEnvelope = null;
            _dataEnumerator = null;
            _endOfData = false;
        }

        /// <summary>
        /// The current value for this enumerator.
        /// </summary>
        public T Current => _dataEnumerator == null ? default(T) : _dataEnumerator.Current;
    }
}
