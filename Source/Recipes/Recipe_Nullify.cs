﻿using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_Nullify : rjw_CORE_EXPOSED.Recipe_RemoveBodyPart
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

		public override void ApplyOnPawn(Pawn p, BodyPartRecord part, Pawn doer, List<Thing> ingredients, Bill bill)
		{
			var har = is_harvest(p, part);

			base.ApplyOnPawn(p, part, doer, ingredients,bill);

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
			foreach (var part in p.health.hediffSet.GetNotMissingParts())
				if (r.appliedOnFixedBodyParts.Contains(part.def) &&
					((part != xxx.genitals) || (!Genital_Helper.genitals_blocked(p))))
					yield return part;
		}
	}
}