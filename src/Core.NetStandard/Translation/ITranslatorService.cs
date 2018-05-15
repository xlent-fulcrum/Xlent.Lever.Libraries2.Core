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
        /// Go through the <paramref name="conceptValues"/> and return a dictionary with translated values..
        /// </summary>
        /// <param name="conceptValues"></param>
        /// <returns>A dictionary with concept values as keys and the translated values as values.</returns>
        Task<IDictionary<string, string>> TranslateAsync(IEnumerable<string> conceptValues);
    }
}