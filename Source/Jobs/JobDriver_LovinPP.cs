/*
 *
 * This class is no longer needed. The changes are now just patched into JobDriver_Lovin.MakeNewToils

using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobDriver_LovinPP : JobDriver_Lovin {
		private Pawn Partner
		{
			get
			{
				return (Pawn)((Thing)base.CurJob.GetTarget(TargetIndex.A));
			}
		}

		protected override IEnumerable<Toil> MakeNewToils ()
		{
			this.FailOn (() => (! xxx.can_fuck (pawn)));
			this.FailOn (() => (! xxx.can_fuck (Partner)));

			foreach (var t in base.MakeNewToils ())
				yield return t;

			// Apply the after-effects of sex. We have to do both pawns here because the partner
			// pawn's Lovin' job will fail and not run this code.
			yield return new Toil {
				initAction = delegate {
					xxx.aftersex (pawn, Partner);
					xxx.aftersex (Partner, pawn);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}

*/