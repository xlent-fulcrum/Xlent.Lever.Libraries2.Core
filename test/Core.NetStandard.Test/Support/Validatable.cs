using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Support
{
    internal class Validatable : IValidatable
    {
        public string Name { get; set; }

        public void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(Name, nameof(Name), errorLocation);
        }
    }
}
