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
        private string _pathToXml;
        private ThingConfig[] _things;

        public ThingSerialization(string pathToXml)
        {
            _pathToXml = pathToXml;
        }

        public ThingConfig[] LoadFromFile()
        {
            if(_things == null)
            {
                var layout = GetLayoutFromFile();
                _things = GetThingsFromLayoutFile(layout);
            }

            return _things;
        }

        ThingSerializationLayout GetLayoutFromFile()
        {
            var serializer = new XmlSerializer(typeof(ThingSerializationLayout));
            var reader = new StreamReader(_pathToXml);
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
                    var parent = layout.Parents.FirstOrDefault(p => p.Name == p.Parent);
                    if(parent == null)
                        throw new Exception($"Unable to find parent thing {t.Parent} in config file {_pathToXml}");
                    return GetThingConfigFromParentChild(parent, t);
                }
            }).ToArray();
        }

        ThingConfig GetThingConfigFromParentChild(ThingConfig parent, ThingConfig child)
        {        
            var properties = child.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                property.SetValue(parent, property.GetValue(child));
            }
            return child;
        }
    }
}
