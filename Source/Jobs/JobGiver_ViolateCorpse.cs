using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobGiver_ViolateCorpse : ThinkNode_JobGiver
	{
		public static Corpse find_corpse(Pawn rapist, Map m)
		{
			//Log.Message("JobGiver_ViolateCorpse::find_corpse( " + rapist.NameStringShort + " ) called");
			Corpse found = null;
			var best_distance = 100f;
			int best_freshness = 10;

			foreach (Corpse corpse in m.listerThings.ThingsOfDef(ThingDef.Named("Human_Corpse")))
			{
				//Log.Message(rapist.NameStringShort + " found a corpse with id " + thing.Label);
				if (rapist.CanReserve(corpse, 1, 0) && !corpse.IsForbidden(rapist))
				{
					int freshness = corpse.GetRotStage().ChangeType<int>();
					var distance = rapist.Position.DistanceToSquared(corpse.Position);
					//Log.Message("   " + corpse.InnerPawn.NameStringShort + " =  " + freshness + "/" + distance + ",  best =  " + best_freshness + "/" + best_distance);
					if (freshness > best_freshness && distance < best_distance)
					{
						found = corpse;
						best_freshness = freshness;
						best_distance = distance;
					}
				}
			}

			return found;
		}

		protected override Job TryGiveJob(Pawn p)
		{
			Log.Message("[RJW] JobGiver_ViolateCorpse::TryGiveJob( " + p.NameStringShort + " ) called");
			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null))
			{
				if (xxx.is_healthy(p) && xxx.can_rape(p) && !comfort_prisoners.is_designated(p))
				{
					var target = find_corpse(p, p.Map);
					Log.Message("[RJW] JobGiver_ViolateCorpse::TryGiveJob - target is " + (target == null ? "NULL" : "Found"));
					if (target != null)
					{
						Messages.Message(p.NameStringShort + " is trying to rape a corpse", p, MessageTypeDefOf.NeutralEvent);
						return new Job(xxx.violate_corpse, target);
					}
					else
					{
						p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(150, 300);
					}
				}
			}

			return null;
		}
	}
}