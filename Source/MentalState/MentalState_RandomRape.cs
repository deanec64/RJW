using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;

namespace rjw
{
	class MentalState_RandomRape : MentalState
	{
		public override bool ForceHostileTo(Thing t)
		{
			return false;
		}

		public override bool ForceHostileTo(Faction f)
		{
			return false;
		}

		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}
	}
}
