using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Types that implements has a specific method for output to logs.
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        /// <remarks>Typically contains more information than the normal ToString(). </remarks>
        string ToLogString();
    }
}
