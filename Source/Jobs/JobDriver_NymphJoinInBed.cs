using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_NymphJoinInBed : JobDriver
	{
		private const int ticks_between_hearts = 100;

		private int ticks_left;

		private TargetIndex ipartner = TargetIndex.A;

		private TargetIndex ibed = TargetIndex.B;

		protected Pawn Partner
		{
			get
			{
				return (Pawn)(job.GetTarget(ipartner));
			}
		}

		protected Building_Bed Bed
		{
			get
			{
				return (Building_Bed)(job.GetTarget(ibed));
			}
		}
		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.Partner, this.job, 1, -1, null) && this.pawn.Reserve(this.Bed, this.job, 1, -1, null);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			//--Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() called");
			this.FailOnDespawnedOrNull(ipartner);
			this.FailOnDespawnedOrNull(ibed);
			this.FailOn(() => !Partner.health.capacities.CanBeAwake);
			this.FailOn(() => !xxx.is_laying_down_alone(Partner));
			yield return Toils_Reserve.Reserve(ipartner, comfort_prisoners.max_rapists_per_prisoner, 0);
			yield return Toils_Goto.GotoThing(ipartner, PathEndMode.OnCell);
			yield return new Toil
			{
				initAction = delegate
				{
					//--Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - setting initAction");
					ticks_left = (int)(2500.0f * Rand.Range(0.30f, 1.30f));
					var gettin_loved = new Job(xxx.gettin_loved, pawn, Bed);
					Partner.jobs.StartJob(gettin_loved, JobCondition.InterruptForced, null, false, true, null);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			var do_lovin = new Toil();
			do_lovin.defaultCompleteMode = ToilCompleteMode.Never;
			do_lovin.FailOn(() => (Partner.CurJob == null) || (Partner.CurJob.def != xxx.gettin_loved));
			do_lovin.AddPreTickAction(delegate
			{
				--ticks_left;
				if (ticks_left <= 0)
					ReadyForNextToil();
				else if (pawn.IsHashIntervalTick(ticks_between_hearts))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
			});
			do_lovin.socialMode = RandomSocialMode.Off;
			yield return do_lovin;
			yield return new Toil
			{
				initAction = delegate
				{
					//--Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - setting pawn.got_some_lovin memory in second initAction");
					var sex_mem = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.GotSomeLovin);
					var pawn_memories = pawn.needs.mood.thoughts.memories as MemoryThoughtHandler;
					if (pawn_memories != null)
					{
						pawn.needs.mood.thoughts.memories.TryGainMemory(sex_mem, Partner);
					}

					//--Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - setting Partner.got_some_lovin memory in second initAction");
					var sex_mem2 = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.GotSomeLovin); // Is this neccessary?
					if (Partner.needs != null && Partner.needs.mood != null && Partner.needs.mood.thoughts != null)
					{
						Partner.needs.mood.thoughts.memories.TryGainMemory(sex_mem2, pawn);
					}

					//--Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - calling aftersex in second initAction");
					xxx.aftersex(pawn, Partner);
					//--Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - calling aftersex again in second initAction");
					//xxx.aftersex (Partner, pawn);
					//--Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - setting mindstate in second initAction");
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(pawn);
					//--Log.Message("JobDriver_NymphJoinInBed::MakeNewToils() - setting mindstate again in second initAction");
					Partner.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(Partner);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}