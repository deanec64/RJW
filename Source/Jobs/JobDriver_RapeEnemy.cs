using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace rjw
{
	public class JobDriver_RapeEnemy : JobDriver
	{
		protected int duration;

		protected int ticks_between_hearts;

		protected int ticks_between_hits = 50;

		protected int ticks_between_thrusts;

		protected TargetIndex iTarget = TargetIndex.A;

		protected bool isAnalSex = false;

		protected bool requierCanRape = true;

		public virtual bool CanUseThisJobForPawn(Pawn rapist)
		{
			return xxx.is_human(rapist);
		}

		public virtual void think_after_sex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false)
		{
			xxx.think_after_sex(pawn, part, violent, isCoreLovin);
		}

		public virtual void roll_to_hit(Pawn rapist, Pawn p)
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

		protected readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
		{
			new CurvePoint(1f,  12f),
			new CurvePoint(16f, 6f),
			new CurvePoint(22f, 9f),
			new CurvePoint(30f, 12f),
			new CurvePoint(50f, 18f),
			new CurvePoint(75f, 24f)
		};

		public virtual void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex = false)
		{
			var pawn_name = (pawn != null) ? pawn.NameStringShort : "NULL";
			var part_name = (pawn != null) ? part.NameStringShort : "NULL";
			//--Log.Message("[RJW]" + this.GetType().ToString() + "::aftersex( " + pawn_name + ", " + part_name + " ) called");
			pawn.Drawer.rotator.Face(part.DrawPos);
			pawn.Drawer.rotator.FaceCell(part.Position);

			part.Drawer.rotator.Face(pawn.DrawPos);
			part.Drawer.rotator.FaceCell(pawn.Position);

			if (violent)
			{
				pawn.Drawer.Notify_MeleeAttackOn(part);
			}

			if (xxx.config.sounds_enabled)
			{
				SoundDef.Named("Cum").PlayOneShot(new TargetInfo(part.Position, pawn.Map, false));
			}

			bool pawnIsNotHuman = xxx.is_animal(pawn);
			bool partIsNotHuman = xxx.is_animal(part);
			//--Log.Message("[RJW]" + this.GetType().ToString() + "::aftersex( " + pawn_name + ", " + part_name + " ) - applying cum effect");
			if (xxx.config.cum_enabled)
			{
				int pawn_cum = pawnIsNotHuman ? 4 : Math.Min((int)(pawn.RaceProps.lifeExpectancy / pawn.ageTracker.AgeBiologicalYears), 2);
				int part_cum = partIsNotHuman ? 4 : Math.Min((int)(part.RaceProps.lifeExpectancy / part.ageTracker.AgeBiologicalYears), 2);
				if (pawn.gender == Gender.Female)
					pawn_cum /= 2;
				if (part.gender == Gender.Female)
					part_cum /= 2;
				FilthMaker.MakeFilth(pawn.PositionHeld, pawn.MapHeld, xxx.cum, pawn.LabelIndefinite(), pawn_cum);
				if (!isCoreLovin)
				{
					FilthMaker.MakeFilth(part.PositionHeld, part.MapHeld, xxx.cum, part.LabelIndefinite(), part_cum);
				}
			}

			//--Log.Message("[RJW]" + this.GetType().ToString() + "::aftersex( " + pawn_name + ", " + part_name + " ) - checking satisfaction");
			xxx.satisfy(pawn, part, violent, isCoreLovin);
			//--Log.Message("[RJW]" + this.GetType().ToString() + "::aftersex( " + pawn_name + ", " + part_name + " ) - checking thoughts");
			think_after_sex(pawn, part, violent, isCoreLovin);

			std.roll_to_catch(pawn, part);

			Impregnate(pawn, part, isAnalSex);
			//--Log.Message("[RJW]" + this.GetType().ToString() + "::aftersex( " + pawn_name + ", " + part_name + " ) - checking disease");
		}

		protected virtual void Impregnate(Pawn pawn, Pawn part, bool isAnalSex)
		{
			if (xxx.is_animal(pawn) || xxx.is_animal(part)) return;
			if (!isAnalSex)
			{
				//--Log.Message("[RJW]" + this.GetType().ToString() + "::aftersex( " + pawn.Name.ToStringShort + ", " + part.Name.ToStringShort + " ) - checking pregnancy");
				xxx.impregnate(pawn, part);
			}
		}

		protected Pawn Target
		{
			get
			{
				return (Pawn)(CurJob.GetTarget(iTarget));
			}
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			duration = (int)(2000.0f * Rand.Range(0.50f, 0.90f));
			ticks_between_hearts = Rand.RangeInclusive(70, 130);
			ticks_between_hits = Rand.Range(xxx.config.min_ticks_between_hits, xxx.config.max_ticks_between_hits);
			ticks_between_thrusts = 100;
			bool pawnHasPenis = Genital_Helper.has_penis(pawn);

			if (xxx.is_bloodlust(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.75);
			if (xxx.is_brawler(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.90);

			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => !pawn.CanReserve(Target, comfort_prisoners.max_rapists_per_prisoner, 0)); // Fail if someone else reserves the Target before the pawn arrives
			this.FailOn(() => !Target.Downed); //Stop rape when victim stand up again.
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			var rape = new Toil();
			rape.initAction = delegate
			{
				pawn.Reserve(Target, comfort_prisoners.max_rapists_per_prisoner, 0);
				if (!pawnHasPenis)
					Target.Drawer.rotator.Face(pawn.DrawPos);
				var dri = Target.jobs.curDriver as JobDriver_GettinRaped;
				if (dri == null)
				{
					var gettin_raped = new Job(xxx.gettin_raped);
					Target.jobs.StartJob(gettin_raped, JobCondition.InterruptForced, null, false, true, null);
					(Target.jobs.curDriver as JobDriver_GettinRaped).increase_time(duration);
				}
				else
				{
					dri.rapist_count += 1;
					dri.increase_time(duration);
				}
			};
			rape.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
				if (pawn.IsHashIntervalTick(ticks_between_thrusts))
					xxx.sexTick(pawn, Target);
				if (pawn.IsHashIntervalTick(ticks_between_hits))
					roll_to_hit(pawn, Target);
			};
			rape.AddFinishAction(delegate
			{
				//// Trying to add some interactions and social logs
				xxx.processAnalSex(pawn, Target, ref isAnalSex, pawnHasPenis);

				if ((Target.jobs != null) &&
					(Target.jobs.curDriver != null) &&
					(Target.jobs.curDriver as JobDriver_GettinRaped != null))
				{
					(Target.jobs.curDriver as JobDriver_GettinRaped).rapist_count -= 1;
				}
			});
			rape.defaultCompleteMode = ToilCompleteMode.Delay;
			rape.defaultDuration = duration;
			yield return rape;

			yield return new Toil
			{
				initAction = delegate
				{
					aftersex(pawn, Target, true, false, isAnalSex);
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(pawn);
					if (!Target.Dead)
					{
						Target.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(Target);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
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
				if (rapist.Faction == target.Faction || (!FactionUtility.HostileTo(rapist.Faction, target.Faction) && rapist.Faction != null)) continue;

				if (IntVec3Utility.ManhattanDistanceFlat(target.Position, rapist.Position) >= targetAcquireRadius) continue; //Too far to fuck i think.

				////--Log.Message("[ABF]"+this.GetType().ToString()+"::TryGiveJob( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - checking\nCanReserve:"+ rapist.CanReserve(target, comfort_prisoners.max_rapists_per_prisoner, 0) + "\nTargetPositionForbidden:"+ target.Position.IsForbidden(rapist)+"\nCanGetRape:" + xxx.can_get_raped(target));
				if (target != rapist && rapist.CanReserve(target, comfort_prisoners.max_rapists_per_prisoner, 0) && !target.Position.IsForbidden(rapist) && Can_rape_Easily(target))
				{
					if (xxx.is_human(target) || (xxx.is_zoophiliac(rapist) && xxx.is_animal(target) && xxx.config.animals_enabled))
					{
						var fuc = GetFuckability(rapist, target);
						//var fuc = xxx.would_fuck(rapist, target); //Cant Use default would fuck because victims are always bleeding.
						////--Log.Message("[ABF]"+this.GetType().ToString()+ "::FindVictim( " + rapist.NameStringShort + " -> " + target.NameStringShort + " ) - fuckability:" + fuc + " ");
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
			////--Log.Message("[RJW]"+this.GetType().ToString()+"::TryGiveJob( " + rapist.NameStringShort + " -> " + best_rapee.NameStringShort + " ) - fuckability:" + best_fuckability + " ");
			return best_rapee;
		}

		public virtual float GetFuckability(Pawn rapist, Pawn target)
		{
			////--Log.Message("[RJW]JobDriver_RapeEnemy::GetFuckability(" + rapist.ToString() + "," + target.ToString() + ")");
			return xxx.would_fuck(rapist, target, false, true);
		}

		protected bool Can_rape_Easily(Pawn p)
		{
			return xxx.can_get_raped(p) && p.Downed && !p.IsPrisonerOfColony;
		}
	}
}