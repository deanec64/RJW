using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_StruggleInBondageGear : JobDriver
	{
		public Apparel target_gear
		{
			get
			{
				return (Apparel)TargetA.Thing;
			}
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return new Toil
			{
				initAction = delegate
				{
					pawn.pather.StopDead();
				},
				defaultCompleteMode = ToilCompleteMode.Delay,
				defaultDuration = 60
			};
			yield return new Toil
			{
				initAction = delegate
				{
					if (PawnUtility.ShouldSendNotificationAbout(pawn))
					{
						var pro = (pawn.gender == Gender.Male) ? "his" : "her";
						Messages.Message(pawn.NameStringShort + " struggles to remove " + pro + " " + target_gear.def.label + ". It's no use!", pawn, MessageSound.Negative);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}