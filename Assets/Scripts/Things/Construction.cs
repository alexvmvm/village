using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Village.Things
{

    public enum ConstructionGroup
    {
        Floors,
        Walls,
        Furniture,
        Objects,
        Farming
    }

    public class Construction
    {
        public ConstructionGroup Group { get { return _group; } }
        public TypeOfThing BuildType { get { return _builds; } }
        public TypeOfThing Requires { get { return _requires; } }
        private Game _game;
        private Thing _thing;
        private TypeOfThing? _buildOn;
        private TypeOfThing _builds;
        private TypeOfThing _requires;
        private ConstructionGroup _group;

        public Construction(Game game, Thing thing, TypeOfThing? buildOn, TypeOfThing builds, ConstructionGroup group, TypeOfThing requires)
        {
            _buildOn = buildOn;
            _builds = builds;
            _thing = thing;
            _game = game;
            _group = group;
            _requires = requires;
        }

        bool ConstructAtPosition(Vector2Int position)
        {
            return _game.QueryThings().Any(t => t.Construction != null && t.Position == position);
        }

        public bool IsPlaceableAt(int x, int y)
        {
            var current = _game.GetThingOnGrid(x, y);
            if (current == null)
                return false;
            if (ConstructAtPosition(current.Position))
                return false;
            if (_buildOn.HasValue && _buildOn != current.Config.TypeOfThing)
                return false;
            return current.Config.BuildSite;
        }

        public void Construct()
        {
            var thing = _game.Create(_builds, _thing.Position.x, _thing.Position.y);
            _game.AddThing(thing);
            _game.Destroy(_thing);
        }

        public static string GetGroupSprite(ConstructionGroup group)
        {
            switch (group)
            {
                case ConstructionGroup.Floors:
                    return "colored_transparent_15";
                case ConstructionGroup.Objects:
                    return "colored_transparent_15";
                case ConstructionGroup.Walls:
                    return "colored_transparent_15";
            }

            return "none";
        }
    }

}
