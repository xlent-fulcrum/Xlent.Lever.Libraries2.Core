namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// Enumeration for diffrent levels of run time environments.
    /// </summary>
    /// <remarks>Can be use for different behavior in for instance the development environment.</remarks>
    public enum RunTimeLevelEnum
    {
        /// <summary>
        /// The default value.
        /// </summary>
        None,

        /// <summary>
        /// A development environment where for instance no external logging system is called, but only local logging.
        /// </summary>
        Development,

        /// <summary>
        /// A test environment where for instance extra validations are executed. Even validations that are time consuming.
        /// </summary>
        Test,

        /// <summary>
        /// A test environment where we want to act just like in production.
        /// </summary>
        ProductionSimulation,

        /// <summary>
        /// The production environment (or a production like environment like VER or STAGE).
        /// </summary>
        Production
    }
}
