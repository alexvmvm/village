using System;

namespace Village.Things.Config
{
    [Serializable]    
    public class CropConfig
    {
        public float TimeToGrow { get; protected set; }
        public string[] Sprites { get; protected set; }
        public TypeOfThing Produces { get; protected set; }
        
        public CropConfig(float timeToGrow, string[] sprites, TypeOfThing produces)
        {
            TimeToGrow = timeToGrow;
            Sprites = sprites;
            Produces = produces;
        }
    }
}