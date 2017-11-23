using System;
using RimWorld;
using Verse;

namespace rjw
{
	public class IncidentWorker_TestInc : IncidentWorker
	{
		public static void list_backstories()
		{
			foreach (var bs in BackstoryDatabase.allBackstories.Values)
				Logger.Message("Backstory \"" + bs.Title + "\" has identifier \"" + bs.identifier + "\"");
		}

		public static void inject_designator()
		{
			var des = new Designator_ComfortPrisoner();
			Find.ReverseDesignatorDatabase.AllDesignators.Add(des);
		}

		public static void spawn_nymphs(Map m)
		{
			IntVec3 loc;
			if (!CellFinder.TryFindRandomEdgeCellWith(m.reachability.CanReachColony, m, 1.0f, out loc)) // TODO fix ROADCHANCE
				return;

			for (int i = 1; i <= 10; ++i)
				nymph_generator.spawn_new(loc, ref m);
			//Find.LetterStack.ReceiveLetter("Nymphs!", "A whole group of nymphs has wandered into your colony.", LetterDefOf.BadNonUrgent, null);
			Find.LetterStack.ReceiveLetter("Nymphs!", "A whole group of nymphs has wandered into your colony.", LetterDefOf.NegativeEvent, null);
		}

		// Applies permanent damage to a randomly chosen colonist, to test that this works
		public static void damage_virally(Map m)
		{
			var vir_dam = DefDatabase<DamageDef>.GetNamed("ViralDamage");
			var p = m.mapPawns.FreeColonists.RandomElement();
			var lun = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "LeftLung"));
			var dam_def = HealthUtility.GetHediffDefFromDamage(vir_dam, p, lun);
			var inj = (Hediff_Injury)HediffMaker.MakeHediff(dam_def, p, null);
			inj.Severity = 2.0f;
			inj.TryGetComp<HediffComp_GetsOld>().IsOld = true;
			p.health.AddHediff(inj, lun, null);
		}

		// Gives all colonists on the map a severe syphilis or HIV infection
		public static void infect_the_colonists(Map m)
		{
			foreach (var p in m.mapPawns.FreeColonists)
			{
				if (Rand.Value < 0.50f)
					std.infect(p, std.syphilis);
				// var std_hed_def = (Rand.Value < 0.50f) ? std.syphilis.hediff_def : std.hiv.hediff_def;
				// p.health.AddHediff (std_hed_def);
				// p.health.hediffSet.GetFirstHediffOfDef (std_hed_def).Severity = Rand.Range (0.50f, 0.90f);
			}
		}

		// Reduces the sex need of the selected pawn
		public static void reduce_sex_need_on_select(Map m)
		{
			Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
			if (pawn != null)
			{
				if (pawn.needs.TryGetNeed<Need_Sex>() != null)
				{
					Logger.Message("[RJW]TestInc::reduce_sex_need_on_select is called");
					pawn.needs.TryGetNeed<Need_Sex>().CurLevel -= 0.5f;
				}
			}
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			var m = (Map)parms.target;

			// list_backstories ();
			// inject_designator ();
			// spawn_nymphs (m);
			// damage_virally (m);
			//infect_the_colonists(m);
			reduce_sex_need_on_select(m);

			return true;
		}
	}
}