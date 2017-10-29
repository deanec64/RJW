
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class ThinkNode_ConditionalHorny : ThinkNode_Conditional {

		protected override bool Satisfied (Pawn p)
		{
			return xxx.need_some_sex(p)>1;
		}

	}
}
