using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;

namespace rjw
{
	public class SexualMentalState : MentalState
	{
		public override void MentalStateTick()
		{
			if (this.pawn.IsHashIntervalTick(150))
			{
				if (xxx.need_some_sex(pawn) < 1f)
				{
					this.RecoverFromState();
					return;
				}
			}
			base.MentalStateTick();
		}
	}

	public class SexualMentalStateWorker : MentalStateWorker
	{
		public override bool StateCanOccur(Pawn pawn)
		{
			return xxx.need_some_sex(pawn) >= 1f && base.StateCanOccur(pawn);
		}
	}

	public class SexualMentalBreakWorker : MentalBreakWorker
	{
		public override float CommonalityFor(Pawn pawn)
		{
			SexualMentalBreakDef d = this.def as SexualMentalBreakDef;
			if (d == null)
			{
				return base.CommonalityFor(pawn);
			} else{
				var need_sex = pawn.needs.TryGetNeed<Need_Sex>();
				return base.CommonalityFor(pawn) * d.commonalityMultiplierBySexNeed.Evaluate(need_sex.CurLevelPercentage*100f);
			}
		}
	}
	public class SexualMentalBreakDef : MentalBreakDef
	{
		public SimpleCurve commonalityMultiplierBySexNeed;
	}
	public class ThinkNode_ConditionalTrait : ThinkNode_Priority
	{
		private TraitDef trait;
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams){
		
			if((pawn.story!= null) &&
				(pawn.story.traits.HasTrait(this.trait)))
			{
				return base.TryIssueJobPackage(pawn, jobParams);
			}
			return ThinkResult.NoJob;
		}
	}
}
