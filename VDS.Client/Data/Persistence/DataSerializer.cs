/*
 * Data Serializer, using DataContractSerializer, which can also be used for communication / WCF
 * The data (which is XML string) should / will be compressed before storing
 */

//#define DEBUG_ON

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using CIS681.Fall2012.VDS.UI;

namespace CIS681.Fall2012.VDS.Data {
    public sealed class DataSerializer<T> {
        // target object, must has a value
        private T target = default(T);
        // serialization encoding
        public Encoding encoding { get; set; }

        public DataSerializer(T target) {
            if (target == null)
                throw new ArgumentNullException("Serialization target MUST NOT be NULL.");
            encoding = Encoding.UTF8;   // by default it is UTF8
            this.target = target;
        }

        #region Serialize to File
        /// <summary>
        /// Save to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path) {
            Save(target, path, encoding);
        }
        public void Save(Stream stream) {
            Save(target, stream, encoding);
        }
        public static void Save(T target, string path) {
            Save(target, path, Encoding.UTF8);
        }
        public static void Save(T target, string path, Encoding encoding) {
            using (FileStream fs = new FileStream(Path.GetFullPath(path), FileMode.Create)) {
                Save(target, fs, encoding);
            }
        }
        public static void Save(T target, Stream stream, Encoding encoding) {
            using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream, encoding)) {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(writer, target);
                // finish serialization
                writer.Flush();
            }
        }
        public static void Save(T target, Stream stream) {
            Save(target, stream, Encoding.UTF8);
        }
        #endregion

        #region Deserialize from File
        public static T Load(string path) {
            return Load(path, Encoding.UTF8);
        }
        public static T Load(Stream stream) {
            return Load(stream, Encoding.UTF8);
        }
        public static T Load(Stream stream, Encoding encoding) {
            T instance = default(T);
            try {
                using (XmlDictionaryReader reader =
                        XmlDictionaryReader.CreateTextReader(stream, encoding, new XmlDictionaryReaderQuotas(), null)) {
                    // Create the DataContractSerializer instance.
                    DataContractSerializer ser = new DataContractSerializer(typeof(T));
                    // Deserialize the data and read it from the instance.
                    instance = (T)ser.ReadObject(reader);
                }
            }
            catch (Exception e) {
                new ErrorHandler("Data corrupted.\nPlease ensure the format is valid.", e).Show();
            }
            return instance;
        }

        /// <summary>
        /// Load objects from file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T Load(string path, Encoding encoding) {
            T instance = default(T);
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
                instance = Load(fs, encoding);
            }
            return instance;
        }
        #endregion

        #region Serialize to String
        public string Serialize() {
            return Serialize(target, encoding);
        }
        public static string Serialize(T target) {
            return Serialize(target, Encoding.UTF8);
        }
        public static string Serialize(T target, Encoding encoding) {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream, encoding)) {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, target);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }
        #endregion

        #region Deserialize from String
        public static T Deserialize(string xml) {
            return Deserialize(xml, Encoding.UTF8);
        }
        public static T Deserialize(string xml, Encoding encoding) {
            using (Stream stream = new MemoryStream()) {
                byte[] data = encoding.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
                return (T)deserializer.ReadObject(stream);
            }
        }
        #endregion
    }
}

#if DEBUG_ON
namespace CIS681.Fall2012.VDS.Data {
    class TEST_CLASS {
        public static void Main() {
            Project p = new Project();
            Test_Serialize(p);
            Project p2 = Test_Deserialize(); 
            // insert breakpoint here to see if p & p2 are the same
        }
        // test serialization here
        private static void Test_Serialize(Project p) {
            string filePath = "X:/test.xml";
            DataSerializer<CIS681.Fall2012.VDS.Data.Project> s = new DataSerializer<CIS681.Fall2012.VDS.Data.Project>(p);
            Diagram d = new Diagram();
            p.Children.Add(d);
            s.Save(filePath);
        }
        private static Project Test_Deserialize() {
            string filePath = "X:/test.xml";
            Project p = DataSerializer<Project>.Load(filePath);
            return p;
        }
    }
}
#endif