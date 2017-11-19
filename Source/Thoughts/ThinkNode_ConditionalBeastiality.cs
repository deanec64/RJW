using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class ThinkNode_ConditionalBeastiality : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			return xxx.config.beastiality_enabled && xxx.is_zoophiliac(p);
		}
	}
}