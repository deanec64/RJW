using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobGiver_ComfortPrisonerRape : ThinkNode_JobGiver {


		protected override Job TryGiveJob (Pawn p)
        {
			bool private wildmode = ModSettings.WildMode;
			Log.Message("[RJW] JobGiver_ComfortPrisonerRape::TryGiveJob( " + p.NameStringShort + " ) called0");
            if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null)) {

                Log.Message("[RJW] JobGiver_ComfortPrisonerRape::TryGiveJob( " + p.NameStringShort + " ) called1");
                // don't allow pawns marked as comfort prisoners to rape others
                if ((xxx.is_healthy(p) && xxx.can_rape(p,xxx.has_traits(p)&&xxx.is_nympho_or_rapist_or_zoophiliac(p)) && !comfort_prisoners.is_designated(p) ) || WildMode) {

                    //Log.Message("[RJW] JobGiver_ComfortPrisonerRape::TryGiveJob( " + p.NameStringShort + " ) called2");
                    Pawn prisoner = xxx.find_prisoner_to_rape(p, p.Map);

                    Log.Message("[RJW] JobGiver_ComfortPrisonerRape::TryGiveJob( " + p.NameStringShort + " ) called3 - ("+((prisoner==null)? "NULL":prisoner.NameStringShort)+") is the prisoner");
                    if (prisoner != null)
                        return new Job(xxx.comfort_prisoner_rapin, prisoner);
                    else if (xxx.config.pawns_always_rapeCP)
                        p.mindState.canLovinTick = Find.TickManager.TicksGame + 5;
                    else
                        p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
					
				}
			}
			
			return null;
		}
	}
}
