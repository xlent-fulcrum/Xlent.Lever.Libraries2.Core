using System.Collections.Generic;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Crud.Model
{
    /// <summary>
    /// An Id for slaves that belongs to a master
    /// </summary>
    public class SlaveToMasterId<TId> : IValidatable
    {
        /// <summary>
        /// The empty constructor.
        /// </summary>
        public SlaveToMasterId()
        {
        }

        /// <summary>
        /// Create a SlaveToMasterId with preset master id and slave id.
        /// </summary>
        public SlaveToMasterId(TId masterId, TId slaveId)
        {
            MasterId = masterId;
            SlaveId = slaveId;
        }

        /// <summary>
        /// Th id of the slave.
        /// </summary>
        public TId SlaveId { get; set; }

        /// <summary>
        /// The id of the master for the slave
        /// </summary>
        public TId MasterId { get; set; }

        /// <inheritdoc />
        public virtual void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotDefaultValue(MasterId, nameof(MasterId), errorLocation);
            FulcrumValidate.IsNotDefaultValue(SlaveId, nameof(SlaveId), errorLocation);
        }

        /// <inheritdoc />
        public override string ToString() => $"{MasterId} {SlaveId}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is SlaveToMasterId<TId> other)) return false;
            return Equals(MasterId, other.MasterId) && Equals(SlaveId, other.SlaveId);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (EqualityComparer<TId>.Default.GetHashCode(SlaveId) * 397) ^ EqualityComparer<TId>.Default.GetHashCode(MasterId);
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }
    }
}
