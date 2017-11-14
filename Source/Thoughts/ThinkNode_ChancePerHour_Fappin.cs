using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class ThinkNode_ChancePerHour_Fappin : ThinkNode_ChancePerHour
	{
		public static float get_fappin_mtb_hours(Pawn p)
		{
			if (p is null || p.Dead)
				return -1.0f;

			if (DebugSettings.alwaysDoLovin)
				return 0.1f;

			if (p.needs.food.Starving)
				return -1.0f;

			if (p.health.hediffSet.BleedRateTotal > 0.0f)
				return -1.0f;
			return (xxx.is_nympho(p) ? 0.5f : 1.0f) * rjw_CORE_EXPOSED.LovePartnerRelationUtility.LovinMtbSinglePawnFactor(p);
		}

		protected override float MtbHours(Pawn p)
		{
			bool can_get_job =
				(p.CurJob != null) &&
				p.jobs.curDriver.layingDown != LayingDownState.NotLaying;

			if (p.jobs.curDriver is JobDriver_LayDown)
			{
				bool is_horny;
				{
					is_horny = xxx.need_some_sex(p) > 1;
				}

				// TODO Bed check?

				if (can_get_job && is_horny)
				{
					float SexNeedFactor = (4 - xxx.need_some_sex(p)) / 2f;
					return get_fappin_mtb_hours(p) * SexNeedFactor;
				}
			}
			return -1.0f;
		}
	}
}