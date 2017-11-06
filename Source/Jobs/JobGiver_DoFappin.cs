
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	public class JobGiver_DoFappin : ThinkNode_JobGiver
	{

		protected override Job TryGiveJob(Pawn p)
		{
			//--Log.Message("[RJW] JobGiver_DoFappin::TryGiveJob( " + p.NameStringShort + " ) called");
			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) &&
				(p.CurJob != null))
			{

				Building_Bed bed = null;

				if (p.jobs.curDriver is JobDriver_LayDown)
				{
					bed = ((JobDriver_LayDown)p.jobs.curDriver).Bed;
				}

				if (p.jobs.curDriver.layingDown != LayingDownState.NotLaying &&
					(bed != null) && //!bed.Medical &&
					xxx.is_healthy_enough(p) &&
					xxx.can_be_fucked(p))
				{

					var no_partner = xxx.is_laying_down_alone(p);
					bool is_frustrated;
					{
						is_frustrated = xxx.need_some_sex(p) == 3f;
					}

					if (xxx.config.pawns_always_do_fapping || (no_partner || is_frustrated))
					{
						p.mindState.awokeVoluntarily = true;
						return new Job(xxx.fappin, bed);
					}
				}
			}
			return null;
		}
	}
}
