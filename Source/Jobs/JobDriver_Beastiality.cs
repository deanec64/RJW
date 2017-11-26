using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_Beastiality : JobDriver
	{
		private int duration;

		private int ticks_between_hearts;

		private int ticks_between_hits = 50;

		private int ticks_between_thrusts;

		protected TargetIndex iprisoner = TargetIndex.A;

		private bool isAnalSex = false;

		//private List<Apparel> worn_apparel;  // Edited by nizhuan-jjr: No Dropping Clothes on attackers!

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

		protected Pawn animal
		{
			get
			{
				return (Pawn)(job.GetTarget(iprisoner));
			}
		}

		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.animal, this.job, 1, 0, null);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			//--Log.Message("[RJW] JobDriver_Beastiality::MakeNewToils() called");
			duration = (int)(2500.0f * Rand.Range(0.50f, 0.90f));
			ticks_between_hearts = Rand.RangeInclusive(70, 130);
			ticks_between_hits = Rand.Range(xxx.config.min_ticks_between_hits, xxx.config.max_ticks_between_hits);
			ticks_between_thrusts = 100;
			bool pawnHasPenis = Genital_Helper.has_penis(pawn);

			if (xxx.is_bloodlust(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.75);
			if (xxx.is_brawler(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.90);

			//this.FailOnDespawnedNullOrForbidden (iprisoner);
			//this.FailOn (() => (!Prisoner.health.capacities.CanBeAwake) || (!comfort_prisoners.is_designated (Prisoner)));
			this.FailOn(() => !pawn.CanReserve(animal, 1, 0));  // Fail if someone else reserves the prisoner before the pawn arrives
			yield return Toils_Reserve.Reserve(iprisoner, 1, 0);
			//--Log.Message("[RJW] JobDriver_Beastiality::MakeNewToils() - moving towards animal");
			yield return Toils_Goto.GotoThing(iprisoner, PathEndMode.OnCell);
			Messages.Message(pawn.NameStringShort + " is trying to rape " + animal.NameStringShort + ".", pawn, MessageTypeDefOf.NeutralEvent);

			var rape = new Toil();
			rape.initAction = delegate
			{
				//--Log.Message("[RJW] JobDriver_Beastiality::MakeNewToils() - reserving animal");
				//pawn.Reserve(animal, 1, 0); // animal rapin seems like a solitary activity
				//if (!pawnHasPenis)
				//	animal.rotationTracker.Face(pawn.DrawPos);

				//--Log.Message("[RJW] JobDriver_Beastiality::MakeNewToils() - Setting animal job driver");
				var dri = animal.jobs.curDriver as JobDriver_GettinRaped;
				if (dri == null)
				{
					var gettin_raped = new Job(xxx.gettin_raped);
					animal.jobs.StartJob(gettin_raped, JobCondition.InterruptForced, null, true, true, null);
					(animal.jobs.curDriver as JobDriver_GettinRaped).increase_time(duration);
				}
				else
				{
					dri.rapist_count += 1;
					dri.increase_time(duration);
				}
				// Try to take off the attacker's clothing
				//--Log.Message("[RJW] JobDriver_Beastiality::MakeNewToils() - stripping necro lover");
				/* Edited by nizhuan-jjr: No Dropping Clothes on attackes!
                        worn_apparel = pawn.apparel.WornApparel.ListFullCopy<Apparel>();
                        while (pawn.apparel != null && pawn.apparel.WornApparelCount > 0) {
                            Apparel apparel = pawn.apparel.WornApparel.RandomElement<Apparel>();
                            pawn.apparel.Remove(apparel);
                        }
                */
			};
			rape.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
				if (pawn.IsHashIntervalTick(ticks_between_thrusts))
					xxx.sexTick(pawn, animal);
				/*
				if (pawn.IsHashIntervalTick (ticks_between_hits))
					roll_to_hit (pawn, animal);
                    */
			};
			rape.AddFinishAction(delegate
			{
				//--Log.Message("[RJW] JobDriver_Beastiality::MakeNewToils() - finished violating");
				//// Trying to add some interactions and social logs
				xxx.processAnalSex(pawn, animal, ref isAnalSex, pawnHasPenis);

				if ((animal.jobs != null) &&
					(animal.jobs.curDriver != null) &&
					(animal.jobs.curDriver as JobDriver_GettinRaped != null))
					(animal.jobs.curDriver as JobDriver_GettinRaped).rapist_count -= 1;
			});
			rape.defaultCompleteMode = ToilCompleteMode.Delay;
			rape.defaultDuration = duration;
			yield return rape;

			yield return new Toil
			{
				initAction = delegate
				{
					//--Log.Message("[RJW] JobDriver_Beastiality::MakeNewToils() - creating aftersex toil");
					xxx.aftersex(pawn, animal, false, false, isAnalSex);
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(pawn);
					if (!animal.Dead)
					{
						animal.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(animal);
					}

					//--Log.Message("[RJW] JobDriver_Beastiality::MakeNewToils() - putting clothes back on");
					/* Edited by nizhuan-jjr: No Dropping Clothes on attackers!
                            if (pawn.apparel != null) {
                                foreach (Apparel apparel in worn_apparel) {
                                    pawn.apparel.Wear(apparel);//  WornApparel.Add(apparel);
                                }
                            }
                    */
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}