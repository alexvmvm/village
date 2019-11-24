using System;

namespace Village.Things.Config
{
    public class ConstructionConfig
    {
        public TypeOfThing? BuildOn;
        public ConstructionGroup Group;
        public TypeOfThing Requires;
        public TypeOfThing Builds;
    }
}