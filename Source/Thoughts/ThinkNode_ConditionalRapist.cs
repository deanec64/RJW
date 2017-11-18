using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	//maybe no longer needed.
	public class ThinkNode_ConditionalRapist : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			if (!xxx.config.random_rape_enabled)
				return false;
			if (Mod_Settings.WildMode) return true;
			if (!xxx.is_rapist(p))
				return false;
			if (!xxx.isSingleOrPartnerNotHere(p))
			{
				return false;
			}
			else return true;
		}
	}
}