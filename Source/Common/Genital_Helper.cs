using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Verse;
using RimWorld;

using Harmony;


namespace rjw
{
    static class Genital_Helper
    {
        public static HediffDef penis = HediffDef.Named("Penis");
        public static HediffDef micropenis = HediffDef.Named("Micropenis");
        public static HediffDef small_penis = HediffDef.Named("SmallPenis");
        public static HediffDef big_penis = HediffDef.Named("BigPenis");
        public static HediffDef huge_penis = HediffDef.Named("HugePenis");
        public static HediffDef peg_penis = HediffDef.Named("PegDick");
        public static HediffDef bionic_penis = HediffDef.Named("BionicPenis");

        public static HediffDef vagina = HediffDef.Named("Vagina");
        public static HediffDef tight_vagina = HediffDef.Named("TightVagina");
        public static HediffDef loose_vagina = HediffDef.Named("LooseVagina");
        public static HediffDef hydraulic_vagina = HediffDef.Named("HydraulicVagina");
        public static HediffDef bionic_vagina = HediffDef.Named("BionicVagina");

        public static HediffDef breasts = HediffDef.Named("Breasts");
        public static HediffDef small_breasts = HediffDef.Named("SmallBreasts");
        public static HediffDef large_breasts = HediffDef.Named("LargeBreasts");
        public static HediffDef huge_breasts = HediffDef.Named("HugeBreasts");
        public static HediffDef hydraulic_breasts = HediffDef.Named("HydraulicBreasts");
        public static HediffDef bionic_breasts = HediffDef.Named("BionicBreasts");

        public static HediffDef anus = HediffDef.Named("Anus");
        public static HediffDef tight_anus = HediffDef.Named("TightAnus");
        public static HediffDef loose_anus = HediffDef.Named("LooseAnus");
        public static HediffDef hydraulic_anus = HediffDef.Named("HydraulicAnus");
        public static HediffDef bionic_anus = HediffDef.Named("BionicAnus");

        public static HediffDef dummy_privates_initializer = DefDatabase<HediffDef>.GetNamed("DummyPrivates");


        public static BodyPartRecord get_genitals(Pawn pawn)
        {
            //Log.Message("Genital_Helper::get_genitals( " + pawn.NameStringShort + " ) called");
            BodyPartRecord genitalPart = pawn.RaceProps.body.AllParts.Find(bpr => bpr.def.defName=="Genitals");

            //Log.Message("Genital_Helper::get_genitals( " + pawn.NameStringShort + " ) - checking genitalPart");
            if (genitalPart == null) {
                return null;
                //First.inject_genitals (pawn.RaceProps.body);
                //genitalPart = pawn.RaceProps.body.AllParts.Find ((BodyPartRecord bpr) => String.Equals (bpr.def.defName, "Genitals"));
            }

            return genitalPart;
        }

        public static BodyPartRecord get_breasts(Pawn pawn)
        {
            //Log.Message("[RJW] get_breasts( " + pawn.NameStringShort + " ) called");
            BodyPartRecord breastsPart = pawn.RaceProps.body.AllParts.Find(bpr => bpr.def.defName=="Chest");

            if (breastsPart == null)
            {
                //Log.Message("[RJW] get_breasts( " + pawn.NameStringShort + " ) - breastsPart is null");
                return null;
            }
            return breastsPart;
        }

        public static BodyPartRecord get_anus(Pawn pawn)
        {
            BodyPartRecord anusPart = pawn.RaceProps.body.AllParts.Find(bpr => bpr.def.defName=="Anus");

            if (anusPart == null)
            {
                return null;
            }
            return anusPart;
        }

		public static bool genitals_blocked (Pawn pawn)	{
			if (pawn.apparel != null)  {
                foreach (var app in pawn.apparel.WornApparel) {
                    if ((app.def is bondage_gear_def gear_def) && (gear_def.blocks_genitals))
                    {
                        return true;
                    }
                }
            }
            return false;
		}

        public static bool breasts_blocked(Pawn pawn) {
            if (pawn.apparel != null) { 
                foreach (var app in pawn.apparel.WornApparel) {
                    if ((app.def is bondage_gear_def gear_def) && (gear_def.blocks_breasts))
                        return true;
                }
            }
            return false;
        }

