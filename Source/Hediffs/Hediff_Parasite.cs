using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace rjw
{
	internal class Hediff_Parasite : Hediff_Pregnant
	{
		new public static void DoBirthSpawn(Pawn mother, Pawn father)
		{
			int num = (mother.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(mother.RaceProps.litterSizeCurve, 300));
			if (num < 1)
			{
				num = 1;
			}
			PawnGenerationRequest request = new PawnGenerationRequest(father.kindDef, father.Faction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false,false, null, null, null, null);
			Pawn pawn = null;
			for (int i = 0; i < num; i++)
			{
				pawn = PawnGenerator.GeneratePawn(request);
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, mother))
				{
					if (pawn.playerSettings != null && mother.playerSettings != null)
					{
						pawn.playerSettings.AreaRestriction = father.playerSettings.AreaRestriction;
					}
					if (pawn.RaceProps.IsFlesh)
					{
						pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
						if (father != null)
						{
							pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
						}
					}
				}
				else
				{
					Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
				}
			}
			if (mother.Spawned)
			{
				FilthMaker.MakeFilth(mother.Position, mother.Map, ThingDefOf.FilthAmnioticFluid, mother.LabelIndefinite(), 5);
				if (mother.caller != null)
				{
					mother.caller.DoCall();
				}
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
		}
	}
}