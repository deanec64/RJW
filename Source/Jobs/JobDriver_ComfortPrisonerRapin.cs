using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_ComfortPrisonerRapin : JobDriver
	{
		private int duration;

		private int ticks_between_hearts;

		private int ticks_between_hits = 50;

		private int ticks_between_thrusts;

		protected TargetIndex iprisoner = TargetIndex.A;

		private bool isAnalSex = false;

		//private List<Apparel> worn_apparel;// Edited by nizhuan-jjr:No Dropping Clothes on Attackers!

		// Same as in JobDriver_Lovin
		private static readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
		{
			new CurvePoint(1f,  12f),
			new CurvePoint(16f, 6f),
			new CurvePoint(22f, 9f),
			new CurvePoint(30f, 12f),
			new CurvePoint(50f, 18f),
			new CurvePoint(75f, 24f)
		};

		protected Pawn Prisoner
		{
			get
			{
				return (Pawn)(job.GetTarget(iprisoner));
			}
		}

		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.Prisoner, this.job, comfort_prisoners.max_rapists_per_prisoner, 0, null);
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
			duration = (int)(2000.0f * Rand.Range(0.50f, 0.90f));
			ticks_between_hearts = Rand.RangeInclusive(70, 130);
			ticks_between_hits = Rand.Range(xxx.config.min_ticks_between_hits, xxx.config.max_ticks_between_hits);
			ticks_between_thrusts = 100;
			bool pawnHasPenis = Genital_Helper.has_penis(pawn);

			if (xxx.is_bloodlust(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.75);
			if (xxx.is_brawler(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.90);

			//--Log.Message("JobDriver_ComfortPrisonerRapin::MakeNewToils() - setting fail conditions");
			this.FailOnDespawnedNullOrForbidden(iprisoner);
			this.FailOn(() => (!Prisoner.health.capacities.CanBeAwake) || (!comfort_prisoners.is_designated(Prisoner)));
			this.FailOn(() => !pawn.CanReserve(Prisoner, comfort_prisoners.max_rapists_per_prisoner, 0)); // Fail if someone else reserves the prisoner before the pawn arrives
			yield return Toils_Goto.GotoThing(iprisoner, PathEndMode.OnCell);
			Messages.Message(pawn.NameStringShort + " is trying to rape " + Prisoner.NameStringShort + ".", pawn, MessageTypeDefOf.NeutralEvent);

			var rape = new Toil();
			rape.initAction = delegate
			{
				//--Log.Message("JobDriver_ComfortPrisonerRapin::MakeNewToils() - reserving prisoner");
				//pawn.Reserve(Prisoner, comfort_prisoners.max_rapists_per_prisoner, 0);
				if (!pawnHasPenis)
					Prisoner.rotationTracker.Face(pawn.DrawPos);

				//--Log.Message("JobDriver_ComfortPrisonerRapin::MakeNewToils() - Setting victim job driver");
				var dri = Prisoner.jobs.curDriver as JobDriver_GettinRaped;
				if (dri == null)
				{
					var gettin_raped = new Job(xxx.gettin_raped);
					Prisoner.jobs.StartJob(gettin_raped, JobCondition.InterruptForced, null, false, true, null);
					(Prisoner.jobs.curDriver as JobDriver_GettinRaped).increase_time(duration);
				}
				else
				{
					dri.rapist_count += 1;
					dri.increase_time(duration);
				}

				//--Log.Message("JobDriver_ComfortPrisonerRapin::MakeNewToils() - Removing victim's clothing");
				// Try to take off the attacker's clothing and add to inventory
				/* Edited by nizhuan-jjr: No Dropping clothes on attackers!
                        if (pawn.apparel != null) {
                            worn_apparel = pawn.apparel.WornApparel.ListFullCopy<Apparel>();
                            while (pawn.apparel != null && pawn.apparel.WornApparelCount > 0) {
                                Apparel apparel = pawn.apparel.WornApparel.RandomElement<Apparel>();
                                pawn.apparel.Remove(apparel);
                                if (pawn.inventory != null && pawn.inventory.innerContainer != null) {
                                    pawn.inventory.innerContainer.TryAdd(apparel);
                                }
                            }
                        }
                */
			};
			rape.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
				if (pawn.IsHashIntervalTick(ticks_between_thrusts))
					xxx.sexTick(pawn, Prisoner);
				if (pawn.IsHashIntervalTick(ticks_between_hits))
					roll_to_hit(pawn, Prisoner);
			};
			rape.AddFinishAction(delegate
			{
				//// Trying to add some interactions and social logs
				xxx.processAnalSex(pawn, Prisoner, ref isAnalSex, pawnHasPenis);
				//--Log.Message("JobDriver_ComfortPrisonerRapin::MakeNewToils() - Clearing victim job");
				if ((Prisoner.jobs != null) &&
				(Prisoner.jobs.curDriver != null) &&
				(Prisoner.jobs.curDriver as JobDriver_GettinRaped != null))
				{
					(Prisoner.jobs.curDriver as JobDriver_GettinRaped).rapist_count -= 1;
					xxx.processBrokenBody(Prisoner);
					xxx.ExtraSatisfyForBrokenCP(Prisoner);
				}
			});
			rape.defaultCompleteMode = ToilCompleteMode.Delay;
			rape.defaultDuration = duration;
			yield return rape;

			yield return new Toil
			{
				initAction = delegate
				{
					//--Log.Message("JobDriver_ComfortPrisonerRapin::MakeNewToils() - Calling aftersex");
					xxx.aftersex(pawn, Prisoner, true, isAnalSex: isAnalSex);
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(pawn);
					if (!Prisoner.Dead)
					{
						//xxx.aftersex (Prisoner, pawn, true);
						Prisoner.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(Prisoner);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}