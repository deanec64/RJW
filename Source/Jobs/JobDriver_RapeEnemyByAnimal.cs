
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace rjw
{
	public class JobDriver_RapeEnemyByAnimal : JobDriver_RapeEnemy
	{

		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return xxx.is_animal(rapist) && xxx.can_fuck(rapist);
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
	}
}
