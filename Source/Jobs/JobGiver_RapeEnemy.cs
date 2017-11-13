using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobGiver_RapeEnemy : ThinkNode_JobGiver
	{
		private float targetAcquireRadius = 20f;

		protected override Job TryGiveJob(Pawn p)
		{
			//Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + p.NameStringShort + " ) called");

			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null))
			{
				if (p.health.capacities.CanBeAwake && xxx.can_rape(p))
				{
					var downedPlayer = find_victim(p, p.Map);

					if (downedPlayer != null)
					{
						//Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + p.NameStringShort + " ) - found victim " + downedPlayer.NameStringShort);
						p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
						if (ABFCommon.is_Insect(p))
						{
							return new Job(DefDatabase<JobDef>.GetNamed("RapeEnemyByInsect"), downedPlayer);
						}
						if (p.RaceProps.IsMechanoid)
						{
							return new Job(DefDatabase<JobDef>.GetNamed("RapeEnemyByMech"), downedPlayer);
						}
						return new Job(DefDatabase<JobDef>.GetNamed("RapeEnemy"), downedPlayer);
					}
					else
					{
						//Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + p.NameStringShort + " ) - unable to find victim");
						p.mindState.canLovinTick = Find.TickManager.TicksGame + 1;
					}
				}
				//else { Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + p.NameStringShort + " ) - can not rape"); }
			}
			//else {  Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + p.NameStringShort + " ) - too fast to play next"); }

			return null;
		}

		public Pawn find_victim(Pawn rapist, Map m)
		{
			Pawn best_rapee = null;
			var best_fuckability = 0.20f; // Don't rape prisoners with <20% fuckability
			foreach (var target in m.mapPawns.AllPawns)
			{
				//if (target.Faction != Faction.OfPlayer) continue;
				if (rapist.Faction == target.Faction | !FactionUtility.HostileTo(rapist.Faction, target.Faction)) continue;

				if (SqrDistance(target.Position, rapist.Position) >= targetAcquireRadius * targetAcquireRadius) continue; //Too far to fuck i think.

				//Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - checking\nCanReserve:"+ rapist.CanReserve(target, comfort_prisoners.max_rapists_per_prisoner, 0) + "\nTargetPositionForbidden:"+ target.Position.IsForbidden(rapist)+"\nHelthEnough:"+ xxx.is_healthy_enough(target)+"\nCanGetRape:" + xxx.can_get_raped(target));
				if (target != rapist && rapist.CanReserve(target, comfort_prisoners.max_rapists_per_prisoner, 0) && !target.Position.IsForbidden(rapist) && is_easy_to_rape(target) && xxx.can_get_raped(target))
				{
					if (xxx.is_human(target) || (xxx.is_zoophiliac(rapist) && xxx.is_animal(target) && xxx.config.animals_enabled))
					{
						var fuc = ABFCommon.would_fuck_ignoreSatisfy(rapist, target, true);
						//var fuc = xxx.would_fuck(rapist, target, true);
						//Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - fuckability:" + fuc + " ");
						if ((fuc > best_fuckability) && (Rand.Value < 0.9 * fuc))
						{
							best_rapee = target;
							best_fuckability = fuc;
						}
						//else { Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - is not good for me "+ "( " + fuc + " )"); }
					}
					//else { Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - is not human or not zoophilia"); }
				}
				//else { Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - is not good"); }
			}
			return best_rapee;
		}

		public bool is_easy_to_rape(Pawn p)
		{
			return xxx.can_get_raped(p) && p.Downed;
		}

		public double Distance(IntVec3 a, IntVec3 b)
		{
			return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
		}

		public double SqrDistance(IntVec3 a, IntVec3 b)
		{
			return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
		}
	}
}