using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Guards
{
    public class ImplementsValidatable : IValidatable
    {
        public string Mandatory { get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNull(Mandatory, nameof(Mandatory), errorLocation);
        }
    }
}
