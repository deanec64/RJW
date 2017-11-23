using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace rjw
{
	public class ThinkNode_ChancePerHour_RandomRape : ThinkNode_ChancePerHour
	{
		protected override float MtbHours(Pawn pawn)
		{
			if (pawn == null)
				return -1f;
			// Use the fappin mtb hours as the base number b/c it already accounts for things like age
			var base_mtb = xxx.config.comfort_prisoner_rape_mtbh_mul * ThinkNode_ChancePerHour_Fappin.get_fappin_mtb_hours(pawn);
			if (base_mtb < 0.0f)
				return -1.0f;

			float desire_factor;
			{
				var need_sex = pawn.needs.TryGetNeed<Need_Sex>();
				if (need_sex != null)
				{
					if (need_sex.CurLevel <= need_sex.thresh_frustrated())
						desire_factor = 0.15f;
					else if (need_sex.CurLevel <= need_sex.thresh_horny())
						desire_factor = 0.60f;
					else
						desire_factor = 1.00f;
				}
				else
					desire_factor = 1.00f;
			}

			float personality_factor;
			{
				personality_factor = 1.0f;
				if (pawn.story != null)
				{
					foreach (var trait in pawn.story.traits.allTraits)
					{
						if (trait.def == TraitDefOf.Bloodlust) personality_factor *= 0.25f;
						else if (trait.def == TraitDefOf.Brawler) personality_factor *= 0.50f;
						else if (trait.def == TraitDefOf.Psychopath) personality_factor *= 0.50f;
						else if (trait.def == TraitDefOf.Kind) personality_factor *= 30.00f;
					}
				}
			}

			float fun_factor;
			{
				if ((pawn.needs.joy != null) && (xxx.is_bloodlust(pawn)))
					fun_factor = Mathf.Clamp01(0.50f + pawn.needs.joy.CurLevel);
				else
					fun_factor = 1.00f;
			}

			var gender_factor = (pawn.gender == Gender.Male) ? 1.0f : 3.0f;

			return base_mtb * desire_factor * personality_factor * fun_factor * gender_factor;
		}

		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			try
			{
				return base.TryIssueJobPackage(pawn, jobParams);
			}
			catch (NullReferenceException e)
			{
				Logger.Message("[RJW]ThinkNode_ChancePerHour_RandomRape:TryIssueJobPackage - error message" + e.Message);
				Logger.Message("[RJW]ThinkNode_ChancePerHour_RandomRape:TryIssueJobPackage - error stacktrace" + e.StackTrace);
				return ThinkResult.NoJob; ;
			}
		}
	}
}