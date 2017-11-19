using System;
using Verse;

namespace rjw
{
	internal class JobDriver_RapeEnemyToParasite : JobDriver_RapeEnemy
	{
		public JobDriver_RapeEnemyToParasite()
		{
			this.requierCanRape = false;
		}

		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return false;
		}

		protected override void Impregnate(Pawn pawn, Pawn part, bool isAnalSex)
		{
			if (pawn == null || part == null || isAnalSex) return;
			//Log.Message("[RJW]JobDriver_RapeEnemyToParasite::impregnate( " + pawn.NameStringShort + ", " + part.NameStringShort + " ) called");

			if (pawn.health.hediffSet.HasHediff(xxx.sterilized) || part.health.hediffSet.HasHediff(xxx.sterilized))
				return;
			if (pawn.health.capacities.GetLevel(xxx.reproduction) <= 0 || part.health.capacities.GetLevel(xxx.reproduction) <= 0)
				return;

			Pawn male, female;
			if (pawn.gender == Gender.Male && part.gender == Gender.Female)
			{
				male = pawn;
				female = part;
			}
			else if (pawn.gender == Gender.Female && part.gender == Gender.Male)
			{
				male = part;
				female = pawn;
			}
			else
			{
				//Log.Message("[RJW] Same sex pregnancies not currently supported...");
				return;
			}

			// fertility check
			float fertility = (xxx.is_animal(female) ? Mod_Settings.pregnancy_coefficient_animals / 100f : Mod_Settings.pregnancy_coefficient_human / 100f);
			float ReproductionFactor = Math.Min(male.health.capacities.GetLevel(xxx.reproduction), female.health.capacities.GetLevel(xxx.reproduction));
			float pregnancy_threshold = fertility * ReproductionFactor;
			float pregnancy_chance = Rand.Value;

			if (pregnancy_chance > pregnancy_threshold)
			{
				//Log.Message("[RJW] Impregnation failed. Chance was " + pregnancy_chance + " vs " + pregnancy_threshold);
				return;
			}

			Hediff_Pregnant hediff_pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDef.Named("Parasite"), part);
			hediff_pregnant.father = pawn;
			part.health.AddHediff(hediff_pregnant);
		}
	}
}