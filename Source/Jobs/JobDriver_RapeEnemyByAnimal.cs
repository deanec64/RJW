using Verse;

namespace rjw
{
	internal class JobDriver_RapeEnemyByAnimal : JobDriver_RapeEnemy
	{
		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			return xxx.is_animal(rapist);
		}
	}
}