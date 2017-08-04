using System.Text;
using Newtonsoft.Json;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// The headers of a request
    /// </summary>
    public class StorableAsByteArray<TData, TId> : StorableByteArray<TId>, IStorableAsByteArray<TData, TId>
    {
        /// <summary>
        /// A dictionary of HTTP headers
        /// </summary>
        public TData Data
        {
            get
            {
                var jsonString = Encoding.UTF8.GetString(ByteArray);
                return JsonConvert.DeserializeObject<TData>(jsonString);
            }
            set
            {
                var jsonString = JsonConvert.SerializeObject(value);
                ByteArray = Encoding.UTF8.GetBytes(jsonString);
            }
        }
    }
}
