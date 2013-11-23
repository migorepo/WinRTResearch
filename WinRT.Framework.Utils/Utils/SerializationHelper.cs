using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace WinRT.Framework.Utils.Utils
{
    /// <summary>
    /// Helper class for Json / xml serialization and deserialization
    /// </summary>
    public class SerializationHelper
    {
        /// <summary>
        /// Method for deserializng the xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(string xmlString)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var xmlText = new StringReader(xmlString);
                var xmlObject = (T)xmlSerializer.Deserialize(xmlText);
                return xmlObject;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Method for serializing the object to json string 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <returns></returns>
        public static string SerializeXml<T>(T xmlObject)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var xmlStringBuilder = new StringBuilder();
                var xmlText = new StringWriter(xmlStringBuilder);
                xmlSerializer.Serialize(xmlText, xmlObject);

                return xmlStringBuilder.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Method for deserializng the json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T DeserializeJson<T>(string jsonString)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(jsonString);
                return result;
            }
            catch
            {
                return default(T);
            }
        }


        /// <summary>
        /// Method for serializing the object to json string 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static string SerializeJson<T>(T jsonObject)
        {
            try
            {
                var result = JsonConvert.SerializeObject(jsonObject);
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
