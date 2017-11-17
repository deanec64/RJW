using System.Linq;
using Verse;

namespace rjw
{
	internal class JobDriver_RapeEnemyByInsect : JobDriver_RapeEnemy
	{
		public JobDriver_RapeEnemyByInsect()
		{
			this.requierCanRape = false;
		}

		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return xxx.is_insect(rapist);
		}

		protected override void Impregnate(Pawn pawn, Pawn part, bool isAnalSex)
		{
			if (xxx.is_human(part))
			{
				if (pawn.gender == Gender.Female)
				{
					HediffDef_InsectEgg egg = (from x in DefDatabase<HediffDef_InsectEgg>.AllDefs where x.IsParent(pawn.def.defName) select x).RandomElement<HediffDef_InsectEgg>();
					if (egg != null)
					{
						//Log.Message("[RJW]JobDriver_RapeEnemyByInsect::aftersex() - Planting egg " + egg.ToString());
						PlantSomething(egg, part, isAnalSex, Rand.Range(1, 2));
					}
					/*else
					{
						Log.Message("[RJW]JobDriver_RapeEnemyByInsect::aftersex() - There is no EggData of " + pawn.def.defName);
					}*/
				}
				else
				{
					//Log.Message("[RJW]JobDriver_RapeEnemyByInsect::aftersex() - Fertilize eggs");
					foreach (var egg in (from x in part.health.hediffSet.GetHediffs<Hediff_InsectEgg>() where x.IsParent(pawn.def.defName) select x))
					{
						egg.Fertilize(pawn);
					}
				}
			}
		}

		public override float GetFuckability(Pawn rapist, Pawn target)
		{
			if (rapist.gender == Gender.Female)
			{
				//Log.Message("[RJW]" + this.GetType().ToString() + "::GetFuckability(" + rapist.ToString() + ") - going to plant egg ->"+ target.ToString());
				return 1f; //Plant Eggs to everyone.
			}
			else
			{
				if ((from x in target.health.hediffSet.GetHediffs<Hediff_InsectEgg>() where x.IsParent(rapist.def.defName) select x).Count() > 0)
				{
					return 1f;//Trying to feritlize eggs to everyone planted eggs.
				}
			}
			return 0f;
		}
	}
}