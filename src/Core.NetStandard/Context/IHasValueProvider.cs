namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Tells that the implementor has a public <see cref="IValueProvider"/> property.
    /// </summary>
    public interface IHasValueProvider
    {
        /// <summary>
        /// The value provider that is used to getting and setting data
        /// </summary>
        IValueProvider ValueProvider { get; }
    }
}