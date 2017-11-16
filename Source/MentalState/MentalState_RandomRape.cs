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
		//set true to allow pawns avoid rape.(but sometime lt'll be long long chase... temporaly set false.
		static bool setHostileWhenRape = false;


		public override void PostStart(string reason)
		{
			base.PostStart(reason);
			this.pawn.mindState.canLovinTick = -1;
		}
		public override bool ForceHostileTo(Thing t)
		{
			if ((this.pawn.jobs != null) &&
				(this.pawn.jobs.curDriver != null) &&
				(this.pawn.jobs.curDriver as JobDriver_RandomRape != null) &&
				setHostileWhenRape)
			{
				return true;
			}
			return false;
		}

		public override bool ForceHostileTo(Faction f)
		{
			if ((this.pawn.jobs != null) &&
				(this.pawn.jobs.curDriver != null) &&
				(this.pawn.jobs.curDriver as JobDriver_RandomRape != null) &&
				setHostileWhenRape)
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
