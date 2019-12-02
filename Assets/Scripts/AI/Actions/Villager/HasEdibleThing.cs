using Village;
using Village.AI;

public class HasEdibleThing : SimpleGOAPAction
{
    public HasEdibleThing(GOAPAgent agent, Game game, TypeOfThing thing) : base(agent, game)
    {
        Preconditions.Add(GOAPAction.Effect.HAS_THING, thing);
        Effects.Add(GOAPAction.Effect.HAS_EDIBLE_THING, true);
    }
}
