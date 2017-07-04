namespace Xlent.Lever.Libraries2.Core.Storage.Test
{
    /// <summary>
    /// Enumeration for the different kinds of data that we expect from a testable class.
    /// </summary>
    public enum TypeOfTestDataEnum
    {
        /// <summary>
        ///  A fixed set of data, not the same data as <see cref="Variant2"/>.
        /// </summary>
        Variant1,
        /// <summary>
        ///  A fixed set of data, not the same data as <see cref="Variant1"/>.
        /// </summary>
        Variant2,
        /// <summary>
        /// A random set of data, shouldn't be Equal to any other instance.
        /// </summary>
        Random
    };
}