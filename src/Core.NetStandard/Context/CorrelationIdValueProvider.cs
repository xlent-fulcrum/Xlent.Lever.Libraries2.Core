namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Stores values in the execution context which is unaffected by asynchronous code that switches threads or context. 
    /// </summary>
    /// <remarks>Updating values in a thread will not affect the value in parent/sibling threads</remarks>
    public class CorrelationIdValueProvider : ICorrelationIdValueProvider
    {
        /// <inheritdoc />
        public IValueProvider ValueProvider { get; }

        private const string CorrelationIdKey = "FulcrumCorrelationId";

        /// <summary>
        /// An instance that uses <see cref="AsyncLocalValueProvider"/> as a getter and setter.
        /// </summary>
        public static ICorrelationIdValueProvider AsyncLocalInstance { get; } = new CorrelationIdValueProvider(new AsyncLocalValueProvider());

        /// <summary>
        /// An instance that uses <see cref="SingleThreadValueProvider"/> as a getter and setter.
        /// </summary>
        public static ICorrelationIdValueProvider MemoryCacheInstance { get; } = new CorrelationIdValueProvider(new SingleThreadValueProvider());

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="valueProvider">The <see cref="IValueProvider"/> to use as a getter and setter.</param>
        public CorrelationIdValueProvider(IValueProvider valueProvider)
        {
            ValueProvider = valueProvider;
        }

        /// <inheritdoc />
        public string CorrelationId
        {
            get => ValueProvider.GetValue<string>(CorrelationIdKey);
            set => ValueProvider.SetValue(CorrelationIdKey, value);
        }
    }
}
