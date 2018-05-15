using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Translation
{
    /// <summary>
    /// What a translator service need to fulfil.
    /// </summary>
    public interface ITranslatorService
    {
        /// <summary>
        /// Go through the <paramref name="conceptValuePaths"/> and return a dictionary with translated values..
        /// </summary>
        /// <param name="conceptValuePaths">The values that needs to be translated.</param>
        /// <param name="targetClientName">The client that we should translate to.</param>
        /// <returns>A dictionary with concept values as keys and the translated values as values.</returns>
        Task<IDictionary<string, string>> TranslateAsync(IEnumerable<string> conceptValuePaths, string targetClientName);
    }
}