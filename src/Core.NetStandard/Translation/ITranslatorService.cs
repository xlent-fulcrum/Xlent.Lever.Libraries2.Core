using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Translation
{
    /// <summary>
    /// What a translator service need to fulfil.
    /// </summary>
    public interface ITranslatorService
    {
        /// <summary>
        /// Go through the <paramref name="conceptValues"/> and add the translated value to <paramref name="translations"/>.
        /// </summary>
        /// <param name="conceptValues"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        Task TranslateAsync(IEnumerable<string> conceptValues, IDictionary<string, string> translations);
    }
}