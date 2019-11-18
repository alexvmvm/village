using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Village.Things.Config;

namespace Village.Things.Serialization
{
    [Serializable]
    public class ThingSerializationLayout
    {
        [XmlArray("Parents")]
        [XmlArrayItem("Thing")] 
        public ThingConfig[] Parents;

        [XmlArray("Things")]
        [XmlArrayItem("Thing")] 
        public ThingConfig[] Things;
    }
}

