

using RimWorld;
using System.Linq;
using Verse;

namespace rjw
{
    class JobDriver_RapeEnemyByInsect : JobDriver_RapeEnemy
    {

		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return ABFCommon.is_Insect(rapist);
		}
		public override void roll_to_hit(Pawn rapist, Pawn p)
		{
			float rand_value = (1-Rand.Value* p.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness));
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
            base.aftersex(pawn,part,violent,isCoreLovin,isAnalSex);
			if (xxx.is_human(part))
			{

				if (pawn.gender == Gender.Female)
				{
					HediffDef_InsectEgg egg = (from x in DefDatabase<HediffDef_InsectEgg>.AllDefs where x.IsParent(pawn.def.defName) select x).RandomElement<HediffDef_InsectEgg>();
					if (egg != null)
					{
						Log.Message("[RJW]JobDriver_RapeEnemyByInsect::aftersex() - planting egg " + egg.ToString());
						PlantSomething(egg, part, isAnalSex, Rand.Range(1, 2));
					}
					else
					{
						Log.Message("[RJW]JobDriver_RapeEnemyByInsect::aftersex() - There is no EggData of " + pawn.def.defName);
					}
				}
				else
				{
					foreach (var egg in (from x in part.health.hediffSet.GetHediffs<Hediff_InsectEgg>() where x.IsParent(pawn.def.defName) select x) )
					{
						egg.Fertilize(pawn);
					}
				}
			}
		}

		public override float GetFuckability(Pawn rapist, Pawn target)
		{
			if (rapist.gender == Gender.Female)
			{
				//Log.Message("[RJW]" + this.GetType().ToString() + "::GetFuckability(" + rapist.ToString() + ") - going to plant egg ->"+ target.ToString());
				return 1f; //Plant Eggs to everyone.
			}
			else
			{
				if ((from x in target.health.hediffSet.GetHediffs<Hediff_InsectEgg>() where x.IsParent(rapist.def.defName) select x).Count() > 0)
				{
					return 1f;//Trying to feritlize eggs to everyone planted eggs.
				}
			}
			return 0f;
		}
	}
}
