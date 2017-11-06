
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using Verse;
using RimWorld;

using Harmony;

namespace rjw
{

	[StaticConstructorOnStartup]
	static class First
	{

		// Generate a HediffGiver for the dummy hediff, then inject it into the OrganicStandard HediffGiverSet
		static void inject_sexualizer()
		{
			//--Log.Message("First::inject_sexualizer() called");
			/* Edited by nizhuan-jjr: I'm using the xpath approach to do this
            var hgs = DefDatabase<HediffGiverSetDef>.GetNamed("OrganicStandard");
            if (hgs != null) {
                var giv = new HediffGiver_Birthday
                {
                    hediff = HediffDef.Named("DummyPrivates"),
                    partsToAffect = new List<BodyPartDef>()
                };
                giv.partsToAffect.Add(DefDatabase<BodyPartDef>.GetNamed("Genitals"));
                giv.canAffectAnyLivePart = false;
                giv.ageFractionChanceCurve = new SimpleCurve
                {
                    { 0.00f, 1.0f },
                    { 0.05f, 1.0f },
                    { 0.06f, 0.0f }, // Stop triggering after 5% age so as not to spam the user with messages
                    { 1.00f, 0.0f } // about colonists getting dummy parts on their birthdays.
                };
                giv.averageSeverityPerDayBeforeGeneration = 0.0f;
                hgs.hediffGivers.Add(giv);
            }
            */
		}

		static void show_bpr(String body_part_record_def_name)
		{

			var bpr = BodyDefOf.Human.AllParts.Find((BodyPartRecord can) => String.Equals(can.def.defName, body_part_record_def_name));
			//--Log.Message(body_part_record_def_name + " BPR internals:");
			//--Log.Message("  def: " + bpr.def.ToString());
			//--Log.Message("  parts: " + bpr.parts.ToString());
			//--Log.Message("  parts.count: " + bpr.parts.Count.ToString());
			//--Log.Message("  height: " + bpr.height.ToString());
			//--Log.Message("  depth: " + bpr.depth.ToString());
			//--Log.Message("  coverage: " + bpr.coverage.ToString());
			//--Log.Message("  groups: " + bpr.groups.ToString());
			//--Log.Message("  groups.count: " + bpr.groups.Count.ToString());
			//--Log.Message("  parent: " + bpr.parent.ToString());
			//Log.Message ("  fleshCoverage: " + bpr.fleshCoverage.ToString ());
			//Log.Message ("  absoluteCoverage: " + bpr.absoluteCoverage.ToString ());
			//Log.Message ("  absoluteFleshCoverage: " + bpr.absoluteFleshCoverage.ToString ());
		}

		// Generate a BodyPartRecord for the genitals part and inject it into the Human BodyDef. By adding the
		// genitals at the end of the list of body parts we can hopefully avoid breaking existing saves with
		// mods that also modify the human BodyDef.
		// I don't think this is used anymore
		public static void inject_genitals(BodyDef target = null)
		{
			//--Log.Message("[RJW] First::inject_genitals() called");
			if (target == null)
			{
				target = BodyDefOf.Human;
			}

			Genital_Helper.inject_genitals(target);
			Genital_Helper.inject_breasts(target);
			Genital_Helper.inject_anus(target);
		}


