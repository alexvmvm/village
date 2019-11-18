using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Linq;
using Village.Things.Config;
using System;

namespace Village.Things.Serialization
{
    public class ThingSerialization 
    {
        private ThingConfig[] _things;


        public ThingConfig[] LoadFromFile(string pathToXml)
        {
            return GetThingsFromLayoutFile(GetLayoutFromFile(new StreamReader(pathToXml) as TextReader));
        }

        public ThingConfig[] LoadFromString(string xml)
        {
            return GetThingsFromLayoutFile(GetLayoutFromFile(new StringReader(xml.Trim())));
        }

        ThingSerializationLayout GetLayoutFromFile(TextReader reader)
        {
            var serializer = new XmlSerializer(typeof(ThingSerializationLayout));
            var obj = (ThingSerializationLayout)serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }

        ThingConfig[] GetThingsFromLayoutFile(ThingSerializationLayout layout)
        {
            return layout.Things.Select(t => {
                if(string.IsNullOrEmpty(t.Parent))
                    return t;
                else
                {
                    var parent = layout.Parents.FirstOrDefault(p => p.Name == t.Parent);
                    if(parent == null)
                        throw new Exception($"Unable to find parent thing {t.Parent}");
                    return GetThingConfigFromParentChild(parent, t);
                }
            }).ToArray();
        }

        ThingConfig GetThingConfigFromParentChild(ThingConfig parent, ThingConfig child)
        {        
            var defaultConfig = new ThingConfig();
            var properties = parent.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in properties)
            {
                var value = field.GetValue(child);
                var defaultValue = field.GetValue(defaultConfig);
                if(value == null || value.Equals(defaultValue))
                    continue;
                field.SetValue(parent, field.GetValue(child));
            }
            return parent;
        }
    }
}
