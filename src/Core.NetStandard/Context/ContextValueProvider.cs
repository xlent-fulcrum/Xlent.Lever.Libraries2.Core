using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Convenience for choosing the right <see cref="IValueProvider"/>.
    /// </summary>
    public class ContextValueProvider
    {
        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.Context.</remarks>
        protected static IValueProvider Chosen;

        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.Context.</remarks>
        public static IValueProvider ContextForApplication
        {
            get
            {
                // TODO: Link to Lever WIKI
                FulcrumAssert.IsNotNull(Chosen, null, $"The application must at startup set {nameof(ContextForApplication)} to the appropriate {nameof(IValueProvider)}.");
                return Chosen;
            }
            set
            {
                InternalContract.RequireNotNull(value, nameof(value));
                Chosen = value;
            }
        }

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
