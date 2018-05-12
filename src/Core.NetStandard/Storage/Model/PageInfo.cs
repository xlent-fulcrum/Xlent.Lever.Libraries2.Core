using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// A class for information around lists that needs to be "paged", i.e. divided into smaller parts.
    /// </summary>
    public class PageInfo : IValidatable
    {
        /// <summary>
        ///  The default value for <see cref="Limit"/>. 
        /// </summary>
        public const int DefaultLimit = 100;

        /// <summary>
        /// The offset that the paging starts at
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The maximum number of items returned.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// The actual number of items returned.
        /// </summary>
        public int Returned { get; set; }

        /// <summary>
        /// The total number of items that can be returned.
        /// </summary>
        /// <remarks>
        /// This number is optional, so don't rely on it to always be set.
        /// The reason for allowing it to be optional is that it can be computational hard to calculate.
        /// </remarks>
        public int? Total { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var total = Total == null ? "" : $" ({Total.Value})";
            return $"{Offset}-{Offset + Returned - 1}{total}";
        }

        /// <inheritdoc />
        public virtual void Validate(string errorLocaction, string propertyPath = "")
        {
            FulcrumValidate.IsGreaterThanOrEqualTo(0, Offset, nameof(Offset), errorLocaction);
            FulcrumValidate.IsGreaterThanOrEqualTo(0, Limit, nameof(Limit), errorLocaction);
            FulcrumValidate.IsGreaterThanOrEqualTo(0, Returned, nameof(Returned), errorLocaction);
            if (Total != null)
            {
                FulcrumValidate.IsGreaterThanOrEqualTo(0, Total.Value, nameof(Total), errorLocaction);
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is PageInfo other)) return false;
            return Equals(Offset, other.Offset) && Equals(Limit, other.Limit) &&
                   Equals(Returned, other.Returned) && Equals(Total, other.Total);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (Offset * 397) ^ Limit;
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }
    }
}