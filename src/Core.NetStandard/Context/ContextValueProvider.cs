using System;
using Xlent.Lever.Libraries2.Core.Application;
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
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.ContextValueProvider.</remarks>
        [Obsolete("Use ApplicationSetup.ContextValueProvider", true)]
        protected static IValueProvider Chosen;

        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.ContextValueProvider.</remarks>
        [Obsolete("Use ApplicationSetup.ContextValueProvider", true)]
        public static IValueProvider ContextForApplication
        {
            get => ApplicationSetup.ContextValueProvider;
            set => ApplicationSetup.ContextValueProvider = value;
        }

        /// <summary>
        /// Default <see cref="IValueProvider"/> for .NET Framework.
        /// </summary>
        public static IValueProvider RecommendedForNetFramework { get; } = new AsyncLocalValueProvider();

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
