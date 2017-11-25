using System.Linq;
using Verse;

namespace rjw
{
	internal class JobDriver_RapeEnemyByMech : JobDriver_RapeEnemy
	{
		public JobDriver_RapeEnemyByMech()
		{
			this.requierCanRape = false;
		}

		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return xxx.is_mechanoid(rapist);
		}

		public override float GetFuckability(Pawn rapist, Pawn target)
		{
			//--Log.Message("[RJW]JobDriver_RapeEnemyByMech::GetFuckability("+ rapist.ToString()+","+ target.ToString() + ") - Force Rape");
			return 1f;
		}

		protected override void Impregnate(Pawn pawn, Pawn part, bool isAnalSex)
		{
			if (pawn.RaceProps.IsMechanoid && xxx.is_human(part))
			{
				//--Log.Message("[RJW]JobDriver_RapeEnemyByMech::aftersex - mech raped humans");
				/*foreach (var item in DefDatabase<HediffDef_MechImplants>.AllDefs)
				{
					//--Log.Message(pawn.def.defName + "Getting Implants\n" +item.defName + "\nParentDef:" + item.parentDef + "\nParentDefs:" + String.Join(",",item.parentDefs.ToArray()) );
				}*/
				HediffDef_MechImplants egg = (from x in DefDatabase<HediffDef_MechImplants>.AllDefs where x.IsParent(pawn.def.defName) select x).RandomElement<HediffDef_MechImplants>();

				PlantSomething(egg, part, isAnalSex, 1);
			}
		}
	}
}