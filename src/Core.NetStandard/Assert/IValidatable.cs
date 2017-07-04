using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Assert
{
    /// <summary>
    /// Interface for classes that are validatable, i.e has a method for validating the properties of the class.
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// FulcrumValidate that the properties are OK. The validation should be made with methods from the <see cref="Validate"/> class.
        /// </summary>
        /// <exception cref="FulcrumAssertionFailedException">A validation failed.</exception>
        /// <param name="errorLocation">A unique errorLocation for the part of errorLocation where the validation check was made.</param>
        /// <param name="propertyPath">The path of properties up to this validation.</param>
        void Validate(string errorLocation, string propertyPath = "");
    }
}
