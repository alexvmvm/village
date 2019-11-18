using UnityEngine;
using System;
using Village.Saving;
using Village.AI;

namespace Village.Things.Config
{
    [Serializable]    
    public class FactoryConfig
    {
        public TypeOfThing[] Produces;
    }
}