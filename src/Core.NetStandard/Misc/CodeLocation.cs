using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Misc
{
    public class CodeLocation
    {
        /// <summary>
        /// The name of the method for this code location.
        /// </summary>
        public string MemberName { get; }
        /// <summary>
        /// The file path for this code location.
        /// </summary>
        public string FilePath { get; }
        /// <summary>
        /// The line number (within <see cref="FilePath"/>) for this code location.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Create a new instance for the current location in the code.
        /// </summary>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public CodeLocation(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            MemberName = memberName;
            FilePath = filePath;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Create a new instance for the current location in the code.
        /// </summary>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static CodeLocation Create(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            return new CodeLocation(memberName, filePath, lineNumber);
        }

        /// <summary>
        /// Create a string representation for the current location in the code.
        /// </summary>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static string AsString(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            return new CodeLocation(memberName, filePath, lineNumber).ToString();
        }

        /// <inheritdoc />
        public override string ToString() => $"{MemberName} in {FilePath} at line {LineNumber}";
    }
}
