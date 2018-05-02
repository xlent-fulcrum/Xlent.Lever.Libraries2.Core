using System;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Information about a claimed lock
    /// </summary>
    public class Lock : IValidatable
    {
        /// <summary>
        /// The proof that the locks is valid
        /// </summary>
        public string LockId { get; set; }

        /// <summary>
        /// The id of the object that the lock is for.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// The id of the object that the lock is for.
        /// </summary>
        public DateTimeOffset ValidUntil { get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(LockId, nameof(LockId), errorLocation);
            FulcrumValidate.IsNotNullOrWhiteSpace(ItemId, nameof(ItemId), errorLocation);
            FulcrumValidate.IsNotDefaultValue(ValidUntil, nameof(ValidUntil), errorLocation);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return LockId.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{ItemId} ({ValidUntil})";
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Lock @lock)) return false;
            return Equals(LockId, @lock.LockId) && Equals(ItemId, @lock.ItemId);
        }
    }
}