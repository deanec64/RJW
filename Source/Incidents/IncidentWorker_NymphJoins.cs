using RimWorld;
using Verse;

namespace rjw
{
	public class IncidentWorker_NymphJoins : IncidentWorker
	{
		protected override bool CanFireNowSub(IIncidentTarget target)
		{
			if (Mod_Settings.nymphos)
			{
				var m = (Map)target;
				var colonist_count = 0;
				var nymph_count = 0;
				foreach (var p in m.mapPawns.FreeColonists)
				{
					++colonist_count;
					if (xxx.is_nympho(p))
						++nymph_count;
				}
				var nymph_fraction = (float)nymph_count / (float)colonist_count;
				return (colonist_count >= 1) && (nymph_fraction < xxx.config.max_nymph_fraction);
			}
			else
				return false;
		}

		public override bool TryExecute(IncidentParms parms)
		{
			//Log.Message("IncidentWorker_NymphJoins::TryExecute() called");

			if (!Mod_Settings.nymphos)
			{
				return false;
			}

			Map m = (Map)parms.target;

			if (m == null)
			{
				//--Log.Message("IncidentWorker_NymphJoins::TryExecute() - map is null, abort!");
				return false;
			}
			else
			{
				//--Log.Message("IncidentWorker_NymphJoins::TryExecute() - map is ok");
			}

			IntVec3 loc;
			/*This could be an alternative
            if (!RCellFinder.TryFindRandomPawnEntryCell(out loc, m, CellFinder.EdgeRoadChance_Friendly + 0.2f, null))
            {
                return false;
            }
            */
			if (!CellFinder.TryFindRandomEdgeCellWith(m.reachability.CanReachColony, m, 1.0f, out loc)) // TODO check this ROADCHANCE
				return false;

			var p = nymph_generator.spawn_new(loc, ref m);

			Find.LetterStack.ReceiveLetter("Nymph Joins", "A wandering nymph has decided to join your colony.", LetterDefOf.Good, p);

			return true;
		}
	}
}