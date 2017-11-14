using RimWorld;
using Verse;

namespace rjw
{
	public class ThinkNode_ConditionalNecro : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			return xxx.config.necro_enabled && xxx.is_necrophiliac(p);
		}
	}
}