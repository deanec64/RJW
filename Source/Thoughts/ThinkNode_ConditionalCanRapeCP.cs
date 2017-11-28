using Verse;
using Verse.AI;

//TODO: Fix this class
namespace rjw
{
	public class ThinkNode_ConditionalCanRapeCP : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn pawn)
		{
			if (!xxx.config.comfort_prisoners_enabled)
			{
				return false;
			}
			if (pawn == null || pawn.Faction == null)
			{
				Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 0 : " + pawn.NameStringShort);
				return false;
			}
			if (Mod_Settings.WildMode)
			{
				return true;
			}
			// Due to the existence of whore system, no longer allow pawns from other factions to
			// rape comfort prisoners
			if (!pawn.Faction.IsPlayer)
			{
				Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 1 : " + pawn.NameStringShort);
				return false;
			}
			if (pawn.Map == null || pawn.Map != Find.VisibleMap)
			{
				Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 2 : " + pawn.NameStringShort);
				return false;
			}
			if (pawn.IsPrisonerOfColony) //TODO: Edit this
			{
				Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 3 : " + pawn.NameStringShort);
				return false;
			}
			if (xxx.config.animals_enabled && xxx.is_animal(pawn))
			{
				Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 4 : " + pawn.NameStringShort);
				return true;
			}
			else if (xxx.is_human(pawn))
			{
				Log.Message("[RJW]ThinkNode_ConditionalCanRapeCP::satisfied called 5 : " + pawn.NameStringShort);
				return xxx.isSingleOrPartnerNotHere(pawn);
			}
			else return false;
		}
	}
}