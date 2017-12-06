using System;
using Xlent.Lever.Libraries2.Core.Misc.Models;
using Xlent.Lever.Libraries2.Core.Storage.Logic;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// A minimal storable item to be used in testing
    /// </summary>
    public partial class TestItemBare
    {
        /// <summary>
        /// The property to save.
        /// </summary>
        public string Value { get; set; }

    }

    public partial class TestItemBare : IItemForTesting
    {
        public virtual void InitializeWithDataForTesting(TypeOfTestDataEnum typeOfTestData)
        {
            switch (typeOfTestData)
            {
                case TypeOfTestDataEnum.Default:
                    Value = "Default";
                    break;
                case TypeOfTestDataEnum.ValidationFail:
                    Value = "";
                    break;
                case TypeOfTestDataEnum.Variant1:
                    Value = "Variant1";
                    break;
                case TypeOfTestDataEnum.Variant2:
                    Value = "Variant2";
                    break;
                case TypeOfTestDataEnum.Random:
                    Value = Guid.NewGuid().ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeOfTestData), typeOfTestData, null);
            }
        }

        public virtual void ChangeDataToNotEqualForTesting()
        {
            Value = Guid.NewGuid().ToString();
        }
    }

    #region override object
    public partial class TestItemBare
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var person = obj as TestItemBare;
            if (person == null) return false;
            if (!string.Equals(person.Value, Value)) return false;
            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }
    }
    #endregion
}
