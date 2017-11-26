using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobGiver_NymphJoinInBed : ThinkNode_JobGiver
	{
		private static bool roll_to_skip(Pawn nymph, Pawn p)
		{
			var fuckability = xxx.would_fuck(nymph, p); // 0.0 to 1.0
			var chance_to_skip = 0.9f - 0.7f * fuckability;
			return Rand.Value < chance_to_skip;
		}

		private static bool is_healthy(Pawn p)
		{
			return (!p.Dead) &&
				p.health.capacities.CanBeAwake &&
				(p.health.hediffSet.BleedRateTotal <= 0.0f) &&
				(p.health.hediffSet.PainTotal < xxx.config.significant_pain_threshold) &&
				xxx.can_fuck(p);
		}

		public static Pawn find_pawn_to_fuck(Pawn nymph, Map map)
		{
			Pawn best_fuckee = null;
			float best_distance = 1.0e6f;
			foreach (var q in map.mapPawns.FreeColonistsAndPrisoners)
				if ((q != nymph) &&
					xxx.is_laying_down_alone(q) &&
					nymph.CanReserve(q, 1, 0) &&
					q.CanReserve(nymph, 1, 0) &&
					roll_to_skip(nymph, q) &&
					(!q.Position.IsForbidden(nymph)) &&
					is_healthy(q))
				{
					var dis = nymph.Position.DistanceToSquared(q.Position);
					if (dis < best_distance)
					{
						best_fuckee = q;
						best_distance = dis;
					}
				}
			return best_fuckee;
		}

		protected override Job TryGiveJob(Pawn p)
		{
			//--Log.Message("[RJW] JobGiver_NymphJoinInBed( " + p.NameStringShort + " ) called");

			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null))
			{
				//--Log.Message("   checking nympho and abilities");
				if (xxx.is_nympho(p) && p.health.capacities.CanBeAwake && xxx.can_fuck(p))
				{
					//--Log.Message("   finding partner");
					var partner = find_pawn_to_fuck(p, p.Map);

					Building_Bed bed = null;

					if (partner != null && partner.jobs.curDriver is JobDriver_LayDown)
					{
						bed = ((JobDriver_LayDown)partner.jobs.curDriver).Bed;
					}

					//--Log.Message("   checking partner");
					if (partner != null)
					{
						//--Log.Message("   checking partner's job");
						if (partner.CurJob != null)
						{
							//--Log.Message("   checking partner again");
							if ((partner != null))
							{
								//--Log.Message("   checking bed");
								if ((bed != null))
								{
									//--Log.Message("   returning job");
									return new Job(DefDatabase<JobDef>.GetNamed("NymphJoinInBed"), partner, bed);
								}
								else
								{
									//--Log.Message("   resetting ticks");
									p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
								}
							}
						}
					}
				}
			}

			return null;
		}
	}
}