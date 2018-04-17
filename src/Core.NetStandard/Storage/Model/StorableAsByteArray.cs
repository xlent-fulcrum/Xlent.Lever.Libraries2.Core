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
        /// This property is stored serialized as a byte array
        /// </summary>
        public TData Data
        {
            get
            {
                if (typeof(TData) == ByteArray.GetType()) return (TData)(object)ByteArray;
                var jsonString = Encoding.UTF8.GetString(ByteArray);
                return JsonConvert.DeserializeObject<TData>(jsonString);
            }
            set
            {
                byte[] valueAsBytes;
                if (typeof(TData) == ByteArray.GetType())
                {
                    valueAsBytes = (byte[]) (object) value;
                }
                else
                {
                    var jsonString = JsonConvert.SerializeObject(value);
                    valueAsBytes = Encoding.UTF8.GetBytes(jsonString);
                }
                ByteArray = valueAsBytes;
            }
        }
    }
}
