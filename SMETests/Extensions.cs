using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace SMETests
{
    internal static class ParseHelpers
    {
        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static T ParseXML<T>(this string @this) where T : class
        {
            var reader = XmlReader.Create(@this.StripQuestions().Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }

        public static string StripQuestions(this string @this)
        {
            var pattern = new Regex("([^.?!]*)\\?", RegexOptions.Compiled);

            var result = pattern.Replace(@this, String.Empty);

            return result;
        }
    }
}
