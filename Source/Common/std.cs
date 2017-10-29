
using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{

    public class std_def : Verse.Def
    {
        public HediffDef hediff_def;
        public HediffDef cohediff_def = null;
        public float catch_chance;
        public float environment_pitch_chance = 0.0f;
        public float spawn_chance = 0.0f;
        public float spawn_severity = 0.0f;
        public float autocure_below_severity = -1.0f;
        public bool applied_on_genitals;
    }

    public static class std
    {

        public static std_def hiv;
        public static std_def herpes;
        public static std_def warts;
        public static std_def syphilis;
        public readonly static HediffDef immunodeficiency = DefDatabase<HediffDef>.GetNamed("Immunodeficiency");

        public static void init()
        {
            hiv = DefDatabase<std_def>.GetNamed("HIV");
            herpes = DefDatabase<std_def>.GetNamed("Herpes");
            warts = DefDatabase<std_def>.GetNamed("Warts");
            syphilis = DefDatabase<std_def>.GetNamed("Syphilis");


        }

        public static List<std_def> all
        {
            get
            {
                return DefDatabase<std_def>.AllDefsListForReading;
            }
        }

        // Returns how severely affected this pawn's crotch is by rashes and warts, on a scale from 0 to 3.
        public static int genital_rash_severity(Pawn p)
        {
            var tr = 0;

            var her = p.health.hediffSet.GetFirstHediffOfDef(herpes.hediff_def);
            if ((her != null) && (her.Severity >= 0.25f))
                ++tr;

            var war = p.health.hediffSet.GetFirstHediffOfDef(warts.hediff_def);
            if (war != null)
                tr += (war.Severity < 0.40f) ? 1 : 2;

            return tr;
        }

        public static Hediff get_infection(Pawn p, std_def sd)
        {
            return p.health.hediffSet.GetFirstHediffOfDef(sd.hediff_def);
        }

        public static float get_severity(Pawn p, std_def sd)
        {
            var hed = get_infection(p, sd);
            return (hed != null) ? hed.Severity : 0.0f;
        }

        public static Hediff infect(Pawn p, std_def sd, bool include_coinfection = true)
        {
            var existing = get_infection(p, sd);
            if (existing == null)
            {

                var part = (sd.applied_on_genitals) ? xxx.genitals : null;
                p.health.AddHediff(sd.hediff_def, part);
                if (include_coinfection && (sd.cohediff_def != null))
                    p.health.AddHediff(sd.cohediff_def, part);
                //Log.Message("[RJW]std::infect genitals std");
                return get_infection(p, sd);

            }
            else
                return existing;
        }

        public static void show_infection_letter(Pawn p, std_def sd, String source = null, float? chance = null)
        {
            StringBuilder info;
            {
                info = new StringBuilder();
                info.Append(p.NameStringShort + " has caught " + sd.label + ((source != null) ? " from " + source + "." : ""));
                if (chance.HasValue)
                    info.Append(" (" + chance.Value.ToStringPercent() + " chance)");
                info.AppendLine(); info.AppendLine();
                info.Append(sd.description);
            }
            Find.LetterStack.ReceiveLetter("Infection: " + sd.label, info.ToString(), LetterDefOf.BadNonUrgent, p);
        }

        public static void update(Pawn p)
        {
            update_immunodeficiency(p);

            // Check if any infections are below the autocure threshold and cure them if so
            foreach (var sd in all)
            {
                var inf = get_infection(p, sd);
                if ((inf != null) && (inf.Severity < sd.autocure_below_severity))
                {
                    p.health.RemoveHediff(inf);
                    if (sd.cohediff_def != null)
                    {
                        var coinf = p.health.hediffSet.GetFirstHediffOfDef(sd.cohediff_def);
                        if (coinf != null)
                            p.health.RemoveHediff(coinf);
                    }
                }
            }

            roll_for_syphilis_damage(p);
        }

        public static bool is_wasting_away(Pawn p)
        {
            var id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
            return ((id != null) && (id.CurStageIndex > 0));
        }

        public static void update_immunodeficiency(Pawn p)
        {
            var min_bf_for_id = 1.0f - immunodeficiency.minSeverity;
            var id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
            var bf = p.health.capacities.GetLevel(PawnCapacityDefOf.BloodFiltration);
            var has = (id != null);
            var should_have = (bf <= min_bf_for_id);

            if (has && (!should_have))
            {
                p.health.RemoveHediff(id);
                id = null;
            }
            else if ((!has) && should_have)
            {
                p.health.AddHediff(immunodeficiency);
                id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
            }

            if (id != null)
            {
                id.Severity = 1.0f - bf;

                // Roll for and apply opportunistic infections:
                // Pawns will have a 90% chance for at least one infection each year at 0% filtration, and a 0%
                // chance at 40% filtration, scaling linearly.
                // Let x = chance infected per roll
                // Then chance not infected per roll = 1 - x
                // And chance not infected on any roll in one day = (1 - x) ^ (60000 / 150) = (1 - x) ^ 400
                // And chance not infected on any roll in one year = (1 - x) ^ (400 * 60) = (1 - x) ^ 24000
                // So 0.10 = (1 - x) ^ 24000
                //    log (0.10) = 24000 log (1 - x)
                //    x = 0.00009593644334648975435114691213 = ~96 in 1 million
                // Important Note:
                // this function is called from Need_Sex::NeedInterval(), where it involves a needsex_tick and a std_tick to actually trigger this update_immunodeficiency.
                // j(this is not exactly the same as the value in Need_Sex, that value is 0, but here j should be 1) std_ticks per this function called, k needsex_ticks per std_tick, 150 ticks per needsex_tick, and x is the chance per 150 ticks,
                // The new equation should be .1 = (1-x)^(24000/kj)
                // log(.1) = (24000/kj) log(1-x),  so log(1-x)= (kj/24000) log(.1), 1-x = .1^(kj/24000), x= 1-.1^(kj/24000)
                // Since k=10,j=1, so kj=10, new x is 1-.1^(10/24000)=0.0009589504, let it be 959/1000000
                if ((Rand.RangeInclusive(1, 1000000) <= 959) && (Rand.Value < bf / min_bf_for_id))
                {
                    BodyPartRecord part;
                    {
                        var rv = Rand.Value;
                        if (rv < 0.25f)
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Jaw"));
                        else if (rv < 0.50f)
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "LeftLung"));
                        else if (rv < 0.75f)
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "RightLung"));
                        else
                            part = BodyDefOf.Human.AllParts.RandomElement();
                    }

                    if ((part != null) &&
                        (!p.health.hediffSet.PartIsMissing(part)) && (!p.health.hediffSet.HasDirectlyAddedPartFor(part)) &&
                        (p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WoundInfection) == null) && // If the pawn already has a wound infection, we can't properly set the immunity for the new one
                        (p.health.immunity.GetImmunity(HediffDefOf.WoundInfection) <= 0.0f))
                    { // Dont spawn infection if pawn already has immunity

                        p.health.AddHediff(HediffDefOf.WoundInfection, part);
                        p.health.HealthTick(); // Creates the immunity record
                        var ir = p.health.immunity.GetImmunityRecord(HediffDefOf.WoundInfection);
                        if (ir != null)
                            ir.immunity = xxx.config.opp_inf_initial_immunity;
                        Find.LetterStack.ReceiveLetter("Opportunistic Infection", "RJW_Opportunistic_Infection_Message".Translate(new object[] { p.NameStringShort }), LetterDefOf.BadNonUrgent, null);

                    }
                }
            }
        }
        public static void roll_to_catch(Pawn catcher, Pawn pitcher)
        {
            var has_artificial_genitals = catcher.health.hediffSet.HasDirectlyAddedPartFor(xxx.genitals);
            var thru_genitals = (!has_artificial_genitals) ? 1.00f : 0.15f;
            var on_genitals = (!has_artificial_genitals) ? 1.00f : 0.00f;

            // Roll for and apply food poisoning from rimming(a.k.a. licking anus)
            // Edited by nizhuan-jjr: get food poisoning by licking anus makes no senses, so I choose to commnet out and save some computation here.
            /*
            if (Genital_Helper.has_anus(catcher)&&(Rand.Value < xxx.config.chance_to_rim) && (!pitcher.health.hediffSet.HasHediff(HediffDefOf.FoodPoisoning))) {
                pitcher.health.AddHediff(HediffDefOf.FoodPoisoning);
                if (PawnUtility.ShouldSendNotificationAbout(pitcher)) {
                    Messages.Message("RJW_Poisoned_by_Rimming_Message".Translate(new object[] {pitcher.NameStringShort, catcher.NameStringShort}), pitcher, MessageSound.Negative);
                }
            }
            */

            float cleanliness_factor;
            {
                var room = catcher.GetRoom();
                var cle = (room != null) ? room.GetStat(RoomStatDefOf.Cleanliness) : xxx.config.std_outdoor_cleanliness;
                var exa = (cle >= 0.0f) ? xxx.config.std_env_pitch_cleanliness_exaggeration : xxx.config.std_env_pitch_dirtiness_exaggeration;
                cleanliness_factor = Mathf.Max(0.0f, 1.0f - exa * cle);
            }

            if (xxx.config.std_show_roll_to_catch)
                Log.Message(catcher.NameStringShort + " is rolling to catch STDs (cleanliness factor: " + cleanliness_factor.ToString() + ")" + ((!has_artificial_genitals) ? "" : " (has artificial genitals)"));

            foreach (var sd in all)
            {
                if (!catcher.health.hediffSet.HasHediff(sd.hediff_def))
                {
                    if (catcher.health.immunity.GetImmunity(sd.hediff_def) <= 0.0f)
                    {

                        var catch_chance = sd.catch_chance * (sd.applied_on_genitals ? on_genitals : thru_genitals);
                        var catch_rv = Rand.Value;
                        if (xxx.config.std_show_roll_to_catch)
                            Log.Message("  Chance to catch " + sd.label + ": " + catch_chance.ToStringPercent() + "; rolled: " + catch_rv.ToString());
                        if (catch_rv < catch_chance)
                        {

                            String pitch_source; float pitch_chance;
                            {
                                if (get_severity(pitcher, sd) >= xxx.config.std_min_severity_to_pitch)
                                {
                                    pitch_source = pitcher.NameStringShort;
                                    pitch_chance = 1.0f;
                                }
                                else
                                {
                                    pitch_source = "the environment";
                                    pitch_chance = sd.environment_pitch_chance * cleanliness_factor;
                                    if (!HugsLibInj.std_floor)
                                    {
                                        pitch_chance = -9001f;
                                    }
                                }
                            }
                            var pitch_rv = Rand.Value;

                            if (xxx.config.std_show_roll_to_catch)
                                Log.Message("    Chance to pitch (from " + pitch_source + "): " + pitch_chance.ToStringPercent() + "; rolled: " + pitch_rv.ToString());
                            if (pitch_rv < pitch_chance)
                            {
                                infect(catcher, sd);
                                show_infection_letter(catcher, sd, pitch_source, catch_chance * pitch_chance);
                                if (xxx.config.std_show_roll_to_catch)
                                    Log.Message("      INFECTED!");
                            }

                        }

                    }
                    else
                        if (xxx.config.std_show_roll_to_catch)
                        Log.Message("  Still immune to " + sd.label);
                }
                else
                    if (xxx.config.std_show_roll_to_catch)
                    Log.Message("  Already infected with " + sd.label);
            }
        }

        public static void generate_on(Pawn p)
        {
            if (!xxx.is_human(p))
                return;
            var nymph_mul = (!xxx.is_nympho(p)) ? 1.0f : xxx.config.nymph_spawn_with_std_mul;
            foreach (var sd in all)
                if (Rand.Value < sd.spawn_chance * nymph_mul)
                {
                    var hed = infect(p, sd, false);
                    float sev;
                    {
                        var r = Rand.Range(sd.hediff_def.minSeverity, sd.hediff_def.maxSeverity);
                        sev = Mathf.Clamp(sd.spawn_severity * r, sd.hediff_def.minSeverity, sd.hediff_def.maxSeverity);
                    }
                    hed.Severity = sev;
                }
        }

        public static void roll_for_syphilis_damage(Pawn p)
        {
            var syp = p.health.hediffSet.GetFirstHediffOfDef(syphilis.hediff_def);
            if ((syp != null) && (syp.Severity >= 0.60f) && (!syp.FullyImmune()))
            {

                // A 30% chance per day of getting any permanent damage works out to ~891 in 1 million for each roll
                // The equation is (1-x)^(60000/150)=.7
                // Important Note:
                // this function is called from Need_Sex::NeedInterval(), where it involves a needsex_tick and a std_tick to actually trigger this roll_for_syphilis_damage.
                // j(this is not exactly the same as the value in Need_Sex, that value is 0, but here j should be 1) std_ticks per this function called, k needsex_ticks per std_tick, 150 ticks per needsex_tick, and x is the chance per 150 ticks,
                // The new equation should be .7 = (1-x)^(400/kj)
                // 1-x = .7^(kj/400), x =1-.7^(kj/400)
                // Since k=10,j=1, so kj=10, new x is 1-.7^(10/400)=0.0088772362, let it be 888/100000
                if (Rand.RangeInclusive(1, 100000) <= 888)
                {
                    BodyPartRecord part; float sev;
                    {
                        var rv = Rand.Value;
                        if (rv < 0.10f)
                        {
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Brain"));
                            sev = 1.0f;
                        }
                        else if (rv < 0.50f)
                        {
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Liver"));
                            sev = (float)Rand.RangeInclusive(1, 3);
                        }
                        else if (rv < 0.75f)
                        {
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "LeftKidney"));
                            sev = (float)Rand.RangeInclusive(1, 2);
                        }
                        else
                        {
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "RightKidney"));
                            sev = (float)Rand.RangeInclusive(1, 2);
                        }
                    }

                    if ((part != null) && (!p.health.hediffSet.PartIsMissing(part)) && (!p.health.hediffSet.HasDirectlyAddedPartFor(part)))
                    {
                        var vir_dam = DefDatabase<DamageDef>.GetNamed("ViralDamage");
                        var dam_def = HealthUtility.GetHediffDefFromDamage(vir_dam, p, part);
                        var inj = (Hediff_Injury)HediffMaker.MakeHediff(dam_def, p, null);
                        inj.Severity = sev;
                        inj.TryGetComp<HediffComp_GetsOld>().IsOld = true;
                        p.health.AddHediff(inj, part, null);
                        Find.LetterStack.ReceiveLetter(syphilis.label + " Damage", "RJW_Syphilis_Damage_Message".Translate(new object[] { p.NameStringShort, (p.gender == Gender.Male ? "Prohis".Translate() : "Proher".Translate()), part.def.label, syphilis.label }), LetterDefOf.BadNonUrgent, p);
                    }
                }

            }
        }

    }
}
