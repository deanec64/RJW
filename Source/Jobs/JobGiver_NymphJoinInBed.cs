
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobGiver_NymphJoinInBed : ThinkNode_JobGiver {

		private static bool roll_to_skip (Pawn nymph, Pawn p)
		{
            if (xxx.config.nymphs_always_JoinInBed) return true;
			var fuckability = xxx.would_fuck (nymph, p); // 0.0 to 1.0
			return Rand.Value < fuckability*1.05f;
		}
		
		public static Pawn find_pawn_to_fuck (Pawn nymph, Map map)
		{
			Pawn best_fuckee = null;
			float best_distance = 1.0e6f;
			foreach (var q in map.mapPawns.FreeColonistsAndPrisoners.Where(x=> xxx.is_laying_down_alone(x)&&xxx.can_be_fucked(x)&&xxx.need_some_sex(x)>1))  //need_some_sex assumes q as humans
				if ((q != nymph) &&
				    nymph.CanReserve (q, 1, 0) &&
				    q.CanReserve (nymph, 1, 0) &&
                    !q.Position.IsForbidden(nymph) &&
                    xxx.is_healthy_enough(q) &&
                    roll_to_skip (nymph, q) ) {
						var dis = nymph.Position.DistanceToSquared (q.Position);
						if (dis < best_distance) {
							best_fuckee = q;
							best_distance = dis;
						}
					}
			return best_fuckee;
		}

		protected override Job TryGiveJob (Pawn p)
		{
            //Log.Message("[RJW] JobGiver_NymphJoinInBed( " + p.NameStringShort + " ) called0");
			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null))
            {
                Log.Message("[RJW] JobGiver_NymphJoinInBed( " + p.NameStringShort + " ) called1 - checking health and sex abilities");
                if (xxx.is_healthy(p) && xxx.can_fuck (p)) {
                    //Log.Message("   finding partner");
					var partner = find_pawn_to_fuck (p, p.Map);

					Building_Bed bed = null;
                    Log.Message("[RJW] JobGiver_NymphJoinInBed called2 - partner is "+(partner==null? "NULL" : partner.NameStringShort));
                    if (partner == null) {
                        p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(100, 300);
                        return null;
                    }
					if (partner.jobs.curDriver is JobDriver_LayDown) {
						bed = ((JobDriver_LayDown)partner.jobs.curDriver).Bed;
                    }
                    //Log.Message("[RJW] JobGiver_NymphJoinInBed called3 - checking partner's job");
                    if (partner.CurJob != null)
                    {
                        //Log.Message("   checking bed");
                        if ((bed != null))
                        {
                            Log.Message("[RJW]JobGiver_NymphJoinInBed called4 - returning job");
                            return new Job(xxx.nymph_rapin, partner, bed);
                        }
                        else
                        {
                            //Log.Message("   resetting ticks");
                            if (xxx.config.nymphs_always_JoinInBed)
                                p.mindState.canLovinTick = Find.TickManager.TicksGame + 5;
                            else p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(100, 300);
                        }
                    }
                }
			}
			return null;
		}
	}
}
