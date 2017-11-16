using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class ThinkNode_ConditionalTrait : ThinkNode_Priority
	{
		private TraitDef trait;
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{

			if ((pawn.story != null) &&
				(pawn.story.traits.HasTrait(this.trait)))
			{
				return base.TryIssueJobPackage(pawn, jobParams);
			}
			return ThinkResult.NoJob;
		}
	}
}
