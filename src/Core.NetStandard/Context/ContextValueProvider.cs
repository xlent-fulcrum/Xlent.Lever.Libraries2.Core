namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Convenience for choosing the right <see cref="IValueProvider"/>.
    /// </summary>
    public class ContextValueProvider
    {
        static ContextValueProvider()
        {
            Chosen = RecommendedForNetCore;
        }

        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.Context.</remarks>
        public static IValueProvider Chosen { get; set; }

        /// <summary>
        /// Default <see cref="IValueProvider"/> for .NET Core.
        /// </summary>
        public static IValueProvider RecommendedForNetCore { get; } = new AsyncLocalValueProvider();

        /// <summary>
        /// Default <see cref="IValueProvider"/> when running Unit Tests.
        /// </summary>
        public static IValueProvider RecommendedForUnitTests { get; } = new SingleThreadValueProvider();
    }
}
