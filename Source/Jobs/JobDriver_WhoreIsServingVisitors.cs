using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_WhoreIsServingVisitors : JobDriver
	{
		private TargetIndex PartnerInd = TargetIndex.A;
		private TargetIndex BedInd = TargetIndex.B;
		private TargetIndex SlotInd = TargetIndex.C;
		private int ticks_left = 200;
		private const int ticks_between_hearts = 100;
		private bool isAnalSex = false;

		public Pawn Actor
		{
			get
			{
				return GetActor();
			}
		}

		public Building_WhoreBed Bed
		{
			get
			{
				return (Building_WhoreBed)(job.GetTarget(BedInd));
			}
		}

		public Pawn Partner
		{
			get
			{
				return (Pawn)(job.GetTarget(PartnerInd));
			}
		}

		public IntVec3 WhoreSleepSpot
		{
			get
			{
				return (IntVec3)job.GetTarget(SlotInd);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticks_left, "ticksLeft", 0, false);
		}

		private static IntVec3 GetSleepingSpot(Building_WhoreBed bed)
		{
			for (int i = 0; i < bed.SleepingSlotsCount; i++)
			{
				if (bed.GetCurOccupant(i) == null)
				{
					return bed.GetSleepingSlotPos(i);
				}
			}
			return bed.GetSleepingSlotPos(0);
		}

		private static IntVec3 GetSleepingSpot(Building_WhoreBed bed, IntVec3 exceptPosition)
		{
			for (int i = 0; i < bed.SleepingSlotsCount; i++)
			{
				if (bed.GetCurOccupant(i) == null && bed.GetSleepingSlotPos(i) != exceptPosition)
				{
					return bed.GetSleepingSlotPos(i);
				}
			}
			return exceptPosition;
		}

		private static bool IsInOrByBed(Building_WhoreBed b, Pawn p)
		{
			for (int i = 0; i < b.SleepingSlotsCount; i++)
			{
				if (b.GetSleepingSlotPos(i).InHorDistOf(p.Position, 1f))
				{
					return true;
				}
			}
			return false;
		}
		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.Partner, this.job, 1, 0, null);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(PartnerInd);
			this.FailOnDespawnedNullOrForbidden(BedInd);
			this.FailOn(() => Actor is null || !xxx.CanUse(Actor, Bed) || !Actor.CanReserve(Partner));
			int price = Bed.priceOfWhore;
			yield return Toils_Reserve.Reserve(PartnerInd, 1, 0);
			//yield return Toils_Reserve.Reserve(BedInd, Bed.SleepingSlotsCount, 0);
			bool partnerHasPenis = Genital_Helper.has_penis(Partner);

			Toil gotoWhoreBed = new Toil
			{
				initAction = delegate
				 {
					 //--Log.Message("[RJW]JobDriver_WhoreIsServingVisitors::MakeNewToils() - gotoWhoreBed initAction is called");
					 Actor.pather.StartPath(WhoreSleepSpot, PathEndMode.OnCell);
					 //Actor.Reserve(Partner, 1, 0);
					 Partner.pather.StartPath(Actor, PathEndMode.Touch);
				 },
				tickAction = delegate
				{
					if (Partner.IsHashIntervalTick(150))
					{
						Partner.pather.StartPath(Actor, PathEndMode.Touch);
						//--Log.Message(Partner.NameStringShort + ": I'm following the whore");
					}
				},
				defaultCompleteMode = ToilCompleteMode.PatherArrival
			};
			gotoWhoreBed.FailOnWhorebedNoLongerUsable(BedInd, Bed);
			yield return gotoWhoreBed;

			Toil waitInBed = new Toil
			{
				initAction = delegate
				{
					//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - waitInBed, initAction is called");
					ticksLeftThisToil = 5000;
					ticks_left = (int)(2000.0f * Rand.Range(0.30f, 1.30f));
					//Actor.pather.StopDead();  //Let's just make whores standing at the bed
					//JobDriver curDriver = Actor.jobs.curDriver;
					//curDriver.layingDown = LayingDownState.LayingInBed;
					//curDriver.asleep = false;
					var gettin_loved = new Job(xxx.gettin_loved, Actor, Bed);
					Partner.jobs.StartJob(gettin_loved, JobCondition.InterruptForced, null, false, true, null);
				},
				tickAction = delegate
				{
					Actor.GainComfortFromCellIfPossible();
					if (IsInOrByBed(Bed, Partner))
					{
						//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - waitInBed, tickAction pass");
						ticksLeftThisToil = 0;
					}
				},
				defaultCompleteMode = ToilCompleteMode.Delay,
			};
			waitInBed.FailOn(() => pawn.GetRoom(RegionType.Set_Passable) == null);
			yield return waitInBed;

			bool canAfford = xxx.CanAfford(Partner, Actor, price);
			if (canAfford)
			{
				Toil loveToil = new Toil
				{
					initAction = delegate
					{
						//Actor.jobs.curDriver.ticksLeftThisToil = 1200;
						//Using ticks_left to control the time of sex
						//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - loveToil, setting initAction");
						if (xxx.HasNonPolyPartner(Actor))
						{
							Pawn pawn = LovePartnerRelationUtility.ExistingLovePartner(Actor);
							if (((Partner != pawn) && !pawn.Dead) && ((pawn.Map == Actor.Map) || (Rand.Value < 0.15)))
							{
								pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.CheatedOnMe, Actor);
							}
						}
						if (xxx.HasNonPolyPartner(Partner))
						{
							Pawn pawn = LovePartnerRelationUtility.ExistingLovePartner(Partner);
							if (((Actor != pawn) && !pawn.Dead) && ((pawn.Map == Partner.Map) || (Rand.Value < 0.25)))
							{
								pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.CheatedOnMe, Partner);
							}
						}
						if (!partnerHasPenis)
							Actor.rotationTracker.Face(Partner.DrawPos);
					},
					defaultCompleteMode = ToilCompleteMode.Never, //Changed from Delay
				};
				loveToil.AddPreTickAction(delegate
				{
					//Actor.Reserve(Partner, 1, 0);
					--ticks_left;
					if (ticks_left <= 0)
						ReadyForNextToil();
					else if (pawn.IsHashIntervalTick(ticks_between_hearts))
					{
						MoteMaker.ThrowMetaIcon(Actor.Position, Actor.Map, ThingDefOf.Mote_Heart);
					}
					Actor.GainComfortFromCellIfPossible();
					Partner.GainComfortFromCellIfPossible();
				});
				loveToil.AddFinishAction(delegate
				{
					//--Log.Message("[RJW] JobDriver_WhoreIsServingVisitors::MakeNewToils() - finished loveToil");
					//// Trying to add some interactions and social logs
					//xxx.processAnalSex(Partner, Actor, ref isAnalSex, partnerHasPenis);
				});
				loveToil.AddFailCondition(() => Partner.Dead || !IsInOrByBed(Bed, Partner));
				loveToil.socialMode = RandomSocialMode.Off;
				yield return loveToil;

				Toil afterSex = new Toil
				{
					initAction = delegate
					{
						//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - calling aftersex in afterSex.initAction");
						xxx.aftersex(Partner, Actor, false, false, isAnalSex);
						//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - setting mindstate in second initAction");
						Actor.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(Actor);
						//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - setting mindstate again in second initAction");
						Partner.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(Partner);

						//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - Partner should pay the price now in afterSex.initAction");
						int remainPrice = xxx.PayPriceToWhore(Partner, price, Actor);
						if (remainPrice <= 0)
						{
							//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - Paying price is success");
						}
						else
						{
							//--Log.Message("JobDriver_WhoreIsServingVisitors::MakeNewToils() - Paying price failed");
						}
						xxx.UpdateRecords(Actor, price-remainPrice);
					},
					defaultCompleteMode = ToilCompleteMode.Instant
				};
				yield return afterSex;
			}
		}
	}
}