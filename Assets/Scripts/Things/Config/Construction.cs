using UnityEngine;
using System;
using Village.Saving;
using Village.AI;

namespace Village.Things.Config
{
    [Serializable]    
    public class ConstructionConfig
    {
        public TypeOfThing? BuildOn { get; protected set; }
        public TypeOfThing Builds { get; protected set; }
        public Thing BuildsConfig { get; private set; }
        public ConstructionGroup Group { get; protected set; }
        public TypeOfThing Requires { get; protected set; }

        public ConstructionConfig(
            TypeOfThing? buildOn,
            ConstructionGroup group,
            TypeOfThing requires
        )
        {
            BuildOn = buildOn;
            Group = group;
            Requires = requires;
        }
    }
}