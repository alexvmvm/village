using System;
using Village.Things.Config;

namespace Village.Things.Serialization
{
    [Serializable]
    public class ThingSerializationLayout
    {
        public ThingConfig[] Parents;
        public ThingConfig[] Things;
    }
}

