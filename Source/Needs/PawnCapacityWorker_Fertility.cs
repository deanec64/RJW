using System;
using System.Collections.Generic;

using Verse;

namespace rjw
{
	public class PawnCapacityWorker_Fertility : PawnCapacityWorker
	{
		public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
		{
			//return 1.0f;
			//Log.Message("[RJW]PawnCapacityWorker_Fertility::CalculateCapacityLevel is called0");
			//return 1f;
			Pawn p = diffSet.pawn;
			if (p == null)
				return 0f;
			if (xxx.is_animal(p))
				return 1f;
			else if (xxx.is_human(p))
			{
				if (p.ageTracker.AgeBiologicalYears > 50 || p.ageTracker.AgeBiologicalYears < 14)
				{
					return 0f;
				}
				float result = PawnCapacityUtility.CalculateTagEfficiency(diffSet, "RJW_FertilitySource", 1f, impactors); //This should be a value ranged in [0,1]. It seems always to be 1.
																														  //Log.Message("[RJW]PawnCapacityWorker_Fertility::CalculateCapacityLevel is called1 - result is "+ result);
				result *= GenMath.FlatHill(14f, 20f, 30f, 50f, p.ageTracker.AgeBiologicalYearsFloat); //This adds an aging factor to Fertility
																									  //Log.Message("[RJW]PawnCapacityWorker_Fertility::CalculateCapacityLevel is called2 - result is " + result);
				return result;
			}
			else return 0f;
		}
		public override bool CanHaveCapacity(BodyDef body)
		{
			//return true;
			return body.HasPartWithTag("RJW_FertilitySource");
		}
	}
}
