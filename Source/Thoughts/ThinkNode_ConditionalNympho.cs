
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	public class ThinkNode_ConditionalNympho : ThinkNode_Conditional
	{

		protected override bool Satisfied(Pawn p)
		{
			return xxx.is_nympho(p);
		}

	}
}
