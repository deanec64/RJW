using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace rjw
{
	public class JobDriver_RapeEnemy : JobDriver_Rape
	{

		protected bool requierCanRape = true;

		public virtual bool CanUseThisJobForPawn(Pawn rapist)
		{
			return xxx.is_human(rapist);
		}

		// Should move these function to common
		public static bool PlantSomething(HediffDef def, Pawn target, bool isToAnal = false, int amount = 1)
		{
			if (def == null) return false;
			if (!isToAnal && !Genital_Helper.has_vagina(target)) return false;
			if (isToAnal && !Genital_Helper.has_anus(target)) return false;
			BodyPartRecord genitalPart = (isToAnal) ? Genital_Helper.get_anus(target) : Genital_Helper.get_genitals(target);
			if (genitalPart != null || genitalPart.parts.Count != 0)
			{
				for (int i = 0; i < amount; i++)
				{
					target.health.AddHediff(def, genitalPart);
				}
				return true;
			}

			return false;
		}

		public virtual Pawn FindVictim(Pawn rapist, Map m, float targetAcquireRadius)
		{
			if (rapist == null || m == null) return null;
			if (!xxx.can_rape(rapist) && requierCanRape) return null;
			Pawn best_rapee = null;
			var best_fuckability = 0.20f; // Don't rape prisoners with <20% fuckability
			foreach (var target in m.mapPawns.AllPawns)
			{
				//if (target.Faction != Faction.OfPlayer) continue;
				//if (rapist.Faction == target.Faction || (!FactionUtility.HostileTo(rapist.Faction, target.Faction) && rapist.Faction != null)) continue;
				if(!rapist.HostileTo(target)) continue;

				if (IntVec3Utility.ManhattanDistanceFlat(target.Position, rapist.Position) >= targetAcquireRadius) continue; //Too far to fuck i think.

				//--Log.Message("[ABF]"+this.GetType().ToString()+"::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - checking\nCanReserve:"+ rapist.CanReserve(target, comfort_prisoners.max_rapists_per_prisoner, 0) + "\nTargetPositionForbidden:"+ target.Position.IsForbidden(rapist)+"\nCanGetRape:" + xxx.can_get_raped(target));
				if (target != rapist && rapist.CanReserve(target, comfort_prisoners.max_rapists_per_prisoner, 0) && !target.Position.IsForbidden(rapist) && Can_rape_Easily(target))
				{
					if (xxx.is_human(target) || (xxx.is_zoophiliac(rapist) && xxx.is_animal(target) && xxx.config.animals_enabled))
					{
						var fuc = GetFuckability(rapist, target);
						//var fuc = xxx.would_fuck(rapist, target); //Cant Use default would fuck because victims are always bleeding.
						//--Log.Message("[ABF]"+this.GetType().ToString()+ "::FindVictim( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - fuckability:" + fuc + " ");
						if ((fuc > best_fuckability) && (Rand.Value < 0.9 * fuc))
						{
							best_rapee = target;
							best_fuckability = fuc;
						}
						//else { //--Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - is not good for me "+ "( " + fuc + " )"); }
					}
					//else { //--Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - is not human or not zoophilia"); }
				}
				//else { //--Log.Message("[ABF] JobGiver_RapeEnemy::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - is not good"); }
			}
			//--Log.Message("[RJW]"+this.GetType().ToString()+"::TryGiveJob( " + rapist.NameStringShort + " -> " + best_rapee.NameStringShort + " ) - fuckability:" + best_fuckability + " ");
			return best_rapee;
		}

		public virtual float GetFuckability(Pawn rapist, Pawn target)
		{
			//--Log.Message("[RJW]JobDriver_RapeEnemy::GetFuckability(" + rapist.ToString() + "," + target.ToString() + ")");
			return xxx.would_fuck(rapist, target, true, true);
		}

		protected bool Can_rape_Easily(Pawn p)
		{
			return xxx.can_get_raped(p) && p.Downed && !p.IsPrisonerOfColony;
		}
	}
}