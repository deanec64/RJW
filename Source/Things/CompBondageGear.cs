
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

// Adds options to the right-click menu for bondage gear to equip the gear on prisoners
namespace rjw {
	public class CompBondageGear : CompUsable {
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions (Pawn p)
		{
			if ((p.Map != null) && (p.Map == Find.VisibleMap) && (p.Map.mapPawns.PrisonersOfColonyCount > 0)) {
				if (!p.CanReserve (parent))
					yield return new FloatMenuOption (FloatMenuOptionLabel + " on prisoner (" + "Reserved".Translate () + ")", null, MenuOptionPriority.DisabledOption);
				else if (p.CanReach (parent, PathEndMode.Touch, Danger.Deadly))
					foreach (var prisoner in p.Map.mapPawns.PrisonersOfColony)
						yield return this.make_option (FloatMenuOptionLabel + " on " + prisoner.NameStringShort, p, prisoner, WorkTypeDefOf.Warden);
			}
		}
	}
}
