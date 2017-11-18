using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_InvitingVisitors : JobDriver
	{
		public bool successfulPass = true;

		private Building_WhoreBed TargetBed { get => (TargetThingB as Building_WhoreBed); }

		private TargetIndex TargetBedIndex = TargetIndex.B;

		private Pawn TargetPawn
		{
			get => (TargetThingA as Pawn);
		}

		private TargetIndex TargetPawnIndex = TargetIndex.A;

		private Pawn Whore
		{
			get => GetActor();
		}

		private bool DoesTargetPawnAcceptAdvance()
		{
			if (xxx.config.always_accept_whores)
				return true;
			if (PawnUtility.WillSoonHaveBasicNeed(TargetPawn))
			{
				return false;
			}
			if (PawnUtility.EnemiesAreNearby(TargetPawn, 9, false))
			{
				return false;
			}
			if (TargetPawn.jobs.curJob == null || (TargetPawn.jobs.curJob.def == JobDefOf.WaitWander || TargetPawn.jobs.curJob.def == JobDefOf.GotoWander || TargetPawn.jobs.curJob.def.joyKind != null))
			{
				//--Log.Message("[RJW]JobDriver_InvitingVisitors::DoesTargetPawnAcceptAdvance() is called");
				return (xxx.WillPawnTryHookup(TargetPawn) && xxx.IsHookupAppealing(TargetPawn, Whore) && xxx.CanAfford(TargetPawn, Whore, TargetBed.priceOfWhore));
			}
			else return false;
		}

		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.TargetPawn, this.job, 1, -1, null) && this.pawn.Reserve(this.Whore, this.job, 1, -1, null);
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetPawnIndex);
			this.FailOnDespawnedNullOrForbidden(TargetBedIndex);
			this.FailOn(() => Whore is null || !xxx.CanUse(Whore, TargetBed));//|| !Whore.CanReserve(TargetPawn)
			yield return Toils_Goto.GotoThing(TargetPawnIndex, PathEndMode.Touch);

			Toil TryItOn = new Toil();
			TryItOn.AddFailCondition(() => !xxx.IsTargetPawnOkay(TargetPawn));
			TryItOn.defaultCompleteMode = ToilCompleteMode.Delay;
			TryItOn.initAction = delegate
			{
				//--Log.Message("[RJW]JobDriver_InvitingVisitors::MakeNewToils - TryItOn - initAction is called");
				Whore.jobs.curDriver.ticksLeftThisToil = 50;
				MoteMaker.ThrowMetaIcon(Whore.Position, Whore.Map, ThingDefOf.Mote_Heart);
			};
			yield return TryItOn;

			Toil AwaitResponse = new Toil();
			AwaitResponse.AddFailCondition(() => !successfulPass);
			AwaitResponse.defaultCompleteMode = ToilCompleteMode.Instant;
			AwaitResponse.initAction = delegate
			{
				List<RulePackDef> extraSentencePacks = new List<RulePackDef>();
				successfulPass = DoesTargetPawnAcceptAdvance();
				//--Log.Message("[RJW]JobDriver_InvitingVisitors::MakeNewToils - AwaitResponse - initAction is called");
				if (successfulPass)
				{
					MoteMaker.ThrowMetaIcon(TargetPawn.Position, TargetPawn.Map, ThingDefOf.Mote_Heart);
					if (xxx.RomanceDiversifiedIsActive)
					{
						extraSentencePacks.Add(RulePackDef.Named("HookupSucceeded"));
					}
					Messages.Message("RJW_VisitorAcceptWhore".Translate(new object[] { TargetPawn.NameStringShort, Whore.NameStringShort }), TargetPawn, MessageTypeDefOf.TaskCompletion);
				}
				else
				{
					MoteMaker.ThrowMetaIcon(TargetPawn.Position, TargetPawn.Map, ThingDefOf.Mote_IncapIcon);
					if (xxx.RomanceDiversifiedIsActive)
					{
						Whore.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("RebuffedMyHookupAttempt"), TargetPawn);
						TargetPawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("FailedHookupAttemptOnMe"), Whore);
						extraSentencePacks.Add(RulePackDef.Named("HookupFailed"));
					}
					//Disabled rejection notifications
					//Messages.Message("RJW_VisitorRejectWhore".Translate(new object[] { TargetPawn.NameStringShort, Whore.NameStringShort }), TargetPawn, MessageTypeDefOf.SilentInput);
				}
				if (xxx.RomanceDiversifiedIsActive)
				{
					Find.PlayLog.Add(new PlayLogEntry_Interaction(DefDatabase<InteractionDef>.GetNamed("TriedHookupWith"), pawn, TargetPawn, extraSentencePacks));
				}
			};
			yield return AwaitResponse;

			Toil BothGoToBed = new Toil();
			BothGoToBed.AddFailCondition(() => !successfulPass || !xxx.CanUse(Whore, TargetBed));
			BothGoToBed.defaultCompleteMode = ToilCompleteMode.Instant;
			BothGoToBed.initAction = delegate
			{
				//--Log.Message("[RJW]JobDriver_InvitingVisitors::MakeNewToils - BothGoToBed - initAction is called0");
				if (successfulPass)
				{
					if (!xxx.CanUse(Whore, TargetBed))
					{
						//--Log.Message("[RJW]JobDriver_InvitingVisitors::MakeNewToils - BothGoToBed - cannot use the whore bed");
						return;
					}
					//--Log.Message("[RJW]JobDriver_InvitingVisitors::MakeNewToils - BothGoToBed - initAction is called1");
					JobTag? tag = null;
					Whore.jobs.jobQueue.EnqueueFirst(new Job(xxx.whore_is_serving_visitors, TargetPawn, TargetBed, TargetBed.SleepPosOfAssignedPawn(Whore)), tag);
					//TargetPawn.jobs.jobQueue.EnqueueFirst(new Job(DefDatabase<JobDef>.GetNamed("WhoreIsServingVisitors"), Whore, TargetBed, (TargetBed.MaxAssignedPawnsCount>1)?TargetBed.GetSleepingSlotPos(1): TargetBed.)), null);
					Whore.jobs.curDriver.EndJobWith(JobCondition.InterruptOptional);
					//TargetPawn.jobs.curDriver.EndJobWith(JobCondition.InterruptOptional);
				}
			};
			yield return BothGoToBed;
		}
	}
}