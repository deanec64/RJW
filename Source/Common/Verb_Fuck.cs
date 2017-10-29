using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Sound;

using RimWorld;

namespace rjw
{
    /*This is useless for now.
	public class Verb_Fuck : Verb
	{
		private const int TargetCooldown = 40;

		protected override bool TryCastShot()
		{
			Pawn casterPawn = base.CasterPawn;

			Thing thing = this.currentTarget.Thing;
			casterPawn.Drawer.rotator.Face(thing.DrawPos);

			this.SoundJuicy().PlayOneShot(new TargetInfo(thing.Position, casterPawn.Map, false));
			casterPawn.Drawer.Notify_MeleeAttackOn(thing);

			casterPawn.Drawer.rotator.FaceCell(thing.Position);
			if (casterPawn.caller != null) {
				casterPawn.caller.Notify_DidMeleeAttack();
			}
			return true;
		}

		private SoundDef SoundJuicy()
		{
			return SoundDef.Named ("Sex");
		}
	}
    */
}
