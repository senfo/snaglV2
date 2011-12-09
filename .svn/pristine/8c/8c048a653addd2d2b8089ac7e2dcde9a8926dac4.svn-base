//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace Berico.Common
{
    /// <summary>
    /// Helper class for functionality related to JSON
    /// </summary>
    public class JSONHelper
    {
        /// <summary>
        /// Serializes a class to Json.  The class being serialized
        /// should be tagged with the DataContract attribute and each
        /// member should have the DataMemeber attribute.
        /// </summary>
        /// <typeparam name="T">The type of the data that is being serialized</typeparam>
        /// <param name="obj">The object that should be serialized to Json</param>
        /// <returns>a string containing the serialized Json</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">Indicates an exception with serialization</exception>
        public static string Serialize<T>(T obj)
        {
            byte[] jsonData;
            string results = string.Empty;

            // Create instance of Json serializer
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

            // Use a memory stream for storing results
            using (MemoryStream ms = new MemoryStream())
            {
                // Serialize the provided class to a Json
                serializer.WriteObject(ms, obj);

                // Save the json data to a byte array
                jsonData = ms.ToArray();

                // Close the memory stream
                ms.Close();
            }

            // Return the results
            if (jsonData.Length == 0)
                return string.Empty;
            else
                return Encoding.UTF8.GetString(jsonData, 0, jsonData.Length);

        }

        /// <summary>
        /// Deserializes a Json string to the target object type
        /// </summary>
        /// <typeparam name="T">The type of the object that the Json will be
        /// deserialized too</typeparam>
        /// <param name="json">The Json string to be deserialized</param>
        /// <returns>an instance of the object that was serialized</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">Indicates an exception with deserialization</exception>
        public static T Deserialize<T>(string json) where T : class
        {
            // Validate parameter
            if (string.IsNullOrEmpty(json))
                return default(T);

            // Use reflection to create an instance of the type
            // specified by the generic parameter
            T obj = Activator.CreateInstance<T>();

            // Create a memory stream and get the bytes for the 
            // input Json string
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                // Create instance of Json serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

                // Deserialize the string
                obj = serializer.ReadObject(ms) as T;

                // Close the memory stream
                ms.Close();
            }

            return obj;
        }
    }
}