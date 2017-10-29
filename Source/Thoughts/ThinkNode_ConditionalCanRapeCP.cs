
using Verse;
using RimWorld;

namespace rjw {
	public class ThinkNode_ConditionalCanRapeCP : ThinkNode_Conditional
	{
		protected override bool Satisfied (Pawn pawn)
		{
			if (!xxx.config.comfort_prisoners_enabled) {
				return false;
			}
            if (pawn == null||pawn.Faction==null)
            {
                //Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 0");
                return false;
            }
			// Due to the existence of whore system, no longer allow pawns from other factions to rape comfort prisoners
			if (HugsLibInj.WildMode) return true;
			if (!pawn.Faction.IsPlayer)
            {
                //Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 1");
                return false;
            }
            if (pawn.Map==null||pawn.Map != Find.VisibleMap)
            {
                //Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 2");
                return false;
            }
            if (pawn.IsPrisonerOfColony)
            {
                //Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 3");
                return false;
            }
            if (xxx.config.animals_enabled && xxx.is_animal(pawn))
            {
                //Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 4");
                return true;
            }
            else if (xxx.is_human(pawn))
            {
                return xxx.isSingleOrPartnerNotHere(pawn);
            }
            else return false;
            
        }
	}
}
