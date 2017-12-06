using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// A minimal storable item to be used in testing
    /// </summary>
    public class TestItemValidated : TestItemBare, IValidatable
    {
        public void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(Value, nameof(Value), errorLocation);
        }
    }
}
