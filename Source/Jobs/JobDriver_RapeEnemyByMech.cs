
using Verse;

namespace rjw
{
	class JobDriver_RapeEnemyByMech : JobDriver_RapeEnemy
	{
		public override void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex = false)
		{
			base.aftersex(pawn, part, violent, isCoreLovin, isAnalSex);

			/*if (pawn.raceprops.ismechanoid && xxx.is_human(part))
            {
                log.message("[abf]addmechaftersex::pretfix() - mech raped humans");
                if (isanalsex)
                {
                    implanttoanal("microcomputer", part);
                }
                else {
                    implanttogenital("microcomputer", part);
                }
            }*/
		}
	}
}
