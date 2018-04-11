using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Decoupling.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Translation
{
    /// <summary>
    /// A convenience class for translations.
    /// </summary>
    public class Translator
    {
        private readonly string _clientName;
        private readonly ITranslatorService _service;
        private readonly Dictionary<string, string> _translations;

        /// <summary>
        /// A translator for a specific <paramref name="clientName"/> that will use the <paramref name="service"/> for the actual translations.
        /// </summary>
        public Translator(string clientName,ITranslatorService service)
        {
            _clientName = clientName;
            _service = service;
            _translations = new Dictionary<string, string>();
        }

        /// <summary>
        /// Decorate the <paramref name="value"/> into a concept value path.
        /// </summary>
        public string Decorate(string conceptName, string value)
        {
            return IsDecorated(value) ? value : Decorate(conceptName, _clientName, value);
        }

        /// <summary>
        /// Decorate the <paramref name="item"/> so that concept values are set to concept value paths.
        /// </summary>
        public TModel DecorateItem<TModel>(TModel item) where TModel : IValidatable
        {
            if (item == null) return default(TModel);
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (item is ITranslatable translatable)
            {
                translatable.DecorateForTranslation(this);
            }
            return item;
        }

        /// <summary>
        ///  Decorate the <paramref name="items"/> so that concept values are set to concept value paths.
        /// </summary>
        public IEnumerable<TModel> DecorateItems<TModel>(IEnumerable<TModel> items) where TModel : IValidatable
        {
            if (items == null) return null;
            var array = items as TModel[] ?? items.ToArray();
            foreach (var item in array)
            {
                DecorateItem(item);
            }

            return array;
        }

        /// <summary>
        /// Decorate the <paramref name="page"/> so that concept values are set to concept value paths.
        /// </summary>
        public PageEnvelope<TModel> DecoratePage<TModel>(PageEnvelope<TModel> page) where TModel : IValidatable
        {
            if (page == null) return null;
            page.Data = DecorateItems(page.Data);
            return page;
        }

        private bool IsDecorated(string value)
        {
            return ConceptValue.TryParse(value, out _);
        }

        private static string Decorate(string conceptName, string clientName, string value) =>
            $"({conceptName}!~{clientName}!{value})";

        /// <summary>
        /// Add all concept values in the <paramref name="item"/> to the list of values to be translated.
        /// </summary>
        public Translator Add<T>(T item)
        {
            if (item == null) return this;
            var regex = new Regex(@"\(([^!]+)!([^!]+)!(.+)\)", RegexOptions.Compiled);
            var jsonString = JsonConvert.SerializeObject(item);
            foreach (Match match in regex.Matches(jsonString))
            {
                var conceptPath = match.Groups[0].ToString();
                _translations.Add(conceptPath, null);
            }
            // TODO: Find all decorated strings and add them to the translation batch.
            return this;
        }

        /// <summary>
        /// Do the actual translation.
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteAsync()
        {
            await _service.TranslateAsync(_translations);
        }

        /// <summary>
        /// Find all concept values in the <paramref name="item"/>, translate them and return the result.
        /// </summary>
        public T Translate<T>(T item)
        {
            if (item == null) return default(T);
            var json = JsonConvert.SerializeObject(item);
            // TODO: Translate
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}