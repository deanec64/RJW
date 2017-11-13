using RimWorld;
using Verse;

namespace rjw
{
	public class ThinkNode_ConditionalWhore : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			return xxx.config.whore_beds_enabled && p.IsFreeColonist && xxx.is_whore(p);
		}
	}
}