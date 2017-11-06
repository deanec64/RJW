
using System;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw
{
	public struct nymph_story
	{
		public Backstory child;
		public Backstory adult;
		public List<Trait> traits;
	}

	public struct nymph_passion_chances
	{
		public float major;
		public float minor;

		public nymph_passion_chances(float maj, float min)
		{
			major = maj;
			minor = min;
		}
	}

	public static class nymph_backstories
	{

		public struct child
		{
			public static Backstory vatgrown_sex_slave;
		};

		public struct adult
		{
			public static Backstory feisty;
			public static Backstory curious;
			public static Backstory tender;
			public static Backstory chatty;
			public static Backstory broken;
		};

		public static void init()
		{
			{
				Backstory bs = new Backstory();
				bs.identifier = ""; // identifier will be set by ResolveReferences
				MiscTranslationDef MTdef = DefDatabase<MiscTranslationDef>.GetNamedSilentFail("rjw_vatgrown_sex_slave");
				if (MTdef != null)
				{
					bs.SetTitle(MTdef.label);
					bs.SetTitleShort(MTdef.stringA);
					bs.baseDesc = MTdef.description;
				}
				else
				{
					bs.SetTitle("Vat-Grown Sex Slave");
					bs.SetTitleShort("SexSlave");
					bs.baseDesc = "NAME is made as a sex machine, rather than a human.During HIS growing period in the factory, HE was taught plenty of social skills and sex skills, just to satisfy various sex demands.";

				}
				// bs.skillGains = new Dictionary<string, int> ();
				bs.skillGains.Add("Social", 8);
				// bs.skillGainsResolved = new Dictionary<SkillDef, int> (); // populated by ResolveReferences
				bs.workDisables = WorkTags.Intellectual;
				bs.requiredWorkTags = WorkTags.None;
				bs.slot = BackstorySlot.Childhood;
				bs.spawnCategories = new List<string>() { "rjw_nymphsCategory", "Slave" }; // Not necessary (I think)
				bs.bodyTypeGlobal = BodyType.Thin;
				bs.bodyTypeFemale = BodyType.Female;
				bs.bodyTypeMale = BodyType.Thin;
				bs.forcedTraits = new List<TraitEntry>();
				bs.forcedTraits.Add(new TraitEntry(xxx.nymphomaniac, 0));
				bs.disallowedTraits = new List<TraitEntry>();
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesMen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesWomen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.TooSmart, 0));
				bs.shuffleable = true;
				bs.ResolveReferences();
				BackstoryDatabase.AddBackstory(bs);
				child.vatgrown_sex_slave = bs;
				Log.Message("[RJW]nymph_backstories::init() succeed0");

			}
			{
				Backstory bs = new Backstory();
				bs.identifier = "";
				MiscTranslationDef MTdef = DefDatabase<MiscTranslationDef>.GetNamedSilentFail("rjw_feisty");
				if (MTdef != null)
				{
					bs.SetTitle(MTdef.label);
					bs.SetTitleShort(MTdef.stringA);
					bs.baseDesc = MTdef.description;
				}
				else
				{
					bs.SetTitle("Feisty Nymph");
					bs.SetTitleShort("Nymph");
					bs.baseDesc = "NAME struts around the colony like a conqueror examining a newly annexed territory. You try to focus on your work but HE won't let you.\n" +
						"\"I heard what you said about me!\" HE accuses. You didn't say anything about HIM. HECAP knows that. " +
						"\"Wanna fight?\" HE challenges. You already know what HE means: a few minutes of play wrestling and then you take HIM, or, if you hesitate, HE takes you.\n" +
						"Though NAME can be hard to get along with you've come to appreciate how HE's more practical than the others.\n" +
						"You explain to HIM, \"See these things here? They need to be over there\" and \"Those people are trying to kill us, so let's kill them first.\" HECAP listens, and does what is necessary.";

				}
				bs.skillGains.Add("Social", -3);
				bs.skillGains.Add("Shooting", 4);
				bs.skillGains.Add("Melee", 6);
				bs.workDisables = (WorkTags.Cleaning | WorkTags.Animals | WorkTags.Caring | WorkTags.Artistic | WorkTags.ManualSkilled);
				bs.requiredWorkTags = WorkTags.None;
				bs.slot = BackstorySlot.Adulthood;
				bs.spawnCategories = new List<string>() { "rjw_nymphsCategory", "Slave" }; // Not necessary (I think)
				bs.bodyTypeGlobal = BodyType.Thin;
				bs.bodyTypeFemale = BodyType.Female;
				bs.bodyTypeMale = BodyType.Thin;
				bs.forcedTraits = new List<TraitEntry>();
				bs.forcedTraits.Add(new TraitEntry(xxx.nymphomaniac, 0));
				bs.disallowedTraits = new List<TraitEntry>();
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesMen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesWomen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.TooSmart, 0));
				bs.shuffleable = true;
				bs.ResolveReferences();
				BackstoryDatabase.AddBackstory(bs);
				adult.feisty = bs;
				Log.Message("[RJW]nymph_backstories::init() succeed1");
			}

			{
				Backstory bs = new Backstory();
				bs.identifier = "";
				MiscTranslationDef MTdef = DefDatabase<MiscTranslationDef>.GetNamedSilentFail("rjw_curious");
				if (MTdef != null)
				{
					bs.SetTitle(MTdef.label);
					bs.SetTitleShort(MTdef.stringA);
					bs.baseDesc = MTdef.description;
				}
				else
				{
					bs.SetTitle("Curious Nymph");
					bs.SetTitleShort("Nymph");
					bs.baseDesc =
						"Hold the covering plate back and place the actuator in slot C. Line the rod up. No, that won't do, it needs to be perfect. Now pop the JC-444 chip in place. \n" +
						"Solder the orange wire to pin #16 and the striped wire to pin #5. \n" +
						"Suddenly, there HE is. You aren't surprised to see NAME, you just didn't hear HIM approach. By now you're used to it. \n" +
						"You continue your work in silence as HE hovers over you, her large eyes fixated on the rapid, rythmic, deliberate motions of your hands.";
				}
				bs.skillGains.Add("Construction", 2);
				bs.skillGains.Add("Crafting", 6);
				bs.workDisables = (WorkTags.Animals | WorkTags.Artistic | WorkTags.Caring | WorkTags.Cooking | WorkTags.Mining | WorkTags.PlantWork | WorkTags.Violent | WorkTags.ManualDumb);
				bs.requiredWorkTags = WorkTags.None;
				bs.slot = BackstorySlot.Adulthood;
				bs.spawnCategories = new List<string>() { "rjw_nymphsCategory", "Slave" }; // Not necessary (I think)
				bs.bodyTypeGlobal = BodyType.Thin;
				bs.bodyTypeFemale = BodyType.Female;
				bs.bodyTypeMale = BodyType.Thin;
				bs.forcedTraits = new List<TraitEntry>();
				bs.forcedTraits.Add(new TraitEntry(xxx.nymphomaniac, 0));
				bs.disallowedTraits = new List<TraitEntry>();
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesMen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesWomen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.TooSmart, 0));
				bs.shuffleable = true;
				bs.ResolveReferences();
				BackstoryDatabase.AddBackstory(bs);
				adult.curious = bs;
				Log.Message("[RJW]nymph_backstories::init() succeed2");
			}

			{
				Backstory bs = new Backstory();
				bs.identifier = "";
				MiscTranslationDef MTdef = DefDatabase<MiscTranslationDef>.GetNamedSilentFail("rjw_tender");
				if (MTdef != null)
				{
					bs.SetTitle(MTdef.label);
					bs.SetTitleShort(MTdef.stringA);
					bs.baseDesc = MTdef.description;
				}
				else
				{
					bs.SetTitle("Tender Nymph");
					bs.SetTitleShort("Nymph");
					bs.baseDesc = "NAME has a pair of charming eyes, and HIS voice is soft and sweet. Due to HIS previous nurse job, HE tends people quite well.\n"
					+ "HE's crouched in the corner, hands over HIS ears and tears in HIS eyes."
					+ "You don't understand, then you notice it: the distant sound of the pained squeals of pigs being slaughtered.";
				}
				bs.skillGains.Add("Medicine", 4);
				bs.workDisables = (WorkTags.Animals | WorkTags.Artistic | WorkTags.Hauling | WorkTags.Violent | WorkTags.ManualSkilled);
				bs.requiredWorkTags = WorkTags.None;
				bs.slot = BackstorySlot.Adulthood;
				bs.spawnCategories = new List<string>() { "rjw_nymphsCategory", "Slave" }; // Not necessary (I think)
				bs.bodyTypeGlobal = BodyType.Thin;
				bs.bodyTypeFemale = BodyType.Female;
				bs.bodyTypeMale = BodyType.Thin;
				bs.forcedTraits = new List<TraitEntry>();
				bs.forcedTraits.Add(new TraitEntry(xxx.nymphomaniac, 0));
				bs.disallowedTraits = new List<TraitEntry>();
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesMen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesWomen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.TooSmart, 0));
				bs.shuffleable = true;
				bs.ResolveReferences();
				BackstoryDatabase.AddBackstory(bs);
				adult.tender = bs;
				Log.Message("[RJW]nymph_backstories::init() succeed3");
			}

			{
				Backstory bs = new Backstory();
				bs.identifier = "";
				MiscTranslationDef MTdef = DefDatabase<MiscTranslationDef>.GetNamedSilentFail("rjw_chatty");
				if (MTdef != null)
				{
					bs.SetTitle(MTdef.label);
					bs.SetTitleShort(MTdef.stringA);
					bs.baseDesc = MTdef.description;
				}
				else
				{
					bs.SetTitle("Chatty Nymph");
					bs.SetTitleShort("Nymph");
					bs.baseDesc = "No one can be more loquacious than NAME. HECAP has a flexible tongue, a sharp mind, and hot lips.";
				}
				bs.skillGains.Add("Social", 6);
				bs.workDisables = (WorkTags.Animals | WorkTags.Caring | WorkTags.Artistic | WorkTags.Violent | WorkTags.ManualDumb | WorkTags.ManualSkilled);
				bs.requiredWorkTags = WorkTags.None;
				bs.slot = BackstorySlot.Adulthood;
				bs.spawnCategories = new List<string>() { "rjw_nymphsCategory", "Slave" }; // Not necessary (I think)
				bs.bodyTypeGlobal = BodyType.Thin;
				bs.bodyTypeFemale = BodyType.Female;
				bs.bodyTypeMale = BodyType.Thin;
				bs.forcedTraits = new List<TraitEntry>();
				bs.forcedTraits.Add(new TraitEntry(xxx.nymphomaniac, 0));
				bs.disallowedTraits = new List<TraitEntry>();
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesMen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesWomen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.TooSmart, 0));
				bs.shuffleable = true;
				bs.ResolveReferences();
				BackstoryDatabase.AddBackstory(bs);
				adult.chatty = bs;
				Log.Message("[RJW]nymph_backstories::init() succeed4");
			}

			{
				Backstory bs = new Backstory();
				bs.identifier = "";
				MiscTranslationDef MTdef = DefDatabase<MiscTranslationDef>.GetNamedSilentFail("rjw_broken");
				if (MTdef != null)
				{
					bs.SetTitle(MTdef.label);
					bs.SetTitleShort(MTdef.stringA);
					bs.baseDesc = MTdef.description;
				}
				else
				{
					bs.SetTitle("Broken Nymph");
					bs.SetTitleShort("Nymph");
					bs.baseDesc = "Maybe NAME suffered some terrible things, HE looks rather emaciated, and no one wants to speak with HIM.\n" +
						"HECAP only behaves a bit vivaciously while staring at some rod-like stuffs.";
				}
				bs.skillGains.Add("Social", -5);
				bs.skillGains.Add("Artistic", 8);
				bs.workDisables = (WorkTags.Cleaning | WorkTags.Animals | WorkTags.Caring | WorkTags.Violent | WorkTags.ManualSkilled);
				bs.requiredWorkTags = WorkTags.None;
				bs.slot = BackstorySlot.Adulthood;
				bs.spawnCategories = new List<string>() { "rjw_nymphsCategory", "Slave" }; // Not necessary (I think)
				bs.bodyTypeGlobal = BodyType.Thin;
				bs.bodyTypeFemale = BodyType.Female;
				bs.bodyTypeMale = BodyType.Thin;
				bs.forcedTraits = new List<TraitEntry>();
				bs.forcedTraits.Add(new TraitEntry(xxx.nymphomaniac, 0));
				bs.disallowedTraits = new List<TraitEntry>();
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesMen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.DislikesWomen, 0));
				bs.disallowedTraits.Add(new TraitEntry(TraitDefOf.TooSmart, 0));
				bs.shuffleable = true;
				bs.ResolveReferences();
				BackstoryDatabase.AddBackstory(bs);
				adult.broken = bs;
				Log.Message("[RJW]nymph_backstories::init() succeed5");
			}


		}

		public static nymph_passion_chances get_passion_chances(Backstory child_bs, Backstory adult_bs, SkillDef skill_def)
		{
			var maj = 0.0f;
			var min = 0.0f;

			if (adult_bs == adult.feisty)
			{
				if (skill_def == SkillDefOf.Melee) { maj = 0.50f; min = 1.00f; }
				else if (skill_def == SkillDefOf.Shooting) { maj = 0.25f; min = 0.75f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.10f; min = 0.67f; }
			}
			else if (adult_bs == adult.curious)
			{
				if (skill_def == SkillDefOf.Construction) { maj = 0.15f; min = 0.40f; }
				else if (skill_def == SkillDefOf.Crafting) { maj = 0.50f; min = 1.00f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.20f; min = 1.00f; }
			}
			else if (adult_bs == adult.tender)
			{
				if (skill_def == SkillDefOf.Medicine) { maj = 0.20f; min = 0.60f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.50f; min = 1.00f; }
			}
			else if (adult_bs == adult.chatty)
			{
				if (skill_def == SkillDefOf.Social) { maj = 1.00f; min = 1.00f; }
			}
			else if (adult_bs == adult.broken)
			{
				if (skill_def == SkillDefOf.Artistic) { maj = 0.50f; min = 1.00f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.00f; min = 0.33f; }
			}

			return new nymph_passion_chances(maj, min);
		}

		// Randomly chooses backstories and traits for a nymph
		public static nymph_story generate()
		{
			var tr = new nymph_story();

			tr.child = child.vatgrown_sex_slave;

			tr.traits = new List<Trait>();
			tr.traits.Add(new Trait(xxx.nymphomaniac, 0, true));

			var beauty = 0;
			var rv = Rand.Value;
			var rv2 = Rand.Value;
			if (rv < 0.300)
			{
				tr.adult = adult.feisty;
				beauty = Rand.RangeInclusive(0, 2);
				if (rv2 < 0.33)
					tr.traits.Add(new Trait(TraitDefOf.Brawler));
				else if (rv2 < 0.67)
					tr.traits.Add(new Trait(TraitDefOf.Bloodlust));
			}
			else if (rv < 0.475)
			{
				tr.adult = adult.curious;
				beauty = Rand.RangeInclusive(0, 2);
				if (rv2 < 0.33)
					tr.traits.Add(new Trait(TraitDef.Named("Prosthophile")));
			}
			else if (rv < 0.650)
			{
				tr.adult = adult.tender;
				beauty = Rand.RangeInclusive(1, 2);
				if (rv2 < 0.50)
					tr.traits.Add(new Trait(TraitDefOf.Kind));
			}
			else if (rv < 0.825)
			{
				tr.adult = adult.chatty;
				beauty = 2;
				if (rv2 < 0.33)
					tr.traits.Add(new Trait(TraitDefOf.Greedy));
			}
			else
			{
				tr.adult = adult.broken;
				beauty = Rand.RangeInclusive(0, 2);
				if (rv2 < 0.33)
					tr.traits.Add(new Trait(TraitDefOf.DrugDesire, 1));
				else if (rv2 < 0.67)
					tr.traits.Add(new Trait(TraitDefOf.DrugDesire, 2));
			}

			if (beauty > 0)
				tr.traits.Add(new Trait(TraitDefOf.Beauty, beauty, false));

			return tr;
		}

	}
}