        public static bool anus_blocked( Pawn pawn)
        {
            if (pawn.apparel != null)
            {
                foreach (var app in pawn.apparel.WornApparel)
                {
                    if ((app.def is bondage_gear_def gear_def) && (gear_def.blocks_anus))
                        return true;
                }
            }
            return false;
        }

        //Comments from nizhuan-jjr to previous authors: Why do you want to sexualize WorldPawns? Why not simply sexualize visible pawns?
        //I don't remove this but want to.
        public static bool pawns_require_sexualization()
        {
            int count_sexualized = 0;
            bool found_one = false;

            foreach (var p in Find.WorldPawns.AllPawnsAliveOrDead)  //Comments from nizhuan-jjr to previous authors: Really not sure why you want to sexualize dead pawns, do you want to harvest organs from those dead? This is done by Harvest Organ Post Mortem 2.0.
            {
                if ((!p.Dead) || p.Spawned)
                {
                    if (Genital_Helper.is_sexualized(p))
                    {
                        ++count_sexualized;
                        if (count_sexualized > 50) //Late game worlds can have thousands of pawns. There's probably no point in checking all of them, and doing so could cause poor performance
                            break;
                    }
                    else
                    {
                        found_one = true;
                        break;
                    }
                }
            }

            if (!found_one)
            {
                foreach (var m in Find.Maps)
                {
                    count_sexualized = 0;
                    foreach (var p in m.mapPawns.AllPawns)
                    {
                        if (Genital_Helper.is_sexualized(p))
                        {
                            ++count_sexualized;
                            if (count_sexualized > 50)
                                break;
                        }
                        else
                        {
                            found_one = true;
                            break;
                        }
                    }
                }
            }

            return found_one;
        }

        public static bool has_genitals(Pawn pawn) {
            BodyPartRecord genitalPart = get_genitals(pawn);
            if (genitalPart is null)
                return false;
            return pawn.health.hediffSet.hediffs.Any((Hediff hed) =>
                                                  (hed.Part == genitalPart) &&
                                                  (((hed as Hediff_Implant) != null) || ((hed as Hediff_AddedPart) != null)) &&
                                                  (hed.def != dummy_privates_initializer));
        }

        public static bool has_breasts(Pawn pawn) {
            BodyPartRecord breastsPart = get_breasts(pawn);
            if (breastsPart is null)
                return false;
            return pawn.health.hediffSet.hediffs.Any((Hediff hed) =>
                                       (hed.Part == breastsPart) &&
                                       (((hed as Hediff_Implant) != null) || ((hed as Hediff_AddedPart) != null)) &&
                                       (hed.def != dummy_privates_initializer));
        }

        public static bool has_anus(Pawn pawn) {
            BodyPartRecord anusPart = get_anus(pawn);
            if (anusPart is null)
                return false;
            return pawn.health.hediffSet.hediffs.Any((Hediff hed) =>
                           (hed.Part == anusPart) &&
                           (((hed as Hediff_Implant) != null) || ((hed as Hediff_AddedPart) != null)) &&
                           (hed.def != dummy_privates_initializer));
        }

        public static bool has_penis(Pawn pawn)
        {
            BodyPartRecord penisPart = get_genitals(pawn);
            if (penisPart is null)
                return false;
            return pawn.health.hediffSet.hediffs.Any((Hediff hed) =>
                           (hed.Part == penisPart) &&
                           hed.def.defName.ToLower().Contains("penis"));
        }
        public static bool has_vagina(Pawn pawn)
        {
            BodyPartRecord vaginaPart = get_genitals(pawn);
            if (vaginaPart is null)
                return false;
            return pawn.health.hediffSet.hediffs.Any((Hediff hed) =>
                           (hed.Part == vaginaPart) &&
                           hed.def.defName.ToLower().Contains("vagina"));
        }

        public static bool is_sexualized (Pawn pawn)
		{
            return (xxx.is_female(pawn)? has_genitals(pawn) && has_anus(pawn) && has_breasts(pawn): has_genitals(pawn) && has_anus(pawn));
            //return has_genitals(pawn) && has_breasts(pawn) && has_anus(pawn);
        }

		public static void sexualize_everyone () {
			foreach (var p in Find.WorldPawns.AllPawnsAliveOrDead)
				if ((! p.Dead) || p.Spawned)
					Genital_Helper.sexualize (p);

			foreach (var m in Find.Maps)
				foreach (var p in m.mapPawns.AllPawns)
					Genital_Helper.sexualize (p);
		}

