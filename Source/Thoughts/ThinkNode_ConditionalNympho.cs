using RimWorld;
using Verse;

namespace rjw
{
	public class ThinkNode_ConditionalNympho : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			return xxx.is_nympho(p);
		}
	}
}