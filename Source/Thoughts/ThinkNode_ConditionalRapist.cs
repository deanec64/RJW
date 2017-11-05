
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class ThinkNode_ConditionalRapist : ThinkNode_Conditional {

		protected override bool Satisfied (Pawn p)
        {
            if (!xxx.config.random_rape_enabled)
                return false;
			if (ModSettings.WildMode) return true;
			if (!xxx.is_rapist(p))
                return false;
            if (!xxx.isSingleOrPartnerNotHere(p))
            {
                return false;
            }
            else return true;
		}

	}
}
