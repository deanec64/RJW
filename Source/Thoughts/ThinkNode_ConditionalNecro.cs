
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class ThinkNode_ConditionalNecro : ThinkNode_Conditional {

		protected override bool Satisfied (Pawn p)
		{
            return xxx.config.necro_enabled && xxx.is_necrophiliac(p);
		}

	}
}
