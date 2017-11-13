

using RimWorld;
using System.Linq;
using Verse;

namespace rjw
{
    class JobDriver_RapeEnemyByAnimal : JobDriver_RapeEnemy
    {
		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return xxx.is_animal(rapist);
		}
	}
}
