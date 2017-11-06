
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw
{
	public class Recipe_RemoveBreasts : rjw_CORE_EXPOSED.Recipe_RemoveBodyPart
	{

		// Quick and dirty method to guess whether the player is harvesting the genitals or amputating them
		// due to infection. The core code can't do this properly because it considers the private part
		// hediffs as "unclean".
		public bool is_harvest(Pawn p, BodyPartRecord part)
		{
			foreach (var hed in p.health.hediffSet.hediffs)
				if ((hed.Part == part) && hed.def.isBad && (hed.Severity >= 0.70f))
					return false;

			return true;
		}

		public override void ApplyOnPawn(Pawn p, BodyPartRecord part, Pawn doer, List<Thing> ingredients)
		{
			var har = is_harvest(p, part);

			base.ApplyOnPawn(p, part, doer, ingredients);

			if (har)
			{
				if (!p.Dead)
					ThoughtUtility.GiveThoughtsForPawnOrganHarvested(p);
				else
					ThoughtUtility.GiveThoughtsForPawnExecuted(p, PawnExecutionKind.OrganHarvesting);
			}
		}

		public override string GetLabelWhenUsedOn(Pawn p, BodyPartRecord part)
		{
			return recipe.label.CapitalizeFirst();
		}

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			if (p.gender == Gender.Female)
			{
				foreach (var part in p.health.hediffSet.GetNotMissingParts())
					if (r.appliedOnFixedBodyParts.Contains(part.def) &&
						((part != xxx.breasts) || (!Genital_Helper.breasts_blocked(p))))
						yield return part;
			}
		}

	}
}
