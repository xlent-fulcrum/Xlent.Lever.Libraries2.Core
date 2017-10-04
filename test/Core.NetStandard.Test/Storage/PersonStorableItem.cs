using System;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Misc.Models;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Storage.Test;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    /// <summary>
    /// A storable item to be used in testing
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    internal partial class PersonStorableItem<TId>
    {
        public PersonStorableItem()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="givenName">The mandatory <see cref="GivenName"/> for the person.</param>
        /// <param name="surname">The mandatory <see cref="Surname"/> for the person.</param>
        public PersonStorableItem(string givenName, string surname)
        {
            GivenName = givenName;
            Surname = surname;
        }

        /// <summary>
        /// The given name (western "first name") for the person.
        /// </summary>
        public string GivenName { get; set; }

        /// <summary>
        /// The surname (western "last name") for the person.
        /// </summary>
        public string Surname { get; set; }

    }
    internal partial class PersonStorableItem<TId> : StorableItem<TId>, INameProperty
    {
        #region IPersonProperty
        /// <inheritdoc />
        public string Name => $"{GivenName} {Surname}";
        #endregion

        #region IValidate
        /// <inheritdoc />
        public override void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(GivenName, nameof(GivenName), errorLocation);
            FulcrumValidate.IsNotNullOrWhiteSpace(Surname, nameof(Surname), errorLocation);
        }
        #endregion
    }
    internal partial class PersonStorableItem<TId> : IDeepCopy<PersonStorableItem<TId>>
    {
        public PersonStorableItem<TId> DeepCopy()
        {
            var target = new PersonStorableItem<TId>
            {
                Id = Id,
                ETag = ETag,
                GivenName = GivenName,
                Surname = Surname
            };
            return target;
        }
    }

    internal partial class PersonStorableItem<TId> : IStorableItemForTesting<PersonStorableItem<TId>, TId>, IValidatable
    {
        public PersonStorableItem<TId> InitializeWithDataForTesting(TypeOfTestDataEnum typeOfTestData)
        {
            switch (typeOfTestData)
            {
                case TypeOfTestDataEnum.Variant1:
                    GivenName = "Joe";
                    Surname = "Smith";
                    break;
                case TypeOfTestDataEnum.Variant2:
                    GivenName = "Mary";
                    Surname = "Jones";
                    break;
                case TypeOfTestDataEnum.Random:
                    GivenName = Guid.NewGuid().ToString();
                    Surname = Guid.NewGuid().ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeOfTestData), typeOfTestData, null);
            }
            return this;
        }

        public PersonStorableItem<TId> ChangeDataToNotEqualForTesting()
        {
            GivenName = Guid.NewGuid().ToString();
            return this;
        }
    }

    #region override object
    internal partial class PersonStorableItem<TId>
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var person = obj as PersonStorableItem<TId>;
            if (person == null) return false;
            if (!Equals(person.Id, Id)) return false;
            if (!string.Equals(person.ETag, ETag, StringComparison.OrdinalIgnoreCase)) return false;
            if (!string.Equals(person.GivenName, GivenName, StringComparison.OrdinalIgnoreCase)) return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!string.Equals(person.Surname, Surname, StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
    #endregion
}
