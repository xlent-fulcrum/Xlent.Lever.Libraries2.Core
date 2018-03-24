using System.Text;
using Newtonsoft.Json;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    internal static class SupportMethods
    {

        /// <summary>
        /// Serialize an item into a byte array,
        /// </summary>
        public static byte[] Serialize<T>(T item)
        {
            var itemAsJsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(itemAsJsonString);
        }

        /// <summary>
        /// Deserialize an item from a byte array,
        /// </summary>

        public static T Deserialize<T>(byte[] itemAsBytes)
        {
            var itemAsJsonString = Encoding.UTF8.GetString(itemAsBytes);
            return JsonConvert.DeserializeObject<T>(itemAsJsonString);
        }
    }
}
