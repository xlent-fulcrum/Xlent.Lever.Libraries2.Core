using System;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Misc
{
    /// <summary>
    /// Attribute to hide methods from <see cref="FulcrumException"/> stack traces.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class StackTraceHiddenAttribute : Attribute
    {
    }
}