		public static void sexualize (Pawn pawn)
		{
            if (pawn.RaceProps.hasGenders &&(!xxx.is_mechanoid(pawn)||!xxx.is_animal(pawn))&& !is_sexualized(pawn))
            {
                sexualize_pawn(pawn);
            }
		}
        public static void sexualize_VisiblePawns()
        {
            foreach (Pawn p in Find.VisibleMap.mapPawns.AllPawnsSpawned)
            {
                sexualize(p);
            }
        }

        public static void add_genitals(Pawn pawn) {
            //Log.Message("Genital_Helper::add_genitals( " + pawn.NameStringShort + " ) called");
            BodyPartRecord genitalPart = get_genitals(pawn);
            //Log.Message("Genital_Helper::add_genitals( " + pawn.NameStringShort + " ) - checking genitals");
            if (genitalPart == null) {
                //Log.Message("[RJW] sexualize_pawn( " + pawn.NameStringShort + " ) doesn't have a genitalsPart");
                return;
            }else if (pawn.health.hediffSet.PartIsMissing(genitalPart))
            {
                //Log.Message("[RJW] sexualize_pawn( " + pawn.NameStringShort + " ) had a genital but was removed.");
                return;
            }
            if (has_genitals(pawn)) {
                //Log.Message("[RJW] sexualize_pawn( " + pawn.NameStringShort + " ) already has genitals");
                return;
            }

            HediffDef privates;
            double value = Rand.Value;

            //Log.Message("Genital_Helper::add_genitals( " + pawn.NameStringShort + " ) - checking race");
            if (pawn.RaceProps.IsMechanoid) {
                //Log.Message("Genital_Helper::add_genitals( " + pawn.NameStringShort + " ) - race is mechanoid");
                if (value < 0.30)
                    privates = pawn.gender == Gender.Male ? peg_penis : hydraulic_vagina;
                else
                    privates = pawn.gender == Gender.Male ? bionic_penis : bionic_vagina;
            } else if (pawn.RaceProps.Humanlike) {
                //Log.Message("Genital_Helper::add_genitals( " + pawn.NameStringShort + " ) - race is humanlike");
                if (value < 0.10)
                    privates = pawn.gender == Gender.Male ? micropenis : loose_vagina;
                else if (value < 0.20)
                    privates = pawn.gender == Gender.Male ? small_penis : loose_vagina;
                else if (value < 0.70)
                    privates = pawn.gender == Gender.Male ? penis : vagina;
                else if (value < 0.80)
                    privates = pawn.gender == Gender.Male ? big_penis : tight_vagina;
                else if (value < 0.90)
                    privates = pawn.gender == Gender.Male ? huge_penis : tight_vagina;
                else if (value < 0.95)
                    privates = pawn.gender == Gender.Male ? peg_penis : hydraulic_vagina;
                else
                    privates = pawn.gender == Gender.Male ? bionic_penis : bionic_vagina;

            } else {
                //Log.Message("Genital_Helper::add_genitals( " + pawn.NameStringShort + " ) - race is something else");
                privates = pawn.gender == Gender.Male ? penis : vagina;
            }
            //Log.Message("Genital_Helper::add_genitals( " + pawn.NameStringShort + " ) - adding hediff");
            pawn.health.AddHediff(privates, genitalPart);

        }

        public static void add_breasts(Pawn pawn) {
            //Log.Message("[RJW] add_breasts( " + pawn.NameStringShort + " ) called");
            BodyPartRecord breastsPart = get_breasts(pawn);

            if (breastsPart == null) {
                //Log.Message("[RJW] add_breasts( " + pawn.NameStringShort + " ) - pawn doesn't have a breastsPart");
                return;
            }
            else if (pawn.health.hediffSet.PartIsMissing(breastsPart))
            {
                //Log.Message("[RJW] sexualize_pawn( " + pawn.NameStringShort + " ) had breasts but were removed.");
                return;
            }
            if (has_breasts(pawn)) {
                //Log.Message("[RJW] add_breasts( " + pawn.NameStringShort + " ) - pawn already has breasts");
                return;
            }

            //Log.Message("[RJW] add_breasts( " + pawn.NameStringShort + " ) - checking gender");

            if (pawn.gender == Gender.Female) {
                HediffDef bewbs;
                double value = Rand.Value;

                if (pawn.RaceProps.IsMechanoid) {
                    if (value < 0.30)
                        bewbs = hydraulic_breasts;
                    else
                        bewbs = bionic_breasts;
                } else if (pawn.RaceProps.Humanlike) {
                    if (value < 0.10)
                        bewbs = small_breasts;
                    else if (value < 0.50)
                        bewbs = breasts;
                    else if (value < 0.70)
                        bewbs = large_breasts;
                    else if (value < 0.90)
                        bewbs = huge_breasts;
                    else if (value < 0.95)
                        bewbs = hydraulic_breasts;
                    else
                        bewbs = bionic_breasts;
                } else {
                    bewbs = breasts;
                }
                pawn.health.AddHediff(bewbs, breastsPart);
            }
        }

