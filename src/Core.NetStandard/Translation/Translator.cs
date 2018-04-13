using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Translation
{
    /// <summary>
    /// A convenience class for translations.
    /// </summary>
    public class Translator
    {
        private readonly string _clientName;
        private readonly ITranslatorService _service;
        private readonly HashSet<string> _conceptValues;
        private readonly Dictionary<string, string> _translations;
        private readonly Regex _conceptValueInStringRegex;

        /// <summary>
        /// A translator for a specific <paramref name="clientName"/> that will use the <paramref name="service"/> for the actual translations.
        /// </summary>
        public Translator(string clientName, ITranslatorService service)
        {
            _clientName = clientName;
            _service = service;
            _conceptValues = new HashSet<string>();
            _translations = new Dictionary<string, string>();
            _conceptValueInStringRegex = new Regex("\"" + @"(\(([^!]+)!([^!]+)!(?:(?!\)" + "\"" + @").)+\)" + ")\"", RegexOptions.Compiled);
        }

        /// <summary>
        /// Decorate the <paramref name="value"/> into a concept value path.
        /// </summary>
        public string Decorate(string conceptName, string value)
        {
            if (value == null) return null;
            return IsDecorated(value) ? value : Decorate(conceptName, _clientName, value);
        }

        /// <summary>
        /// Decorate the <paramref name="item"/> so that concept values are set to concept value paths.
        /// </summary>
        public TModel DecorateItem<TModel>(TModel item)
        {
            if (Equals(item, default(TModel))) return item;
            DecoratePropertiesWithConceptAttribute(item);
            return item;
        }

        private void DecorateWithReflection(string conceptName, object o, PropertyInfo property)
        {
            var oldValue = (string)property.GetValue(o);
            var newValue = Decorate(conceptName, oldValue);
            property.SetValue(o, newValue);
        }

        private void DecoratePropertiesWithConceptAttribute(object o)
        {
            if (o == null) return;
            var objectType = o.GetType();
            if (!objectType.IsClass) return;
            foreach (var property in objectType.GetProperties())
            {
                var currentValue = property.GetValue(o);
                if (property.MemberType == MemberTypes.NestedType)
                {
                    // Recursive call
                    DecoratePropertiesWithConceptAttribute(currentValue);
                    continue;
                }
                if (property.MemberType != MemberTypes.Property) continue;
                if (currentValue == null) continue;
                var conceptAttribute = GetConceptAttribute(o, property);
                if (conceptAttribute == null)
                {
                    if (!(currentValue is ICollection collection)) continue;
                    foreach (var item in collection)
                    {
                        // Recursive call
                        DecoratePropertiesWithConceptAttribute(item);
                    }
                    continue;
                }

                if (currentValue is string)
                {
                    DecorateWithReflection(conceptAttribute.ConceptName, o, property);
                }
                // ReSharper disable once SuspiciousTypeConversion.Global

                if (currentValue is ICollection<string> stringCollection)
                {
                    var newValue = stringCollection.Select(v => Decorate(conceptAttribute.ConceptName, v));
                    switch (stringCollection)
                    {
                        case string[] _:
                            property.SetValue(o, newValue.ToArray());
                            break;
                        case List<string> _:
                            property.SetValue(o, newValue.ToList());
                            break;
                        default:
                            FulcrumAssert.Fail(
                                $"Failed to decorate class {objectType.FullName}: A collection can only have the {nameof(TranslationConceptAttribute)} attribute if it is of type  {typeof(string[]).Name} or {typeof(List<string>).Name}, which was not true for property {property.Name} ({property.PropertyType.Name}).");
                            break;
                    }
                }
            }
        }

        private TranslationConceptAttribute GetConceptAttribute(object o, PropertyInfo property)
        {
            return (TranslationConceptAttribute)property.GetCustomAttributes(false).FirstOrDefault(a => a is TranslationConceptAttribute);
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

        private static string Decorate(string conceptName, string clientName, string value) => value == null ? null :
            $"({conceptName}!~{clientName}!{value})";

        /// <summary>
        /// Add all concept values in the <paramref name="item"/> to the list of values to be translated.
        /// </summary>
        public Translator Add<T>(T item)
        {
            if (item == null) return this;
            var jsonString = JsonConvert.SerializeObject(item);
            foreach (Match match in _conceptValueInStringRegex.Matches(jsonString))
            {
                FulcrumAssert.IsGreaterThanOrEqualTo(1, match.Groups.Count, null, $"Expected match to have at least one group");
                var conceptPath = match.Groups[1].ToString();
                _conceptValues.Add(conceptPath);
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
            InternalContract.RequireNotNull(_service, null, $"No translation service was set up for this class ({typeof(Translator).FullName}), so this method ({nameof(ExecuteAsync)}) must not be called.");
            await _service.TranslateAsync(_conceptValues, _translations);
        }

        /// <summary>
        /// Find all concept values in the <paramref name="item"/>, translate them and return the result.
        /// </summary>
        public T Translate<T>(T item)
        {
            if (item == null) return default(T);
            var json = JsonConvert.SerializeObject(item);
            foreach (var conceptValue in _conceptValues)
            {
                json = json.Replace(conceptValue, _translations[conceptValue]);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}