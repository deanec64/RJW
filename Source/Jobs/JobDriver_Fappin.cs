﻿
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	public class JobDriver_Fappin : JobDriver
	{

		private const int ticks_between_hearts = 100;

		private int ticks_left;

		private static readonly SimpleCurve fap_interval_from_age = new SimpleCurve {
			new CurvePoint (16f, 0.75f),
			new CurvePoint (22f, 0.75f),
			new CurvePoint (30f, 1.50f),
			new CurvePoint (50f, 3.00f),
			new CurvePoint (75f, 5.00f)
		};

		private const TargetIndex BedOrRestSpotIndex = TargetIndex.A;

		private Building_Bed Bed
		{
			get
			{
				return (Building_Bed)base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			ticks_left = (int)(2000.0f * Rand.Range(0.20f, 0.70f));

			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.KeepLyingDown(TargetIndex.A);

			bool hasBed = this.pawn.CurJob.GetTarget(TargetIndex.A).HasThing;
			if (hasBed)
			{
				yield return Toils_Reserve.Reserve(TargetIndex.A, this.Bed.SleepingSlotsCount, 0, null);
				yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.A, TargetIndex.None);
				yield return Toils_Bed.GotoBed(TargetIndex.A);
			}
			else
			{
				yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
			}

			Toil do_fappin = Toils_LayDown.LayDown(TargetIndex.A, hasBed, false, true, false);

			do_fappin.initAction = delegate
			{
				Log.Message("[RJW]JobDriver_Fappin::MakeNewToils - do_fappin.initAction is called");
			};


			do_fappin.AddPreTickAction(delegate
			{
				--this.ticks_left;
				if (this.ticks_left <= 0)
					this.ReadyForNextToil();
				else if (pawn.IsHashIntervalTick(ticks_between_hearts))
					MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
			});

			do_fappin.AddFinishAction(delegate
			{
				//Thought_Memory newThought = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.Fapped);
				pawn.mindState.canLovinTick = Find.TickManager.TicksGame + generate_min_ticks_to_next_fappin(pawn);
				xxx.satisfy(pawn, null);
			});
			
			do_fappin.socialMode = RandomSocialMode.Off;
			yield return do_fappin;
		}

		private int generate_min_ticks_to_next_fappin(Pawn p)
		{
			if (!DebugSettings.alwaysDoLovin)
			{
				var interval = fap_interval_from_age.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
				var rinterval = Math.Max(0.5f, Rand.Gaussian(interval, 0.3f));
				return (int)((xxx.is_nympho(p) ? 0.5f : 1.0f) * rinterval * 2500.0f);
			}
			else
				return 50;
		}

	}
}