        public static void add_anus(Pawn pawn) {
            BodyPartRecord anusPart = get_anus(pawn);

            if (anusPart == null) {
                //Log.Message("[RJW] sexualize_pawn( " + pawn.NameStringShort + " ) doesn't have an anusPart");
                return;
            }
            else if (pawn.health.hediffSet.PartIsMissing(anusPart))
            {
                //Log.Message("[RJW] sexualize_pawn( " + pawn.NameStringShort + " ) had an anus but was removed.");
                return;
            }
            if (has_anus(pawn)) {
                //Log.Message("[RJW] sexualize_pawn( " + pawn.NameStringShort + " ) already has an anus");
                return;
            }

            HediffDef asshole;
            double value = Rand.Value;
            if (pawn.RaceProps.IsMechanoid) {
                if (value < 0.30)
                    asshole = hydraulic_anus;
                else
                    asshole = bionic_anus;
            } else if (pawn.RaceProps.Humanlike) {
                if (value < 0.20)
                    asshole = loose_anus;
                else if (value < 0.70)
                    asshole = anus;
                else if (value < 0.90)
                    asshole = tight_anus;
                else if (value < 0.95)
                    asshole = hydraulic_anus;
                else
                    asshole = bionic_anus;
            } else {
                asshole = anus;
            }

            pawn.health.AddHediff(asshole, anusPart);
            
        }

        public static void sexualize_pawn(Pawn pawn) {
            //Log.Message("[RJW] sexualize_pawn( " + pawn.NameStringShort + " ) called");

            if (!pawn.RaceProps.hasGenders) {
                //Log.Message("[RJW] sexualize_pawn() - unable to sexualize genderless pawn " + pawn.NameStringShort);
                return;
            }

            add_genitals(pawn);
            add_breasts(pawn);
            add_anus(pawn);

        } 

    public static void inject_genitals (BodyDef target)
		{
            //Log.Message("[RJW] inject_genitals() called");
			BodyPartRecord tor_rec = target.corePart;

			if (tor_rec != null) {

                var gen_rec = new BodyPartRecord
                {
                    def = DefDatabase<BodyPartDef>.GetNamed("Genitals"),
                    height = BodyPartHeight.Bottom,
                    depth = BodyPartDepth.Outside,
                    coverage = 0.02f
                };
                gen_rec.groups.Add (BodyPartGroupDefOf.Torso);
				gen_rec.parent = tor_rec;

				// TODO lots of broken/missing stuff here

				//gen_rec.fleshCoverage = 1.0f;
				//gen_rec.absoluteCoverage = gen_rec.parent.absoluteCoverage * gen_rec.coverage;
				//gen_rec.absoluteFleshCoverage = gen_rec.absoluteCoverage * gen_rec.fleshCoverage;

				// coverage is set by XML (or hardcoded like above)
				// absoluteCoverage is derived from coverage and the parent's absoluteCoverage
				// fleshCoverage is derived from the parts' coverages
				// absoluteFleshCoverage is derived from absoluteCoverage and fleshCoverage

				// so inserting the genitals affects the Torso's fleshCoverage which affects its absoluteFleshCoverage
				tor_rec.parts.Add (gen_rec);
				//if (gen_rec.coverage <= tor_rec.fleshCoverage)
					//tor_rec.fleshCoverage -= gen_rec.coverage;
				//else {
					//tor_rec.fleshCoverage = 0.0f;
					//Log.Warning ("[RJW] Torso BPR fleshCoverage pushed below zero during genitals injection");
				//}
				//tor_rec.absoluteFleshCoverage = tor_rec.absoluteCoverage * tor_rec.fleshCoverage;

				target.AllParts.Add (gen_rec);

			} else
				Log.Error ("[RJW] Failed to find the \"Torso\" BodyPartRecord");
		}

