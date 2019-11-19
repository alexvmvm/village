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
        public ThingConfig[] LoadFromString(string xml)
        {
            var layout = GetLayoutFromFile(new StringReader(xml.Trim()));
            return GetThingsFromLayoutFile(layout);
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
            var properties = parent.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            var parentCopy = new ThingConfig();
            foreach (FieldInfo field in properties)
            {
                field.SetValue(parentCopy, field.GetValue(parent));
            }

            var defaultConfig = new ThingConfig();
            foreach (FieldInfo field in properties)
            {
                var value = field.GetValue(child);
                var defaultValue = field.GetValue(defaultConfig);
                if(value == null || value.Equals(defaultValue))
                    continue;
                field.SetValue(parentCopy, field.GetValue(child));
            }
            return parentCopy;
        }
    }
}
