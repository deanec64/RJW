
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	public class JobDriver_NymphJoinInBed : JobDriver
	{

		private const int ticks_between_hearts = 100;

		private const int ticks_between_thrusts = 150;

		private int ticks_left;

		private TargetIndex ipartner = TargetIndex.A;

		private TargetIndex ibed = TargetIndex.B;

		private bool isAnalSex = false;

		protected Pawn Partner
		{
			get
			{
				return (Pawn)(CurJob.GetTarget(ipartner));
			}
		}

		protected Building_Bed Bed
		{
			get
			{
				return (Building_Bed)(CurJob.GetTarget(ibed));
			}
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() called");
			this.FailOnDespawnedOrNull(ipartner);
			this.FailOnDespawnedOrNull(ibed);
			this.FailOn(() => !Partner.health.capacities.CanBeAwake);
			this.FailOn(() => !xxx.is_laying_down_alone(Partner));
			yield return Toils_Reserve.Reserve(ipartner, comfort_prisoners.max_rapists_per_prisoner, 0);
			yield return Toils_Goto.GotoThing(ipartner, PathEndMode.OnCell);
			bool pawnHasPenis = Genital_Helper.has_penis(pawn);

			Toil do_lovin = new Toil();
			do_lovin.defaultCompleteMode = ToilCompleteMode.Never;
			do_lovin.FailOn(() => (Partner.CurJob == null) || (Partner.CurJob.def != xxx.gettin_loved));

			do_lovin.initAction = delegate
			{
				Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - do_lovin.initAction is called");
				ticks_left = (int)(2000.0f * Rand.Range(0.30f, 1.30f));
				if (!pawnHasPenis)
					Partner.Drawer.rotator.Face(pawn.DrawPos);

				var gettin_loved = new Job(xxx.gettin_loved, pawn, Bed);
				Partner.jobs.StartJob(gettin_loved, JobCondition.InterruptForced, null, false, true, null);
			};
			do_lovin.AddPreTickAction(delegate
			{
				--ticks_left;
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
				if (pawn.IsHashIntervalTick(ticks_between_thrusts))   // Should test whether this will fail the job
					xxx.sexTick(pawn, Partner);
				if (ticks_left <= 0)
					ReadyForNextToil();
			});
			do_lovin.AddFinishAction(delegate
			{
				//// Trying to add some interactions and social logs
				xxx.processAnalSex(pawn, Partner, ref isAnalSex, pawnHasPenis);
			});
			do_lovin.socialMode = RandomSocialMode.Off;
			yield return do_lovin;

			Toil afterLovin = new Toil
			{
				initAction = delegate
				{
					Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - afterLovin.initAction is called");

					//Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - calling aftersex in second initAction");
					xxx.aftersex(pawn, Partner, false, false, isAnalSex);
					//Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - setting mindstate in second initAction");
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(pawn);
					//Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - setting mindstate again in second initAction");
					Partner.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(Partner);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield return afterLovin;
		}

	}
}
