using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobGiver_RandomRape : ThinkNode_JobGiver
	{
		public static Pawn find_victim(Pawn rapist, Map m)
		{
			Pawn best_rapee = null;
			var best_fuckability = 0.10f; // Don't rape prisoners with <10% fuckability
			foreach (var target in m.mapPawns.AllPawns.Where(x => x != rapist && xxx.is_healthy_enough(x) && xxx.can_get_raped(x)))
			{
				if (rapist.CanReserve(target, comfort_prisoners.max_rapists_per_prisoner, 0) && !target.Position.IsForbidden(rapist))
				{
					if (xxx.config.rapists_always_rape || (!xxx.is_animal(target) || xxx.is_zoophiliac(rapist)))
					{
						var fuc = xxx.would_fuck(rapist, target, true,true);
						if ((fuc > best_fuckability) && (Rand.Value < fuc))
						{
							best_rapee = target;
							best_fuckability = fuc;
						}
					}
				}
			}
			return best_rapee;
		}

		protected override Job TryGiveJob(Pawn p)
		{
			//--Log.Message("[RJW] JobGiver_RandomRape::TryGiveJob( " + p.NameStringShort + " ) called");

			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null))
			{
				// don't allow pawns marked as comfort prisoners to rape others
				if (xxx.is_healthy(p) && xxx.can_rape(p, true))
				{
					var prisoner = find_victim(p, p.Map);

					if (prisoner != null)
					{
						//--Log.Message("[RJW] JobGiver_RandomRape::TryGiveJob( " + p.NameStringShort + " ) - found victim " + prisoner.NameStringShort);
						
						return new Job(xxx.random_rape, prisoner);
					}
					else
					{
						//TODO: Remove this later (Hoge's recommendation)
						//--Log.Message("[RJW] JobGiver_RandomRape::TryGiveJob( " + p.NameStringShort + " ) - unable to find victim");
						if (xxx.config.rapists_always_rape)
							p.mindState.canLovinTick = Find.TickManager.TicksGame + 5;
						else p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
					}
				}
			}

			return null;
		}
	}
}