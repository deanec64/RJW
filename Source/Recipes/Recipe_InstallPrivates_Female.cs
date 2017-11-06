
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw
{
	public class Recipe_InstallPrivates_Female : Recipe_InstallArtificialBodyPart
	{

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			var gen_blo = Genital_Helper.genitals_blocked(p);
			if (p.gender == Gender.Female)
			{
				foreach (var part in base.GetPartsToApplyOn(p, r))
					if ((!gen_blo) || (part != xxx.genitals))
						yield return part;
			}
		}

	}
}
