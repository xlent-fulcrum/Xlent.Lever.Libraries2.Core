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
        /// Go through the <paramref name="translations"/> and set the value with the translated value.
        /// </summary>
        /// <param name="translations"></param>
        /// <returns></returns>
        Task TranslateAsync(IDictionary<string, string> translations);
    }
}