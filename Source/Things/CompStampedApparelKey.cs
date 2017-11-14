using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

// Adds unlock options to right-click menu for holokeys.
namespace rjw
{
	public class CompStampedApparelKey : CompUsable
	{
		protected string make_label(Pawn p, Pawn other)
		{
			return FloatMenuOptionLabel + " on " + ((other == null) ? "self" : other.NameStringShort);
		}

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn p)
		{
			if (!p.CanReserve(parent))
				yield return new FloatMenuOption(FloatMenuOptionLabel + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.DisabledOption);
			else if (p.CanReach(parent, PathEndMode.Touch, Danger.Deadly))
			{
				// Option for the pawn to use the key on themself
				if (!p.is_wearing_locked_apparel())
					yield return new FloatMenuOption("Not wearing locked apparel", null, MenuOptionPriority.DisabledOption);
				else
					yield return this.make_option(make_label(p, null), p, null, null);

				if ((p.Map != null) && (p.Map == Find.VisibleMap))
				{
					// Options for use on downed colonists
					foreach (var other in p.Map.mapPawns.FreeColonists)
						if ((other != p) && other.Downed && other.is_wearing_locked_apparel())
							yield return this.make_option(make_label(p, other), p, other, null);

					// Options for use on prisoners
					foreach (var prisoner in p.Map.mapPawns.PrisonersOfColony)
						if (prisoner.is_wearing_locked_apparel())
							yield return this.make_option(make_label(p, prisoner), p, prisoner, WorkTypeDefOf.Warden);

					// Options for use on corpses
					foreach (var q in p.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse))
					{
						var corpse = q as Corpse;
						if (corpse.InnerPawn.is_wearing_locked_apparel())
							yield return this.make_option(make_label(p, corpse.InnerPawn), p, corpse, null);
					}
				}
			}
		}
	}
}