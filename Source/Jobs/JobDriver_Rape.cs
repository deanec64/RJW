using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace rjw
{
	public class JobDriver_Rape : JobDriver
	{
		protected int duration;

		protected int ticks_between_hearts;

		protected int ticks_between_hits = 50;

		protected int ticks_between_thrusts;

		protected TargetIndex iTarget = TargetIndex.A;

		protected bool isAnalSex = false;

		//private List<Apparel> worn_apparel;// Edited by nizhuan-jjr: No Dropping Clothes on attackers!

		// Same as in JobDriver_Lovin
		protected static readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
		{
			new CurvePoint(1f,  12f),
			new CurvePoint(16f, 6f),
			new CurvePoint(22f, 9f),
			new CurvePoint(30f, 12f),
			new CurvePoint(50f, 18f),
			new CurvePoint(75f, 24f)
		};

		public Pawn Target
		{
			get
			{
				return (Pawn)(job.GetTarget(iTarget));
			}
		}

		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.Target, this.job, comfort_prisoners.max_rapists_per_prisoner, 0, null);
		}

		public static void roll_to_hit(Pawn rapist, Pawn p)
		{
			if (!Mod_Settings.prisoner_beating)
			{
				return;
			}

			float rand_value = Rand.Value;
			float victim_pain = p.health.hediffSet.PainTotal;
			// bloodlust makes the aggressor more likely to hit the prisoner
			float beating_chance = xxx.config.base_chance_to_hit_prisoner * (xxx.is_bloodlust(rapist) ? 1.25f : 1.0f);
			// psychopath makes the aggressor more likely to hit the prisoner past the significant_pain_threshold
			float beating_threshold = xxx.is_psychopath(rapist) ? xxx.config.extreme_pain_threshold : xxx.config.significant_pain_threshold;

			//--Log.Message("roll_to_hit:  rand = " + rand_value + ", beating_chance = " + beating_chance + ", victim_pain = " + victim_pain + ", beating_threshold = " + beating_threshold);
			if ((victim_pain < beating_threshold && rand_value < beating_chance) || (rand_value < (beating_chance / 2)))
			{
				//--Log.Message("   done told her twice already...");
				if (InteractionUtility.TryGetRandomVerbForSocialFight(rapist, out Verb v))
				{
					rapist.meleeVerbs.TryMeleeAttack(p, v);
				}
			}

			/*
            //if (p.health.hediffSet.PainTotal < xxx.config.significant_pain_threshold)
			if ((Rand.Value < 0.50f) &&
				((Rand.Value < 0.33f) || (p.health.hediffSet.PainTotal < xxx.config.significant_pain_threshold) ||
			     (xxx.is_bloodlust (rapist) || xxx.is_psychopath (rapist)))) {
				Verb v;
				if (InteractionUtility.TryGetRandomVerbForSocialFight (rapist, out v))
					rapist.meleeVerbs.TryMeleeAttack (p, v);
			}
            */
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			//--Log.Message("[RJW] JobDriver_RandomRape::MakeNewToils() called");
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
			this.FailOn(() => (!Target.health.capacities.CanBeAwake)); // || (!comfort_prisoners.is_designated (Prisoner)));
			this.FailOn(() => !pawn.CanReserve(Target, comfort_prisoners.max_rapists_per_prisoner, 0)); // Fail if someone else reserves the prisoner before the pawn arrives
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			var rape = new Toil();
			rape.initAction = delegate
			{
				//pawn.Reserve(Target, comfort_prisoners.max_rapists_per_prisoner, 0);
				//if (!pawnHasPenis)
				//	Target.rotationTracker.Face(pawn.DrawPos);
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
				rape.FailOn(() => (Target.CurJob == null) || (Target.CurJob.def != xxx.gettin_raped));
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
					aftersex(pawn, Target, true, isAnalSex: isAnalSex);
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(pawn);
					if (!Target.Dead)
					{
						Target.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(Target);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}

		public virtual void think_after_sex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false)
		{
			xxx.think_after_sex(pawn, part, violent, isCoreLovin);
		}
		public virtual void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex = false)
		{
			var pawn_name = (pawn != null) ? pawn.NameStringShort : "NULL";
			var part_name = (pawn != null) ? part.NameStringShort : "NULL";
			//--Log.Message("[RJW]" + this.GetType().ToString() + "::aftersex( " + pawn_name + ", " + part_name + " ) called");
			pawn.rotationTracker.Face(part.DrawPos);
			pawn.rotationTracker.FaceCell(part.Position);

			part.rotationTracker.Face(pawn.DrawPos);
			part.rotationTracker.FaceCell(pawn.Position);

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
	}
}