        public static void inject_breasts(BodyDef target)
        {
            //Log.Message("[RJW] inject_breasts() called");
            BodyPartRecord tor_rec = target.corePart;

            if (tor_rec != null)
            {

                var gen_rec = new BodyPartRecord
                {
                    def = DefDatabase<BodyPartDef>.GetNamed("Chest"),
                    height = BodyPartHeight.Top,
                    depth = BodyPartDepth.Outside,
                    coverage = 0.1f
                };
                gen_rec.groups.Add(BodyPartGroupDefOf.Torso);
                gen_rec.parent = tor_rec;

                // TODO lots of broken/missing stuff here

                //gen_rec.fleshCoverage = 1.0f;
                //gen_rec.absoluteCoverage = gen_rec.parent.absoluteCoverage * gen_rec.coverage;
                //gen_rec.absoluteFleshCoverage = gen_rec.absoluteCoverage * gen_rec.fleshCoverage;

                // coverage is set by XML (or hardcoded like above)
                // absoluteCoverage is derived from coverage and the parent's absoluteCoverage
                // fleshCoverage is derived from the parts' coverages
                // absoluteFleshCoverage is derived from absoluteCoverage and fleshCoverage

                // so inserting the genitals affects the Torso's fleshCoverage which affects its absoluteFleshCoverage
                tor_rec.parts.Add(gen_rec);
                //if (gen_rec.coverage <= tor_rec.fleshCoverage)
                //tor_rec.fleshCoverage -= gen_rec.coverage;
                //else {
                //tor_rec.fleshCoverage = 0.0f;
                //Log.Warning ("[RJW] Torso BPR fleshCoverage pushed below zero during genitals injection");
                //}
                //tor_rec.absoluteFleshCoverage = tor_rec.absoluteCoverage * tor_rec.fleshCoverage;

                target.AllParts.Add(gen_rec);

            }
            else
                Log.Error("[RJW] Failed to find the \"Torso\" BodyPartRecord");
        }

        public static void inject_anus(BodyDef target)
        {
            //Log.Message("[RJW] inject_anus() called");
            BodyPartRecord tor_rec = target.corePart;

            if (tor_rec != null)
            {

                var gen_rec = new BodyPartRecord
                {
                    def = DefDatabase<BodyPartDef>.GetNamed("Anus"),
                    height = BodyPartHeight.Bottom,
                    depth = BodyPartDepth.Outside,
                    coverage = 0.02f
                };
                gen_rec.groups.Add(BodyPartGroupDefOf.Torso);
                gen_rec.parent = tor_rec;

                // TODO lots of broken/missing stuff here

                //gen_rec.fleshCoverage = 1.0f;
                //gen_rec.absoluteCoverage = gen_rec.parent.absoluteCoverage * gen_rec.coverage;
                //gen_rec.absoluteFleshCoverage = gen_rec.absoluteCoverage * gen_rec.fleshCoverage;

                // coverage is set by XML (or hardcoded like above)
                // absoluteCoverage is derived from coverage and the parent's absoluteCoverage
                // fleshCoverage is derived from the parts' coverages
                // absoluteFleshCoverage is derived from absoluteCoverage and fleshCoverage

                // so inserting the genitals affects the Torso's fleshCoverage which affects its absoluteFleshCoverage
                tor_rec.parts.Add(gen_rec);
                //if (gen_rec.coverage <= tor_rec.fleshCoverage)
                //tor_rec.fleshCoverage -= gen_rec.coverage;
                //else {
                //tor_rec.fleshCoverage = 0.0f;
                //Log.Warning ("[RJW] Torso BPR fleshCoverage pushed below zero during genitals injection");
                //}
                //tor_rec.absoluteFleshCoverage = tor_rec.absoluteCoverage * tor_rec.fleshCoverage;

                target.AllParts.Add(gen_rec);

            }
            else
                Log.Error("[RJW] Failed to find the \"Torso\" BodyPartRecord");
        }
    }
}
