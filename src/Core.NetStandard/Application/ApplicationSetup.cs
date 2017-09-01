using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging.Model;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// Help class to setup your application
    /// </summary>
    public static class ApplicationSetup
    {
        private static IThreadHandler _threadHandler;

        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        public static IThreadHandler ThreadHandler
        {
            get
            {
                // TODO: Link to Lever WIKI
                FulcrumAssert.IsNotNull(_threadHandler, null, $"The application must at startup set {nameof(ThreadHandler)} to the appropriate {nameof(IThreadHandler)}.");
                return _threadHandler;
            }
            set
            {
                InternalContract.RequireNotNull(value, nameof(value));
                _threadHandler = value;
            }
        }
        
        private static IFulcrumLogger _logger;

        /// <summary>
        /// The chosen <see cref="IFulcrumLogger"/> to use.
        /// </summary>
        public static IFulcrumLogger Logger
        {
            get
            {
                // TODO: Link to Lever WIKI
                FulcrumAssert.IsNotNull(_logger, null, $"The application must at startup set {nameof(Logger)} to the appropriate {nameof(IFulcrumLogger)}.");
                return _logger;
            }
            set
            {
                InternalContract.RequireNotNull(value, nameof(value));
                _logger = value;
            }
        }

        private static IValueProvider _context;

        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        public static IValueProvider ContextValueProvider
        {
            get
            {
                // TODO: Link to Lever WIKI
                FulcrumAssert.IsNotNull(_context, null, $"The application must at startup set {nameof(ContextValueProvider)} to the appropriate {nameof(IValueProvider)}.");
                return _context;
            }
            set
            {
                InternalContract.RequireNotNull(value, nameof(value));
                _context = value;
            }
        }
    }
}
