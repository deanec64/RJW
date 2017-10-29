using Verse;
using RimWorld;

namespace rjw {
	public class ThoughtWorker_FeelingBroken : ThoughtWorker {

		protected override ThoughtState CurrentStateInternal (Pawn p)
		{
            var brokenstages = p.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken);
            if (brokenstages != null)
            {
                switch (brokenstages.CurStageIndex)
                {
                    case 0:
                        return ThoughtState.ActiveAtStage(0);
                    case 1:
                        return ThoughtState.ActiveAtStage(1);
                    case 2:
                        return ThoughtState.ActiveAtStage(2);
                    default:
                        return ThoughtState.Inactive;
                }
            }
            return ThoughtState.Inactive;

        }
	
	}
}
