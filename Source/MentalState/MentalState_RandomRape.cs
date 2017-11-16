using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;

namespace rjw
{
	public class MentalState_RandomRape : SexualMentalState
	{
		public override bool ForceHostileTo(Thing t)
		{
			if ((this.pawn.jobs != null) &&
				(this.pawn.jobs.curDriver != null) &&
				(this.pawn.jobs.curDriver as JobDriver_RandomRape != null))
			{
				return true;
			}
			return false;
		}

		public override bool ForceHostileTo(Faction f)
		{
			if ((this.pawn.jobs != null) &&
				(this.pawn.jobs.curDriver != null) &&
				(this.pawn.jobs.curDriver as JobDriver_RandomRape != null))
			{
				return true;
			}
			return false;
		}

		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}
	}
}
