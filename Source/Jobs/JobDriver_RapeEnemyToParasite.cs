

using System.Linq;
using Verse;

namespace rjw
{
    class JobDriver_RapeEnemyToParasite : JobDriver_RapeEnemy
	{
		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return false;
		}
		public override void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex = false)
		{
			base.aftersex(pawn, part, violent, isCoreLovin, isAnalSex);

			if (xxx.is_human(part) && part.gender == Gender.Female && Genital_Helper.has_vagina(part))
			{

				Hediff_Pregnant hediff_pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDef.Named("Parasite"), part);
				hediff_pregnant.father = pawn;
				part.health.AddHediff(hediff_pregnant);
			}
		}
	}
}
