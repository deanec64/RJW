using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	public class JobGiver_Beastiality : ThinkNode_JobGiver
	{

		public static Pawn find_target(Pawn pawn, Map m)
		{
			//Log.Message("JobGiver_Beastiality::find_target( " + pawn.NameStringShort + " ) called");
			Pawn found = null;
			var best_distance = 50f;
			var best_fuckability = 0.1f; // Don't rape animals with 0% fuckability

			foreach (Pawn target in m.mapPawns.AllPawns.Where(x => xxx.is_animal(x) && xxx.can_get_raped(x) && pawn.CanReserve(x, 1, 0) && (x.gender != pawn.gender) && (Rand.Value > 0.5f))) //Added randomizer. We don't need to check all animals on the map, do we?
			{
				if (target != null && !target.Position.IsForbidden(pawn))
				{
					float tamed = (target.Faction == pawn.Faction ? 1f : 0f);
					float wildness = target.RaceProps.wildness;
					float petness = target.RaceProps.petness;
					//float temperment = (petness <= 0 ? wildness / 0.1f : wildness / petness);
					//Log.Message("[RJW]JobGiver_Beastiality::find_target wildness is " + wildness);
					//Log.Message("[RJW]JobGiver_Beastiality::find_target petness is " + petness);
					//Log.Message("[RJW]JobGiver_Beastiality::find_target temperment is " + temperment);

					float distance = pawn.Position.DistanceToSquared(target.Position);
					//Log.Message("[RJW]JobGiver_Beastiality::find_target distance is " + distance);

					var fuc = xxx.would_fuck(pawn, target);
					fuc = fuc + (fuc * petness) - (fuc * wildness) + tamed;

					if ((xxx.config.zoophis_always_rape || fuc > best_fuckability) && distance < best_distance)
					{
						found = target;
						best_distance = distance;
						best_fuckability = fuc;
					}
				}
			}

			return found;
		}

		protected override Job TryGiveJob(Pawn p)
		{
			//--Log.Message("[RJW] JobGiver_Beastiality::TryGiveJob( " + p.NameStringShort + " ) called");
			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null))
			{

				if (xxx.is_healthy(p) && xxx.can_rape(p, true))
				{
					var target = find_target(p, p.Map);
					//--Log.Message("[RJW] JobGiver_Beastiality::TryGiveJob - target is " + (target == null ? "NULL" : target.NameStringShort));
					if (target != null)
					{
						return new Job(xxx.beastiality, target);
					}
					else
					{
						if (xxx.config.zoophis_always_rape)
							p.mindState.canLovinTick = Find.TickManager.TicksGame + 5;
						else
							p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(150, 300);

					}
				}
			}

			return null;
		}
	}
}
