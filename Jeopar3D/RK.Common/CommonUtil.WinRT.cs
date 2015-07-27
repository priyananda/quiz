using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace RK.Common
{
    public static partial class CommonUtil
    {
        /// <summary>
        /// Deserializes an object of the given type from the given storage file.
        /// </summary>
        /// <param name="storagefile">The file to deserialize from.</param>
        public static async Task<T> DeserializeFromXmlFile<T>(StorageFile storagefile)
            where T : class
        {
            try
            {
                //Open file for reading
                using (Stream inStream = await storagefile.OpenStreamForReadAsync())
                {
                    //Create the serializer
                    XmlSerializer serializer = await Task.Factory.StartNew(() => new XmlSerializer(typeof(T)));

                    //Deserialize the object and return the result
                    return await Task.Factory.StartNew(() => serializer.Deserialize(inStream) as T);
                }
            }
            catch (FileNotFoundException) { }

            //Return default value if something went wrong
            return default(T);
        }

        /// <summary>
        /// Serializes the given object to the given storage file.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="storageFile">The target file.</param>
        /// <param name="objectToSerialize">The object to serialize.</param>
        public static async Task SerializeToXmlFile<T>(StorageFile storageFile, T objectToSerialize)
        {
            using (Stream outStream = await storageFile.OpenStreamForWriteAsync())
            {
                //Create the serializer
                XmlSerializer serializer = await Task.Factory.StartNew(() => new XmlSerializer(typeof(T)));

                await Task.Factory.StartNew(() => serializer.Serialize(outStream, objectToSerialize));

                return;
            }
        }
    }
}