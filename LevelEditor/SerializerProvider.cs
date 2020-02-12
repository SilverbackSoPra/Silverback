using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LevelEditor.Engine;

namespace LevelEditor
{
    public static class SerializerProvider
    {
        private static readonly Dictionary<string, XmlSerializer> cache =
            new Dictionary<string, XmlSerializer>();

        public static XmlSerializer Create(Type type, XmlRootAttribute root)
        {
            var key = string.Format(
                CultureInfo.InvariantCulture,
                "{0}:{1}",
                type,
                root.ElementName);

            if (!cache.ContainsKey(key))
            {
                cache.Add(key, new XmlSerializer(type, root));
            }

            return cache[key];
        }

        public static Level mLevel = null;
    }
}
