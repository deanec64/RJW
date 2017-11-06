
using System;

using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace rjw
{
    public class JobGiver_RapeEnemy : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn p)
        {
			//Log.Message("[RJW] JobGiver_RapeEnemy::TryGiveJob( " + p.NameStringShort + " ) called");
			if (!p.health.capacities.CanBeAwake || Find.TickManager.TicksGame < p.mindState.canLovinTick || p.CurJob != null ) return null;

			JobDef_RapeEnemy rapeEnemyJobDef = null;
			int? highestPriority = null;
			foreach (JobDef_RapeEnemy job in DefDatabase<JobDef_RapeEnemy>.AllDefs)
			{
				if (job.CanUseThisJobForPawn(p))
				{
					if (highestPriority == null)
					{
						rapeEnemyJobDef = job;
						highestPriority = job.priority;
					}
					else if (job.priority > highestPriority)
					{
						rapeEnemyJobDef = job;
						highestPriority = job.priority;
					}
				}
			}

			if (rapeEnemyJobDef == null)
			{
				//Log.Warning("[RJW] JobGiver_RapeEnemy::ChoosedJobDef( " + p.ToString()+ " ) no defined JobDef_RapeEnemy for him.");
				return null;
			}
			//Log.Message("[RJW] JobGiver_RapeEnemy::ChoosedJobDef( " + p.ToString() + " ) - " + rapeEnemyJobDef.ToString() + " choosed");
            var downedPlayer = rapeEnemyJobDef.FindVictim(p, p.Map);
                    
            if (downedPlayer != null)
            {
                //Log.Message("[RJW]" + this.GetType().ToString() + "::TryGiveJob( " + p.NameStringShort + " ) - found victim " + downedPlayer.NameStringShort);
                p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
                return new Job(rapeEnemyJobDef, downedPlayer);
            }
            else {
                //Log.Message("[RJW]" + this.GetType().ToString() + "::TryGiveJob( " + p.NameStringShort + " ) - unable to find victim");
                p.mindState.canLovinTick = Find.TickManager.TicksGame + 1;
            }
			//else {  Log.Message("[RJW] JobGiver_RapeEnemy::TryGiveJob( " + p.NameStringShort + " ) - too fast to play next"); }

			return null;
        }
        
    }

}
