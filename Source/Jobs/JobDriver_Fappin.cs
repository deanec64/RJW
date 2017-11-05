
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobDriver_Fappin : JobDriver {

		private const int ticks_between_hearts = 100;
		
		private int ticks_left;
		
		private TargetIndex ibed = TargetIndex.A;
		
		private static readonly SimpleCurve fap_interval_from_age = new SimpleCurve {
			new CurvePoint (16f, 0.75f),
			new CurvePoint (22f, 0.75f),
			new CurvePoint (30f, 1.50f),
			new CurvePoint (50f, 3.00f),
			new CurvePoint (75f, 5.00f)
		};
		
		private Building_Bed Bed
		{
			get {
				return (Building_Bed)((Thing)base.CurJob.GetTarget(ibed));
			}
		}
		
		protected override IEnumerable<Toil> MakeNewToils ()
		{
			ticks_left = (int)(2500.0f * Rand.Range (0.20f, 0.70f));
			
			this.FailOnDespawnedOrNull (ibed);
			this.KeepLyingDown (ibed);
			yield return Toils_Bed.ClaimBedIfNonMedical (ibed, TargetIndex.None);
			yield return Toils_Bed.GotoBed (ibed);

			Toil do_fappin = Toils_LayDown.LayDown (ibed, true, false, false, false);
			do_fappin.AddPreTickAction (delegate {
				--this.ticks_left;
			    if (this.ticks_left <= 0)
			    	this.ReadyForNextToil ();
			    else if (pawn.IsHashIntervalTick (ticks_between_hearts))
			    	MoteMaker.ThrowMetaIcon (pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
			});
			do_fappin.AddFinishAction (delegate {
			    pawn.mindState.canLovinTick = Find.TickManager.TicksGame + generate_min_ticks_to_next_fappin (pawn);
				xxx.satisfy (pawn, null);
			});
			do_fappin.socialMode = RandomSocialMode.Off;
			yield return do_fappin;
		}
		
		private int generate_min_ticks_to_next_fappin (Pawn p)
		{
			if (! DebugSettings.alwaysDoLovin) {
				var interval = fap_interval_from_age.Evaluate (pawn.ageTracker.AgeBiologicalYearsFloat);
				var rinterval = Math.Max (0.5f, Rand.Gaussian (interval, 0.3f));
				return (int)((xxx.is_nympho (p) ? 0.5f : 1.0f) * rinterval * 2500.0f);
			} else
				return 50;
		}
		
	}
}
