using System;

namespace Village.Things.Config
{
    [Serializable]    
    public class CropConfig
    {
        public float TimeToGrow;
        public string[] Sprites;
        public TypeOfThing Produces;
    }
}