		static void inject_recipes()
		{
			//--Log.Message("[RJW] First::inject_recipes");
			var cra_spo = DefDatabase<ThingDef>.GetNamed("CraftingSpot");
			var mac_ben = DefDatabase<ThingDef>.GetNamed("TableMachining");
			var tai_ben = DefDatabase<ThingDef>.GetNamed("ElectricTailoringBench");

			// Inject the recipes to create the artificial privates into the crafting spot or machining bench.
			// BUT, also dynamically detect if EPOE is loaded and, if it is, inject the recipes into EPOE's
			// crafting benches instead.
			var bas_ben = DefDatabase<ThingDef>.GetNamed("TableBasicProsthetic", false);
			(bas_ben ?? cra_spo).AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakePegDick"));

			var sim_ben = DefDatabase<ThingDef>.GetNamed("TableSimpleProsthetic", false);
			(sim_ben ?? mac_ben).AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeHydraulicVagina"));
			(sim_ben ?? mac_ben).AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeHydraulicBreasts"));
			(sim_ben ?? mac_ben).AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeHydraulicAnus"));

			var bio_ben = DefDatabase<ThingDef>.GetNamed("TableBionics", false);
			(bio_ben ?? mac_ben).AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeBionicVagina"));
			(bio_ben ?? mac_ben).AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeBionicPenis"));
			(bio_ben ?? mac_ben).AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeBionicAnus"));
			(bio_ben ?? mac_ben).AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeBionicBreasts"));



			// Inject the bondage gear recipes into their appropriate benches
			if (xxx.config.bondage_gear_enabled)
			{
				mac_ben.AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeHololock"));
				tai_ben.AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeArmbinder"));
				tai_ben.AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeGag"));
				tai_ben.AllRecipes.Add(DefDatabase<RecipeDef>.GetNamed("MakeChastityBelt"));
			}
		}

		static void inject_items()
		{
			//--Log.Message("[RJW] First::inject_items() called");
			/* Just use the Def to add ThingDef
            if (xxx.config.whore_beds_enabled)
            {
                var bedDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.thingClass == typeof(Building_Bed)).ToArray();

                var fields = typeof(ThingDef).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var bedDef in bedDefs)
                {
                    var whoreBedDef = new ThingDef();
                    foreach (var field in fields)
                    {
                        field.SetValue(whoreBedDef, field.GetValue(bedDef));
                    }
                    whoreBedDef.graphicData.texPath += "Whore";
                    whoreBedDef.defName += "Whore";
                    whoreBedDef.label = "WhoreBedFormat".Translate(whoreBedDef.label);
                    whoreBedDef.description = "WhoreBedDesc".Translate(whoreBedDef.description);
                    whoreBedDef.thingClass = typeof(Building_WhoreBed);
                    whoreBedDef.shortHash = 0;
                    typeof(ShortHashGiver).GetMethod("GiveShortHash", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { whoreBedDef, typeof(ThingDef) });
                    DefDatabase<ThingDef>.Add(whoreBedDef);
                }
            }
            */

		}

