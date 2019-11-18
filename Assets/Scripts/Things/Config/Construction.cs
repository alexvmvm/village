using UnityEngine;
using System;
using Village.Saving;
using Village.AI;

namespace Village.Things.Config
{
    [Serializable]    
    public class ConstructionConfig
    {
        public TypeOfThing? BuildOn;
        public TypeOfThing Builds;
        public Thing BuildsConfig;
        public ConstructionGroup Group;
        public TypeOfThing Requires;

        //         public ConstructionConfig(
        //     TypeOfThing? buildOn,
        //     ConstructionGroup group,
        //     TypeOfThing requires
        // )
        // {
        //     BuildOn = buildOn;
        //     Group = group;
        //     Requires = requires;
        // }
    }
}