
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw
{
	public class Recipe_InstallPrivates_Male : Recipe_InstallArtificialBodyPart
	{

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			var gen_blo = Genital_Helper.genitals_blocked(p);
			if (p.gender == Gender.Male)
			{
				foreach (var part in base.GetPartsToApplyOn(p, r))
					if ((!gen_blo) || (part != xxx.genitals))
						yield return part;
			}
			else if (xxx.is_female(p))  //This allows futas
			{
				foreach (var part in base.GetPartsToApplyOn(p, r))
					if ((!gen_blo) || (part != xxx.genitals))
						yield return part;
			}
		}

	}
}
