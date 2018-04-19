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
    }
}
