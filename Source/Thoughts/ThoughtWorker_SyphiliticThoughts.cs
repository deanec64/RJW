using RimWorld;
using Verse;

namespace rjw
{
	public class ThoughtWorker_SyphiliticThoughts : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			var syp = p.health.hediffSet.GetFirstHediffOfDef(std.syphilis.hediff_def);
			if (syp != null)
			{
				if (syp.Severity >= 0.80f)
					return ThoughtState.ActiveAtStage(1);
				else if (syp.Severity >= 0.50f)
					return ThoughtState.ActiveAtStage(0);
			}
			return ThoughtState.Inactive;
		}
	}
}