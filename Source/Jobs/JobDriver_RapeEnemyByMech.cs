

using RimWorld;
using System;
using System.Linq;
using Verse;

namespace rjw
{
	class JobDriver_RapeEnemyByMech : JobDriver_RapeEnemy
	{


		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return rapist.RaceProps.IsMechanoid;
		}
		public override float GetFuckability(Pawn rapist, Pawn target)
		{
			//Log.Message("[RJW]JobDriver_RapeEnemyByMech::GetFuckability("+ rapist.ToString()+","+ target.ToString() + ") - Force Rape");
			return 1f;
		}

		public override void roll_to_hit(Pawn rapist, Pawn p)
		{
			float rand_value = (1 - Rand.Value * p.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness));
			float victim_pain = p.health.hediffSet.PainTotal;

			float beating_chance = xxx.config.base_chance_to_hit_prisoner * (xxx.is_bloodlust(rapist) ? 1.25f : 1.0f);
			float beating_threshold = xxx.is_psychopath(rapist) ? xxx.config.extreme_pain_threshold : xxx.config.significant_pain_threshold;

			if ((victim_pain < beating_threshold && rand_value < beating_chance) || (rand_value < (beating_chance / 2)))
			{
				if (InteractionUtility.TryGetRandomVerbForSocialFight(rapist, out Verb v))
				{
					rapist.meleeVerbs.TryMeleeAttack(p, v);
				}
			}
		}
		public override void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex = false)
		{
			base.aftersex(pawn, part, violent, isCoreLovin, isAnalSex);

			if (pawn.RaceProps.IsMechanoid && xxx.is_human(part))
            {
                //Log.Message("[RJW]JobDriver_RapeEnemyByMech::aftersex - mech raped humans");
				/*foreach (var item in DefDatabase<HediffDef_MechImplants>.AllDefs)
				{
					Log.Message(pawn.def.defName + "Getting Implants\n" +item.defName + "\nParentDef:" + item.parentDef + "\nParentDefs:" + String.Join(",",item.parentDefs.ToArray()) );
				}*/
				HediffDef_MechImplants egg = (from x in DefDatabase<HediffDef_MechImplants>.AllDefs where x.parentDef == pawn.def.defName || x.parentDefs.Contains(pawn.def.defName) select x).RandomElement<HediffDef_MechImplants>();

				PlantSomething(egg, part, isAnalSex, 1);
            }
		}
	}
}
