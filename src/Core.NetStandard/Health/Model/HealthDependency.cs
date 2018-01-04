namespace Xlent.Lever.Libraries2.Core.Health.Model
{
    /// <summary>
    /// A healthdependency for a service
    /// </summary>
    public class HealthDependency
    {
        /// <summary>
        /// The name of the dependency
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dependencytype
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Url to the dependency
        /// </summary>
        public string Url { get; set; }
    }
}