		static void show_bs(Backstory bs)
		{
			//--Log.Message("Backstory \"" + bs.Title + "\" internals:");
			//--Log.Message("  identifier: " + bs.identifier);
			//--Log.Message("  slot: " + bs.slot.ToString());
			//--Log.Message("  Title: " + bs.Title);
			//--Log.Message("  TitleShort: " + bs.TitleShort);
			//--Log.Message("  baseDesc: " + bs.baseDesc);
			//--Log.Message("  skillGains: " + ((bs.skillGains == null) ? "null" : bs.skillGains.ToString()));
			//--Log.Message("  skillGainsResolved: " + ((bs.skillGainsResolved == null) ? "null" : bs.skillGainsResolved.ToString()));
			//--Log.Message("  workDisables: " + bs.workDisables.ToString());
			//--Log.Message("  requiredWorkTags: " + bs.requiredWorkTags.ToString());
			//--Log.Message("  spawnCategories: " + bs.spawnCategories.ToString());
			//--Log.Message("  bodyTypeGlobal: " + bs.bodyTypeGlobal.ToString());
			//--Log.Message("  bodyTypeFemale: " + bs.bodyTypeFemale.ToString());
			//--Log.Message("  bodyTypeMale: " + bs.bodyTypeMale.ToString());
			//--Log.Message("  forcedTraits: " + ((bs.forcedTraits == null) ? "null" : bs.forcedTraits.ToString()));
			//--Log.Message("  disallowedTraits: " + ((bs.disallowedTraits == null) ? "null" : bs.disallowedTraits.ToString()));
			//--Log.Message("  shuffleable: " + bs.shuffleable.ToString());
		}
		static void CheckingCompatibleMods()
		{
			try
			{
				xxx.straight = DefDatabase<TraitDef>.GetNamedSilentFail("Straight");
				xxx.bisexual = DefDatabase<TraitDef>.GetNamedSilentFail("Bisexual");
				xxx.asexual = DefDatabase<TraitDef>.GetNamedSilentFail("Asexual");
				xxx.faithful = DefDatabase<TraitDef>.GetNamedSilentFail("Faithful");
				xxx.philanderer = DefDatabase<TraitDef>.GetNamedSilentFail("Philanderer");
				xxx.polyamorous = DefDatabase<TraitDef>.GetNamedSilentFail("Polyamorous");
				if (xxx.straight is null)
				{
					xxx.RomanceDiversifiedIsActive = false;
					//--Log.Message("[RJW]RomanceDiversified is not detected.0");

				}
				else
				{
					xxx.RomanceDiversifiedIsActive = true;
					//--Log.Message("[RJW]RomanceDiversified is detected.");
				}

			}
			catch (Exception)
			{
				xxx.RomanceDiversifiedIsActive = false;
				xxx.straight = null;
				xxx.bisexual = null;
				xxx.asexual = null;
				xxx.faithful = null;
				xxx.philanderer = null;
				xxx.polyamorous = null;
				//--Log.Message("[RJW]RomanceDiversified is not detected.1");
			}
			try
			{
				xxx.babystate = DefDatabase<HediffDef>.GetNamedSilentFail("BabyState");
				if (xxx.babystate is null)
				{
					xxx.RimWorldChildrenIsActive = false;
					//--Log.Message("[RJW]Children&Pregnancy is not detected.0");
				}
				else
				{
					xxx.RimWorldChildrenIsActive = true;
					//--Log.Message("[RJW]Children&Pregnancy is detected.");
				}

			}
			catch (Exception)
			{
				xxx.RimWorldChildrenIsActive = false; //A dirty way to check if the mod is active
				xxx.babystate = null;
				//--Log.Message("[RJW]Children&Pregnancy is not detected.1");
			}
		}

		static First()
		{
			//--Log.Message("[RJW] First::First() called");

			// check for required mods
			//CheckModRequirements();
			//CheckIncompatibleMods();
			CheckingCompatibleMods();


			inject_sexualizer();
			//inject_genitals();
			inject_recipes();
			inject_items();

			xxx.init(); // Must only be called after injections are complete
			nymph_backstories.init();
			std.init();
			bondage_gear_tradeability.init();

			var har = HarmonyInstance.Create("rjw");
			har.PatchAll(Assembly.GetExecutingAssembly());
			PATCH_Pawn_ApparelTracker_TryDrop.apply(har);
		}

		internal static void CheckModRequirements()
		{
			//--Log.Message("First::CheckModRequirements() called");
			List<string> required_mods = new List<string> {
				"HugsLib",
			};
			foreach (string required_mod in required_mods)
			{
				bool found = false;
				foreach (ModMetaData installed_mod in ModLister.AllInstalledMods)
				{
					if (installed_mod.Active && installed_mod.Name.Contains(required_mod))
					{
						found = true;
					}

					if (!found)
					{
						ErrorMissingRequirement(required_mod);
					}
				}
			}

		}

		internal static void CheckIncompatibleMods()
		{
			//--Log.Message("First::CheckIncompatibleMods() called");
			List<string> incompatible_mods = new List<string> {
				"Bogus Test Mod That Doesn't Exist"
			};
			foreach (string incompatible_mod in incompatible_mods)
			{
				foreach (ModMetaData installed_mod in ModLister.AllInstalledMods)
				{
					if (installed_mod.Active && installed_mod.Name.Contains(incompatible_mod))
					{
						ErrorIncompatibleMod(installed_mod);
					}
				}
			}
		}

		internal static void ErrorMissingRequirement(string missing)
		{
			Log.Error("Initialization error:  Unable to find required mod '" + missing + "' in mod list");
		}

		internal static void ErrorIncompatibleMod(ModMetaData othermod)
		{
			Log.Error("Initialization Error:  Incompatible mod '" + othermod.Name + "' detected in mod list");
		}
	}
}
