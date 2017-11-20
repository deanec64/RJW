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
				if (xxx.need_some_sex(pawn) < 2f)
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
			SexualMentalStateDef d = this.def as SexualMentalStateDef;
			if (d == null)
			{
				return base.StateCanOccur(pawn);
			}
			else
			{
				return base.StateCanOccur(pawn) &&
					(!d.requireCanFuck || xxx.can_fuck(pawn)) &&
					(!d.requireCanBeFuck || xxx.can_be_fucked(pawn)) &&
					(!d.requireCanRape || xxx.can_rape(pawn)) &&
					(!d.requireCanGetRaped || xxx.can_get_raped(pawn) );
			}
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
	public class SexualMentalStateDef : MentalStateDef
	{
		public bool requireCanFuck = false;
		public bool requireCanBeFuck = false;
		public bool requireCanRape = false;
		public bool requireCanGetRaped = false;
	}
	public class SexualMentalBreakDef : MentalBreakDef
	{
		public SimpleCurve commonalityMultiplierBySexNeed;
	}
}
