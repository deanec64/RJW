using Verse;
using Verse.AI;

namespace rjw
{
	public class JobGiver_ComfortPrisonerRape : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn p)
		{
			bool wildmode = Mod_Settings.WildMode;
			//Log.Message("[RJW] JobGiver_ComfortPrisonerRape::TryGiveJob( " + p.NameStringShort + " ) called0");
			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null))
			{
				//Log.Message("[RJW] JobGiver_ComfortPrisonerRape::TryGiveJob( " + p.NameStringShort + " ) called1");
				// don't allow pawns marked as comfort prisoners to rape others
				if ((xxx.is_healthy(p) && xxx.can_rape(p, xxx.has_traits(p) && xxx.is_nympho_or_rapist_or_zoophiliac(p)) || wildmode) && !comfort_prisoners.is_designated(p))
				{
					//Log.Message("[RJW] JobGiver_ComfortPrisonerRape::TryGiveJob( " + p.NameStringShort + " ) called2");
					Pawn target = xxx.find_prisoner_to_rape(p, p.Map);
					//Log.Message("[RJW] JobGiver_ComfortPrisonerRape::TryGiveJob( " + p.NameStringShort + " ) called3 - ("+((prisoner==null)? "NULL":prisoner.NameStringShort)+") is the prisoner");
					if (target != null)
					{
						if (xxx.is_human(target) && (xxx.is_rapist(p) || xxx.is_nympho(p) || wildmode)) //TODO: make a designation target for animals
						{
							return new Job(xxx.comfort_prisoner_rapin, target);
						}
						else if (xxx.is_animal(target) && (xxx.is_zoophiliac(p) || wildmode))
						{
							return new Job(xxx.comfort_prisoner_rapin, target);
						}
						else if (xxx.config.pawns_always_rapeCP)
							p.mindState.canLovinTick = Find.TickManager.TicksGame + 5;
						else
							p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
					}
				}
			}

			return null;
		}
	}
}