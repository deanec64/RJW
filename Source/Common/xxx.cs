﻿
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

using UnityEngine;

using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;
using RimWorld.Planet;
//using static RimWorld.Planet.CaravanInventoryUtility;

using RimWorldChildren;

namespace rjw
{
    public static class xxx {
        public readonly static BindingFlags ins_public_or_no = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public readonly static config config = DefDatabase<config>.GetNamed("the_one");

        public const float base_sat_per_fuck = 0.40f;

        public const float base_attraction = 0.60f;

        public const float no_partner_ability = 0.8f;

        public readonly static TraitDef nymphomaniac = TraitDef.Named("Nymphomaniac");
        public readonly static TraitDef rapist = TraitDef.Named("Rapist");
        public readonly static TraitDef necrophiliac = TraitDef.Named("Necrophiliac");
        public readonly static TraitDef zoophiliac = TraitDef.Named("Zoophiliac");
        //RomanceDiversified Traits
        public static TraitDef straight;
        public static TraitDef bisexual;
        public static TraitDef asexual;
        public static TraitDef faithful;
        public static TraitDef philanderer;
        public static TraitDef polyamorous;
        public static bool RomanceDiversifiedIsActive; //A dirty way to check if the mod is active

        //Children&Pregnancy Hediffs
        public static HediffDef babystate;
        public static bool RimWorldChildrenIsActive; //A dirty way to check if the mod is active
        //The Hediff to prevent reproduction
        public readonly static HediffDef sterilized = HediffDef.Named("Sterilized");
        //The Hediff for broken body(resulted from being raped as CP for too many times)
        public readonly static HediffDef feelingBroken = HediffDef.Named("FeelingBroken");

        public static PawnCapacityDef reproduction = DefDatabase<PawnCapacityDef>.GetNamed("Reproduction");

        // Will be set in init. Can't be set earlier because the genitals part has to be injected first.
        public static BodyPartRecord genitals = null;
        public static BodyPartRecord breasts = null;
        public static BodyPartRecord anus = null;

        public readonly static ThoughtDef saw_rash_1 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates1");
        public readonly static ThoughtDef saw_rash_2 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates2");
        public readonly static ThoughtDef saw_rash_3 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates3");

        public readonly static ThoughtDef got_raped = DefDatabase<ThoughtDef>.GetNamed("GotRaped");
        public readonly static ThoughtDef got_raped_by_animal = DefDatabase<ThoughtDef>.GetNamed("GotRapedByAnimal");
        public readonly static ThoughtDef masochist_got_raped = DefDatabase<ThoughtDef>.GetNamed("MasochistGotRaped");
        public readonly static ThoughtDef masochist_got_raped_by_animal = DefDatabase<ThoughtDef>.GetNamed("MasochistGotRapedByAnimal");
        public readonly static ThoughtDef hate_my_rapist = DefDatabase<ThoughtDef>.GetNamed("HateMyRapist");
        public readonly static ThoughtDef kinda_like_my_rapist = DefDatabase<ThoughtDef>.GetNamed("KindaLikeMyRapist");
        public readonly static ThoughtDef allowed_me_to_get_raped = DefDatabase<ThoughtDef>.GetNamed("AllowedMeToGetRaped");
        public readonly static ThoughtDef stole_some_lovin = DefDatabase<ThoughtDef>.GetNamed("StoleSomeLovin");
        public readonly static ThoughtDef bloodlust_stole_some_lovin = DefDatabase<ThoughtDef>.GetNamed("BloodlustStoleSomeLovin");
        public readonly static ThoughtDef violated_corpse = DefDatabase<ThoughtDef>.GetNamed("ViolatedCorpse");

        public readonly static JobDef fappin = DefDatabase<JobDef>.GetNamed("Fappin");
        public readonly static JobDef gettin_loved = DefDatabase<JobDef>.GetNamed("GettinLoved");
        public readonly static JobDef nymph_rapin = DefDatabase<JobDef>.GetNamed("NymphJoinInBed");
        public readonly static JobDef gettin_raped = DefDatabase<JobDef>.GetNamed("GettinRaped");
        public readonly static JobDef comfort_prisoner_rapin = DefDatabase<JobDef>.GetNamed("ComfortPrisonerRapin");
        public readonly static JobDef violate_corpse = DefDatabase<JobDef>.GetNamed("ViolateCorpse");
        public readonly static JobDef beastiality = DefDatabase<JobDef>.GetNamed("Beastiality");
        public readonly static JobDef random_rape = DefDatabase<JobDef>.GetNamed("RandomRape");
        public readonly static JobDef inviting_visitors = DefDatabase<JobDef>.GetNamed("InvitingVisitors");
        public readonly static JobDef whore_is_serving_visitors = DefDatabase<JobDef>.GetNamed("WhoreIsServingVisitors");
        public readonly static JobDef struggle_in_BondageGear = DefDatabase<JobDef>.GetNamed("StruggleInBondageGear");
        public readonly static JobDef unlock_BondageGear = DefDatabase<JobDef>.GetNamed("UnlockBondageGear");
        public readonly static JobDef give_BondageGear = DefDatabase<JobDef>.GetNamed("GiveBondageGear");

        public readonly static ThingDef mote_noheart = ThingDef.Named("Mote_NoHeart");

        // bondage gear things
        public readonly static ThingDef holokey = ThingDef.Named("Holokey");

        public readonly static StatDef sex_stat = StatDef.Named("SexAbility");
        public readonly static StatDef vulnerability_stat = StatDef.Named("Vulnerability");

        public readonly static RecordDef GetRapedAsComfortPrisoner = DefDatabase<RecordDef>.GetNamed("GetRapedAsComfortPrisoner");

        public readonly static ThingDef cum = ThingDef.Named("FilthCum");

        //Anal raping related
        public readonly static RulePackDef analSexSucceeded = RulePackDef.Named("AnalSexSucceeded");
        public readonly static RulePackDef analSexFailed = RulePackDef.Named("AnalSexFailed");
        public readonly static InteractionDef analSex = DefDatabase<InteractionDef>.GetNamed("AnalSex");

        private static readonly SimpleCurve attractiveness_from_age_male = new SimpleCurve
        {
            new CurvePoint(0f, 0.0f),
            new CurvePoint(4f,  0.1f),
            new CurvePoint(5f,  0.6f),
            new CurvePoint(15f, 0.8f),
            new CurvePoint(20f, 1.0f),
            new CurvePoint(32f, 1.0f),
            new CurvePoint(40f, 0.9f),
            new CurvePoint(45f, 0.77f),
            new CurvePoint(50f, 0.3f),
            new CurvePoint(55f, 0.1f),
            new CurvePoint(60f, 0f)
        };

        private static readonly SimpleCurve attractiveness_from_age_female = new SimpleCurve
        {
            new CurvePoint(0f, 0.0f),
            new CurvePoint(4f,  0.1f),
            new CurvePoint(5f,  0.8f),
            new CurvePoint(14f, 1.0f),
            new CurvePoint(28f, 1.0f),
            new CurvePoint(30f, 0.9f),
            new CurvePoint(45f, 0.6f),
            new CurvePoint(53f, 0.1f),
            new CurvePoint(55f, 0f)
        };

        public static void init() {
            genitals = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Genitals"));
            breasts = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Chest"));
            anus = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Anus"));
        }

        public static void bootstrap(Map m) {
            if (m.GetComponent<MapCom_Injector>() == null)
                m.components.Add(new MapCom_Injector(m));
        }

        public static bool has_traits(Pawn pawn)
        {
            return (pawn != null && pawn.story != null && pawn.story.traits != null);
        }
        public static string random_pick_a_trait(this Pawn pawn)
        {
            return pawn.story.traits.allTraits.RandomElement().def.defName;
        }

        public static bool is_psychopath(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TraitDefOf.Psychopath));
        }

        public static bool is_bloodlust(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TraitDefOf.Bloodlust));
        }

        public static bool is_brawler(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TraitDefOf.Brawler));
        }

        public static bool is_kind(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TraitDefOf.Kind));
        }

        public static bool is_rapist(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(rapist));
        }

        public static bool is_necrophiliac(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(necrophiliac));
        }

        public static bool is_zoophiliac(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(zoophiliac));
        }


        public static bool is_masochist(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TraitDef.Named("Masochist")));
        }

        public static bool is_nympho(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(nymphomaniac));
        }

        public static bool is_gay(Pawn pawn) {
            return (pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TraitDefOf.Gay));
        }

        // A quick check on whether the pawn has the two traits
        // It's used in determine the eligibility of CP raping for the non-futa women
        // Before using it, you should make sure the pawn has traits.
        public static bool is_nympho_or_rapist_or_zoophiliac(Pawn pawn)
        {
            return pawn.story.traits.HasTrait(rapist) || pawn.story.traits.HasTrait(nymphomaniac) || pawn.story.traits.HasTrait(zoophiliac);
        }
        
        //RomanceDiversified Traits
        public static bool is_asexual(Pawn pawn)
        {
            return (xxx.RomanceDiversifiedIsActive && pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(asexual));
        }
        public static bool is_bisexual(Pawn pawn)
        {
            return (xxx.RomanceDiversifiedIsActive && pawn != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(bisexual));
        }

        public static bool is_whore(Pawn pawn)
        {
            return (pawn != null && pawn.ownership != null && pawn.ownership.OwnedBed is Building_WhoreBed &&(!xxx.RomanceDiversifiedIsActive||!pawn.story.traits.HasTrait(xxx.asexual)));
        }

        public static bool is_animal(Pawn pawn) {

            //return !pawn.RaceProps.Humanlike;
            //Edited by nizhuan-jjr:to make Misc.Robots not allowed to have sex. This change makes those robots not counted as animals.
            return pawn.RaceProps.Animal;
        }
        public static bool is_mechanoid(Pawn pawn) {//Added by nizhuan-jjr:to make Misc.Robots not allowed to have sex. Note:Misc.MAI is not a mechanoid. 

            return pawn.RaceProps.IsMechanoid;
        }
        public static bool is_tooluser(Pawn pawn) {
            return pawn.RaceProps.ToolUser;
        }
        public static bool is_human(Pawn pawn)
        {
            return pawn.RaceProps.Humanlike;//||pawn.kindDef.race == ThingDefOf.Human
        }
        public static bool is_female(Pawn pawn)
        {
            return pawn.gender == Gender.Female;
        }

        public static bool is_healthy(Pawn p)
        {
            return (!p.Dead) &&
                p.health.capacities.CanBeAwake &&
                (p.health.hediffSet.BleedRateTotal <= 0.0f) &&
                (p.health.hediffSet.PainTotal < config.significant_pain_threshold);
        }
        public static bool is_healthy_enough(Pawn p)
        {
            return (!p.Dead) && p.health.capacities.CanBeAwake && (p.health.hediffSet.BleedRateTotal <= 0.0f);  //don't care the pain level
        }

        public static float need_some_sex(Pawn pawn)
        {
            float horniness_degree=-1f;
            if (pawn != null)
            {
                var need_sex = pawn.needs.TryGetNeed<Need_Sex>();
                if (need_sex != null)
                {
                    if (need_sex.CurLevel < need_sex.thresh_frustrated()) horniness_degree = 3f;
                    else if (need_sex.CurLevel < need_sex.thresh_horny()) horniness_degree = 2f;
                    else if (need_sex.CurLevel < need_sex.thresh_satisfied()) horniness_degree = 1f;
                    else horniness_degree = 0f;
                }
            }
            return horniness_degree;
            
        }

        public static bool HasNonPolyPartner(Pawn p)
        {
            foreach (DirectPawnRelation relation in p.relations.DirectRelations)
            {
                if ((((relation.def == PawnRelationDefOf.Lover) || (relation.def == PawnRelationDefOf.Fiance)) || (relation.def == PawnRelationDefOf.Spouse)) && (xxx.RomanceDiversifiedIsActive ? !relation.otherPawn.story.traits.HasTrait(xxx.polyamorous) : true))
                {
                    return true;
                }
            }
            return false;
        }

        public static Gender opposite_gender(Gender g) {
            if (g == Gender.Male)
                return Gender.Female;
            else if (g == Gender.Female)
                return Gender.Male;
            else
                return Gender.None;
        }

        public static float get_sex_ability(Pawn p) {
            return p.GetStatValue(sex_stat, false);
        }

        public static float get_vulnerability(Pawn p)
        {
            return p.GetStatValue(vulnerability_stat, false);
        }

        public static bool isSingleOrPartnerNotHere(Pawn pawn)
        {
            return LovePartnerRelationUtility.ExistingLovePartner(pawn) == null || LovePartnerRelationUtility.ExistingLovePartner(pawn).Map != pawn.Map;
        }

        public static bool can_fuck(Pawn pawn) {
            if (is_human(pawn)) {
                return (pawn.ageTracker.AgeBiologicalYears >= HugsLibInj.sex_minimum_age) &&
                    Genital_Helper.has_genitals(pawn) && !Genital_Helper.genitals_blocked(pawn);
            }
            else
            {
                //return true;
                return config.animals_enabled && is_animal(pawn) && !is_mechanoid(pawn) && pawn.ageTracker.CurLifeStageIndex >= 2 && get_sex_ability(pawn) > 0.0f;
            }
        }

        public static bool can_be_fucked(Pawn pawn)
        {
            if (is_human(pawn))
            {
                return (pawn.ageTracker.AgeBiologicalYears >= HugsLibInj.sex_minimum_age) &&
                    (get_sex_ability(pawn) > 0.0f) && (!Genital_Helper.genitals_blocked(pawn));
            }
            else
            {
                //return true;
                return is_animal(pawn) && config.animals_enabled && !is_mechanoid(pawn) && pawn.ageTracker.CurLifeStageIndex >= 2 && get_sex_ability(pawn) > 0.0f;
            }
        }

        public static bool can_rape(Pawn pawn, bool AllowNonFutaFemaleRaping=false)
        {
			if (HugsLibInj.WildMode)
			{
				return true;
			}
			else if (is_human(pawn))
			{
				int age = pawn.ageTracker.AgeBiologicalYears;
				return (age >= HugsLibInj.sex_minimum_age) && (need_some_sex(pawn) > 0) && (!Genital_Helper.genitals_blocked(pawn))
					&& (HugsLibInj.NonFutaWomenRaping_MaxVulnerability < 0 ? Genital_Helper.has_penis(pawn) : (Genital_Helper.has_penis(pawn) || AllowNonFutaFemaleRaping && is_female(pawn) && get_vulnerability(pawn) <= HugsLibInj.NonFutaWomenRaping_MaxVulnerability)
					);
			}
			else
			{
				//return true;
				return is_animal(pawn) && config.animals_enabled && !is_mechanoid(pawn) && (pawn.ageTracker.CurLifeStageIndex >= 2) && get_sex_ability(pawn) > 0.0f && pawn.gender == Gender.Male;
			}
        }

        public static bool can_get_raped(Pawn pawn) {
            // Pawns can still get raped even if their genitals are destroyed/removed.
            // Animals can always be raped regardless of age
            if (is_human(pawn)) {
                int age = pawn.ageTracker.AgeBiologicalYears;
                return (HugsLibInj.WildMode || (age >= HugsLibInj.sex_minimum_age) && (get_sex_ability(pawn) > 0.0f) && !Genital_Helper.genitals_blocked(pawn) && (HugsLibInj.Rapee_MinVulnerability_human < 0 ? false : get_vulnerability(pawn) >= HugsLibInj.Rapee_MinVulnerability_human));
            } else if (is_animal(pawn) && config.animals_enabled )
            {
                float combatPower = pawn.kindDef.combatPower;
                float bodySize = pawn.RaceProps.baseBodySize;
                //Log.Message("[RJW]xxx::can_get_raped - animal pawn - vulnerability is "+ get_vulnerability(pawn));
                return combatPower <= 80 && bodySize<= 1.2 && bodySize>=0.25 && (get_sex_ability(pawn) > 0.0f) && !is_mechanoid(pawn) && (HugsLibInj.Rapee_MinVulnerability_animals<0? false:get_vulnerability(pawn) >= HugsLibInj.Rapee_MinVulnerability_animals);
            }
            return false;
        }

        // Returns how fuckable 'fucker' thinks 'p' is on a scale from 0.0 to 1.0
        public static float would_fuck(Pawn fucker, Pawn p, bool invert_opinion = false) {
            var fucker_age = fucker.ageTracker.AgeBiologicalYears;
            var p_age = p.ageTracker.AgeBiologicalYears;
            //Log.Message("[RJW]would_fuck("+fucker.NameStringShort+","+p.NameStringShort+","+invert_opinion.ToString()+") is called");
            if ((is_animal(fucker)||is_animal(p))&&!config.animals_enabled)
            {
                return 0f;
            }

            bool age_ok;
            {
                //var ffa_age = config.sex_free_for_all_age;
                //if (xxx.is_animal(fucker) && xxx.config.animals_enabled && (p_age >= xxx.config.sex_free_for_all_age)) {
                if (is_animal(fucker) && (p_age >= HugsLibInj.sex_free_for_all_age)) {
                    age_ok = true;
                } else if (is_animal(p) && (fucker_age >= HugsLibInj.sex_free_for_all_age)) {
                    // don't check the age of animals when they are the victim
                    age_ok = true;
                    //} else if ((fucker_age >= xxx.config.sex_free_for_all_age) && (p_age >= xxx.config.sex_free_for_all_age)) {
                } else if ((fucker_age >= HugsLibInj.sex_free_for_all_age) && (p_age >= HugsLibInj.sex_free_for_all_age)) {
                    age_ok = true;
                    //} else if ((fucker_age < xxx.config.sex_minimum_age) || (p_age < xxx.config.sex_minimum_age)) {
                } else if ((fucker_age < HugsLibInj.sex_minimum_age) || (p_age < HugsLibInj.sex_minimum_age)) {
                    age_ok = false;
                } else {
                    age_ok = Math.Abs(fucker.ageTracker.AgeBiologicalYearsFloat - p.ageTracker.AgeBiologicalYearsFloat) < 2.05f;
                }
            }

            //Log.Message("would_fuck() - age_ok = " + age_ok.ToString());
            if (age_ok) {
                if ((!(fucker.Dead || p.Dead)) &&
                    (!(fucker.needs.food.Starving || p.needs.food.Starving)) &&
                    (fucker.health.hediffSet.BleedRateTotal <= 0.0f) && (p.health.hediffSet.BleedRateTotal <= 0.0f)) {

                    float orientation_factor;  //0 or 1
                    {
                        Gender seeking = (!is_gay(fucker)) ? opposite_gender(fucker.gender) : fucker.gender;
                        if (is_asexual(fucker)) {
                            orientation_factor = 0f;
                        }
                        else if (is_bisexual(fucker))
                        {
                            orientation_factor = 1.0f;
                        }
                        else if (p.gender == seeking)
                        {
                            orientation_factor = 1.0f;
                        }
                        else {
                            orientation_factor = 0.1f;
                        }
                    }
                    //Log.Message("would_fuck() - orientation_factor = " + orientation_factor.ToString());

                    float age_factor = (p.gender == Gender.Male) ? attractiveness_from_age_male.Evaluate(p_age) : attractiveness_from_age_female.Evaluate(p_age);
                    //Log.Message("would_fuck() - age_factor = " + age_factor.ToString());

                    if (xxx.is_animal(fucker)) {  //0;0.1 to 1
                        age_factor = 1.0f;
                    }else if (is_animal(p))
                    {
                        if (!is_zoophiliac(fucker))
                            age_factor *= 0.1f;
                    }
                    //Log.Message("would_fuck() - animal age_factor = " + age_factor.ToString());

                    float body_factor; //0.8 to 1.25
                    {
                        if (p.story != null) {
                            if (p.story.bodyType == BodyType.Female) body_factor = 1.25f;
                            else if (p.story.bodyType == BodyType.Fat) body_factor = 1.0f;
                            else body_factor = 1.1f;

                            if (RelationsUtility.IsDisfigured(p))
                                body_factor *= 0.8f;
                        } else {
                            body_factor = 1.25f;
                        }
                    }
                    //Log.Message("would_fuck() - body_factor = " + body_factor.ToString());

                    float trait_factor;  // 0.7 to 1.3
                    {
                        if (p.story != null) {
                            var deg = p.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
                            trait_factor = 1.0f + 0.15f * (float)deg;
                        } else {
                            trait_factor = 1.0f;
                        }
                    }
                    //Log.Message("would_fuck() - trait_factor = " + trait_factor.ToString());

                    float opinion_factor;  //0.8 to 1.25
                    {
                        if (p.relations != null) {
                            var opi = (float)((!invert_opinion) ? fucker.relations.OpinionOf(p) : 100 - fucker.relations.OpinionOf(p)); // -100 to 100
                            opinion_factor = 0.8f + (opi + 100.0f) * (.45f / 200.0f); // 0.8 to 1.25
                        } else {
                            opinion_factor = 1.0f;
                        }
                    }
                    //Log.Message("would_fuck() - opinion_factor = " + opinion_factor.ToString());

                    float horniness_factor; // 1 to 1.5
                    {
                        float need_sex = need_some_sex(fucker);
                        switch (need_sex)
                        {
                            case 3:
                                horniness_factor = 1.5f;
                                break;
                            case 2:
                                horniness_factor = 1.3f;
                                break;
                            case 1:
                                horniness_factor = 1.1f;
                                break;
                            default:
                                horniness_factor = 1f;
                                break;
                        }
                    }
                    //Log.Message("would_fuck() - horniness_factor = " + horniness_factor.ToString());

                    float vulnerability_factor;  // 0.5;1.5 to 2.5
                    {
                        float vulnerabilityFucker = get_vulnerability(fucker); //0 to 3
                        float vulnerabilityP = get_vulnerability(p); //0 to 3
                        if (vulnerabilityFucker > vulnerabilityP)
                            vulnerability_factor = 0.5f;
                        else
                        {
                            if (is_masochist(p) || is_rapist(fucker) || is_bloodlust(fucker) || is_psychopath(fucker) || is_nympho(fucker) || (is_zoophiliac(fucker) && is_animal(p)))
                            {
                                vulnerability_factor = 2f + 0.5f * Mathf.InverseLerp(vulnerabilityFucker, 3f, vulnerabilityP);
                            }
                            else
                            {
                                vulnerability_factor = 1.5f+ 0.5f * Mathf.InverseLerp(vulnerabilityFucker, 3f, vulnerabilityP);
                            }

                        }
                    }
                    //Log.Message("would_fuck() - horniness_factor = " + horniness_factor.ToString());

                    //The maximium would be 1.25*1.3*1.25*1.5*2.5=7.6171875; average is .5*1.025*1.025*1.25*2=1.31328125; minimium except 0 is .1*.1*.8*.7*.8*.5 = 0.00224
                    var prenymph_att = Mathf.InverseLerp(0f, 4f, base_attraction * orientation_factor * age_factor * body_factor * trait_factor * opinion_factor * horniness_factor* vulnerability_factor); // 0 to 1
                    var final_att = (!is_nympho(fucker)) ? prenymph_att : 0.2f + 0.8f * prenymph_att;
                    Log.Message("would_fuck( " + fucker.NameStringShort + ", " + p.NameStringShort + " ) - prenymph_att = " + prenymph_att.ToString() + ", final_att = " + final_att.ToString());

                    return final_att;

                } else
                    return 0.0f;
            } else
                return 0.0f;
        }

        public static void satisfy(Pawn pawn, Pawn part, bool violent = false,bool isCoreLovin = false) {
            string pawn_name = (pawn != null) ? pawn.NameStringShort : "NULL";
            string part_name = (part != null) ? part.NameStringShort : "NULL";
            Log.Message("xxx::satisfy( " + pawn_name + ", " + part_name + ", " + violent + " ) called");
            var base_satisfaction_per_event = base_sat_per_fuck;
            var pawn_ability = (pawn != null) ? get_sex_ability(pawn) : no_partner_ability;
            var part_ability = (part != null) ? get_sex_ability(part) : no_partner_ability;

            Log.Message("xxx::satisfy( " + pawn_name + ", " + part_name + ", " + violent + " ) - calculate base satisfaction");
            // Base satisfaction is based on partner's ability
            var pawn_satisfaction = base_satisfaction_per_event * part_ability;
            var part_satisfaction = base_satisfaction_per_event * pawn_ability;

            Log.Message("xxx::satisfy( " + pawn_name + ", " + part_name + ", " + violent + " ) - modifying pawn satisfaction");
            if (pawn != null && (xxx.is_rapist(pawn) || xxx.is_bloodlust(pawn))) {
                // Rapists and Bloodlusts get more satisfaction from violetn encounters
                // Rapists and Bloodlusts get less satisfaction from non-violent encounters
                pawn_satisfaction *= (violent) ? 1.20f : 0.8f;
            } else {
                // non-violent pawns get less satisfaction from violent encounters
                // non-violent pawns get full satisfaction from non-violent encounters
                pawn_satisfaction *= (violent) ? 0.8f : 1.0f;
            }

            Log.Message("xxx::satisfy( " + pawn_name + ", " + part_name + ", " + violent + " ) - modifying part satisfaction");
            if (part!=null && !part.Dead && xxx.is_masochist(part) ) {
                // masochists get some satisfaction from violent encounters
                // masochists get less satisfaction from non-violent encounters
                part_satisfaction *= (violent) ? 0.8f : 0.5f;
            } else {
                // non-masochists get little satisfaction from violent encounters
                // non-masochists get full satisfaction from non-violent encounters
                part_satisfaction *= (violent) ? 0.2f : 1.0f;
            }

            Log.Message("xxx::satisfy( " + pawn_name + ", " + part_name + ", " + violent + " ) - setting pawn joy");
            if (pawn != null && pawn.needs != null) {
                if (pawn.needs.TryGetNeed<Need_Sex>() != null)
                {
                    pawn.needs.TryGetNeed<Need_Sex>().CurLevel += pawn_satisfaction;
                    if (pawn.needs.joy != null)
                        pawn.needs.joy.CurLevel += pawn_satisfaction * 0.50f;       // convert half of satisfaction to joy
                }
                    
            }

            if (part != null && part.needs != null && !part.Dead && !isCoreLovin) {
                if (part.needs.TryGetNeed<Need_Sex>() != null)
                {
                    if (is_female(pawn)&&!is_female(part))  //Males are being fucked by female may feel a bit better. I don't bother to check the sex orientations here, because it'll be quite a work.
                        part_satisfaction *= 1.05f;
                    Log.Message("xxx::satisfy( " + pawn_name + ", " + part_name + ", " + violent + " ) - setting part joy");
                    part.needs.TryGetNeed<Need_Sex>().CurLevel += part_satisfaction;
                    if (part.needs.joy != null)
                        part.needs.joy.CurLevel += part_satisfaction * 0.50f;       // convert half of satisfaction to joy
                }
                    
            }

            Log.Message("xxx::satisfy( " + pawn_name + ", " + part_name + ", " + violent + " ) - pawn_satisfaction = " + pawn_satisfaction + ", part_satisfaction = " + part_satisfaction);

        }

        public static bool bed_has_at_least_two_occupants(Building_Bed bed) {
            int occupantc = 0;
            foreach (var occ in bed.CurOccupants)
                if (++occupantc >= 2)
                    break;
            return occupantc >= 2;
        }

        public static bool is_laying_down_alone(Pawn p) {
            if ((p.CurJob == null) ||
                (p.jobs.curDriver.layingDown == LayingDownState.NotLaying))
                return false;

            Building_Bed bed = null;

            if (p.jobs.curDriver is JobDriver_LayDown) {
                bed = ((JobDriver_LayDown)p.jobs.curDriver).Bed;
            }

            if (bed != null)
                return !bed_has_at_least_two_occupants(bed);
            else
                return true;
        }

        public static int generate_min_ticks_to_next_lovin(Pawn pawn) {
            if (!DebugSettings.alwaysDoLovin) {
                float interval = rjw_CORE_EXPOSED.JobDriver_Lovin.LovinIntervalHoursFromAgeCurve.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
                float rinterval = Math.Max(0.5f, Rand.Gaussian(interval, 0.3f));

                float tick = 1.0f;

                if (xxx.is_animal(pawn) || xxx.is_nympho(pawn)) {
                    tick = 0.5f;
                }

                return (int)(tick * rinterval * 2500.0f);
            } else {
                return 100;
            }
        }


        public static void sexTick(Pawn pawn, Pawn partner) {
            pawn.Drawer.rotator.Face(partner.DrawPos);

            if (xxx.config.sounds_enabled) {
                SoundDef.Named("Sex").PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
            }

            pawn.Drawer.Notify_MeleeAttackOn(partner);
            pawn.Drawer.rotator.FaceCell(partner.Position);
        }

        public static void think_after_sex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false) {
            Log.Message("xxx::think_after_sex( " + pawn.NameStringShort + ", " + part.NameStringShort + ", " + violent + " ) called");

            Log.Message("xxx::think_after_sex( " + pawn.NameStringShort + ", " + part.NameStringShort + ", " + violent + " ) - setting pawn thoughts");
            // pawn thoughts
            // Edited by nizhuan-jjr:The two types of stole_sone_lovin are violent due to the description, so I make sure the thought would only trigger after violent behaviors. 
            if (!xxx.is_animal(pawn) && violent ) {
                var pawn_thought = ( xxx.is_rapist(pawn) || xxx.is_bloodlust(pawn)) ? xxx.bloodlust_stole_some_lovin : xxx.stole_some_lovin;
                pawn.needs.mood.thoughts.memories.TryGainMemory(pawn_thought);

                if (xxx.is_necrophiliac(pawn) && part.Dead) {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(xxx.violated_corpse);
                }
            }

            Log.Message("xxx::think_after_sex( " + pawn.NameStringShort + ", " + part.NameStringShort + ", " + violent + " ) - setting part thoughts");
            // partner thoughts
            if (!xxx.is_animal(part) && violent && !part.Dead &&!isCoreLovin) {
                if (xxx.is_animal(pawn)) {
                    var part_thought = (xxx.is_masochist(part)) ? xxx.masochist_got_raped_by_animal : xxx.got_raped_by_animal;
                    part.needs.mood.thoughts.memories.TryGainMemory(part_thought);
                } else {
                    var part_thought = (xxx.is_masochist(part)) ? xxx.masochist_got_raped : xxx.got_raped;
                    part.needs.mood.thoughts.memories.TryGainMemory(part_thought);

                    var part_thought_about_rapist = (!xxx.is_masochist(part)) ? xxx.hate_my_rapist : xxx.kinda_like_my_rapist;
                    part.needs.mood.thoughts.memories.TryGainMemory(part_thought_about_rapist, pawn);
                }

                foreach (var bystander in part.Map.mapPawns.SpawnedPawnsInFaction(part.Faction)) {
                    if ((bystander != pawn) && (bystander != part) && !xxx.is_animal(bystander)) {
                        part.needs.mood.thoughts.memories.TryGainMemory(xxx.allowed_me_to_get_raped, bystander);
                    }
                }
            }

            Log.Message("xxx::think_after_sex( " + pawn.NameStringShort + ", " + part.NameStringShort + ", " + violent + " ) - setting disease thoughts");
            // check for visible diseases
            if (xxx.is_human(pawn) && xxx.is_human(part)) {
                // Add negative relation for visible diseases on the genitals
                var pawn_rash_severity = std.genital_rash_severity(pawn) - std.genital_rash_severity(part);
                ThoughtDef pawn_thought_about_rash = null;
                if (pawn_rash_severity == 1) pawn_thought_about_rash = saw_rash_1;
                else if (pawn_rash_severity == 2) pawn_thought_about_rash = saw_rash_2;
                else if (pawn_rash_severity >= 3) pawn_thought_about_rash = saw_rash_3;
                if (pawn_thought_about_rash != null) {
                    var memory = (Thought_Memory)ThoughtMaker.MakeThought(pawn_thought_about_rash);
                    pawn.needs.mood.thoughts.memories.TryGainMemory(memory, part);
                }

                if (!part.Dead && !isCoreLovin) {
                    var part_rash_severity = std.genital_rash_severity(part) - std.genital_rash_severity(pawn);
                    ThoughtDef part_thought_about_rash = null;
                    if (part_rash_severity == 1) part_thought_about_rash = saw_rash_1;
                    else if (part_rash_severity == 2) part_thought_about_rash = saw_rash_2;
                    else if (part_rash_severity >= 3) part_thought_about_rash = saw_rash_3;
                    if (part_thought_about_rash != null)
                    {
                        var memory = (Thought_Memory)ThoughtMaker.MakeThought(part_thought_about_rash);
                        part.needs.mood.thoughts.memories.TryGainMemory(memory, pawn);
                    }
                    if (!violent)
                    {
                        var memory = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.GotSomeLovin);
                        pawn.needs.mood.thoughts.memories.TryGainMemory(memory, part);
                        part.needs.mood.thoughts.memories.TryGainMemory(memory, pawn);

                    }
                }
                
            }
        }

        // Should be called after "pawn" has fucked "partner"
        // "rapist" can be set to either "pawn" or "partner", or null if no rape occurred
        public static void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex =false) {
            var pawn_name = (pawn != null) ? pawn.NameStringShort : "NULL";
            var part_name = (pawn != null) ? part.NameStringShort : "NULL";
            Log.Message("xxx::aftersex( " + pawn_name + ", " + part_name + " ) called");
            pawn.Drawer.rotator.Face(part.DrawPos);
            pawn.Drawer.rotator.FaceCell(part.Position);

            part.Drawer.rotator.Face(pawn.DrawPos);
            part.Drawer.rotator.FaceCell(pawn.Position);

            if (violent) {
                pawn.Drawer.Notify_MeleeAttackOn(part);
            }

            if (xxx.config.sounds_enabled) {
                SoundDef.Named("Cum").PlayOneShot(new TargetInfo(part.Position, pawn.Map, false));
            }

            bool pawnIsAnimal = is_animal(pawn);
            bool partIsAnimal = is_animal(part);
            Log.Message("xxx::aftersex( " + pawn_name + ", " + part_name + " ) - applying cum effect");
            if (xxx.config.cum_enabled) {
                int pawn_cum = pawnIsAnimal ? 4 : Math.Min((int)(pawn.RaceProps.lifeExpectancy / pawn.ageTracker.AgeBiologicalYears),2);
                int part_cum = partIsAnimal ? 4 : Math.Min((int)(part.RaceProps.lifeExpectancy / part.ageTracker.AgeBiologicalYears),2);
                if (pawn.gender == Gender.Female)
                    pawn_cum /= 2;
                if (part.gender == Gender.Female)
                    part_cum /= 2;
                FilthMaker.MakeFilth(pawn.PositionHeld, pawn.MapHeld, cum, pawn.LabelIndefinite(), pawn_cum);
                if (!isCoreLovin)
                {
                    FilthMaker.MakeFilth(part.PositionHeld, part.MapHeld, cum, part.LabelIndefinite(), part_cum);
                }
            }

            Log.Message("xxx::aftersex( " + pawn_name + ", " + part_name + " ) - checking satisfaction");
            satisfy(pawn, part, violent,isCoreLovin);
            Log.Message("xxx::aftersex( " + pawn_name + ", " + part_name + " ) - checking thoughts");
            think_after_sex(pawn, part, violent, isCoreLovin);

            if (!isAnalSex)
            {
                Log.Message("xxx::aftersex( " + pawn_name + ", " + part_name + " ) - checking pregnancy");
                impregnate(pawn, part);
            }

            if (pawnIsAnimal|| partIsAnimal)
                return;
            Log.Message("xxx::aftersex( " + pawn_name + ", " + part_name + " ) - checking disease");
            std.roll_to_catch(pawn, part);
        }

        // Should be called after "necro" has fucked "corpse"
        // The necrophiliac should be set to "necro", or null if no rape occurred. The necrophiliac is assumed to be human, not animals.
        public static void aftersex(Pawn necro, Corpse corpse, bool violent = true, bool isCoreLovin = false)
        {
            var necro_name = (necro != null && !xxx.is_animal(necro)) ? necro.NameStringShort : "NULL";
            Pawn deadpawn = (corpse != null && corpse.InnerPawn != null)? corpse.InnerPawn: null;
            var corpse_name = (deadpawn != null) ? deadpawn.NameStringShort : "NULL";
            Log.Message("xxx::aftersex( " + necro_name + ", " + corpse_name + "[a deadpawn name]" + " ) called");
            necro.Drawer.rotator.Face(corpse.DrawPos);
            necro.Drawer.rotator.FaceCell(corpse.Position);
            
            /* Although violent, there's no need to attack the corpse 
            if (violent)
            {
                necro.Drawer.Notify_MeleeAttackOn(corpse);
            }
            */

            if (xxx.config.sounds_enabled)
            {
                SoundDef.Named("Cum").PlayOneShot(new TargetInfo(corpse.Position, necro.Map, false));
            }

            Log.Message("xxx::aftersex( " + necro_name + ", " + corpse_name +"[a deadpawn name]"+ " ) - applying cum effect");
            if (xxx.config.cum_enabled)
            {
                var necro_cum = (int)(necro.RaceProps.lifeExpectancy / necro.ageTracker.AgeBiologicalYears);
                FilthMaker.MakeFilth(necro.PositionHeld, necro.MapHeld, cum, necro.LabelIndefinite(), necro_cum);
            }

            Log.Message("xxx::aftersex( " + necro_name + ", " + corpse_name + "[a deadpawn name]" + " ) - checking satisfaction");
            satisfy(necro, deadpawn, violent);
            Log.Message("xxx::aftersex( " + necro_name + ", " + corpse_name + "[a deadpawn name]" + " ) - checking thoughts");
            think_after_sex(necro, deadpawn, violent);
            
            //The dead have no hediff, so no need to roll_to_catch; TO DO: add a roll_to_catch_from_corpse to std
            //Log.Message("xxx::aftersex( " + necro_name + ", " + corpse_name + "[a deadpawn name]" + " ) - checking disease");
            //std.roll_to_catch(necro, deadpawn);
        }

        public static void impregnate(Pawn pawn, Pawn part) {
            if (pawn == null || part == null) return;
            Log.Message("xxx::impregnate( " + pawn.NameStringShort + ", " + part.NameStringShort + " ) called");
            
            if (pawn.health.hediffSet.HasHediff(sterilized) || part.health.hediffSet.HasHediff(sterilized))
                return;
            if (pawn.health.capacities.GetLevel(reproduction)<=0 || part.health.capacities.GetLevel(reproduction) <= 0)
                return;

            Pawn male, female;
            if (pawn.gender == Gender.Male && part.gender == Gender.Female) {
                male = pawn;
                female = part;
            } else if (pawn.gender == Gender.Female && part.gender == Gender.Male) {
                male = part;
                female = pawn;
            } else {
                Log.Message("[RJW] Same sex pregnancies not currently supported...");
                return;
            }

            if (!xxx.is_animal(female) && !xxx.is_animal(male)) {
                if (RimWorldChildrenIsActive){
                    try
                    {
                        TryImpregnate_RimWorldChildren(female, male);
                        return;
                    }
                    catch (System.TypeLoadException)
                    {
                        Log.Message("[RJW] RimWorldChildren does not appear to be available, reverting to default pregnancy method");
                    }
                }
            } else {
                Log.Message("[RJW] Can't use RimWorldChildren for hybrid pregnancies, using default");
            }

            // fertility check
            float fertility = (xxx.is_animal(female)? HugsLibInj.pregnancy_coefficient_animals / 100f : HugsLibInj.pregnancy_coefficient_human / 100f);
            float ReproductionFactor = Math.Min(male.health.capacities.GetLevel(reproduction), female.health.capacities.GetLevel(reproduction));
            float pregnancy_threshold = fertility * ReproductionFactor;
            float pregnancy_chance = Rand.Value;

            if (pregnancy_chance > pregnancy_threshold) {
                Log.Message("[RJW] Impregnation failed. Chance was " + pregnancy_chance + " vs " + pregnancy_threshold);
                return;
            }

            Hediff_Pregnant hediff_pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDef.Named("Pregnant"), female);
            hediff_pregnant.father = male;
            female.health.AddHediff(hediff_pregnant);
            Log.Message("[RJW] Impregnation succeeded. Chance was " + pregnancy_chance + " vs " + pregnancy_threshold);

        }

        public static void TryImpregnate_RimWorldChildren(Pawn female, Pawn male) {
            // fertility check
            float fertility = (xxx.is_animal(female) ? HugsLibInj.pregnancy_coefficient_animals / 100f : HugsLibInj.pregnancy_coefficient_human / 100f);
            float ReproductionFactor = Math.Min(male.health.capacities.GetLevel(reproduction), female.health.capacities.GetLevel(reproduction));
            float pregnancy_threshold = fertility * ReproductionFactor;
            float pregnancy_chance = Rand.Value;

            BodyPartRecord torso = female.RaceProps.body.AllParts.Find(x => x.def == BodyPartDefOf.Torso);
            var human_pregnancy = DefDatabase<HediffDef>.GetNamed("HumanPregnancy", false);
            if (female.health.hediffSet.HasHediff(HediffDefOf.Pregnant) || female.health.hediffSet.HasHediff(human_pregnancy, torso)) {
                Log.Message("[RJW] C&P target is already pregnant, bailing");
                return;
            }

            var contraceptive = DefDatabase<HediffDef>.GetNamed("Contraceptive", false);
            if (female.health.hediffSet.HasHediff(contraceptive, null) || male.health.hediffSet.HasHediff(contraceptive, null)) {
                pregnancy_threshold = 0.0f;
            }

            if (pregnancy_chance > pregnancy_threshold) {
                Log.Message("[RJW] C&P impregnation failed. Chance was " + pregnancy_chance + " vs " + pregnancy_threshold);
                return;
            }

            Hediff_HumanPregnancy hediff_Pregnant = (Hediff_HumanPregnancy)HediffMaker.MakeHediff(HediffDef.Named("HumanPregnancy"), female, torso);
            hediff_Pregnant.father = male;
            female.health.AddHediff(hediff_Pregnant, torso, null);
            Log.Message("[RJW] C&P impregnation succeeded. Chance was " + pregnancy_chance + " vs " + pregnancy_threshold);

        }

        //============↓======Section of utilities of CP Rape system===============↓==================
        public static void processAnalSex(Pawn pawn, Pawn Prisoner, ref bool AnalSexSuccess,bool pawnHasPenis=true)
        {
            if (pawnHasPenis && (!is_female(Prisoner)|| Rand.Value < .33f))
            {
                Log.Message("[RJW]xxx::processAnalSex is called");
                List<RulePackDef> extraSentencePacks = new List<RulePackDef>();
                if (Genital_Helper.has_anus(Prisoner) && !Genital_Helper.anus_blocked(Prisoner))
                {
                    extraSentencePacks.Add(analSexSucceeded);
                    AnalSexSuccess = true;
                    Messages.Message("AnalSexSucceeded".Translate(new object[] { pawn.NameStringShort, Prisoner.NameStringShort }), MessageSound.Benefit);
                }
                else
                {
                    extraSentencePacks.Add(analSexFailed);
                    Messages.Message("AnalSexFailed".Translate(new object[] { pawn.NameStringShort, Prisoner.NameStringShort }), MessageSound.Silent);
                }
                PlayLogEntry_Interaction playLogEntry = new PlayLogEntry_Interaction(analSex, pawn, Prisoner, extraSentencePacks);
                Find.PlayLog.Add(playLogEntry);

            }
        }
        // Returns the designated pawn if the comfort prisoner designation is still valid
        public static Pawn check_cp_designation(Map m, Designation des)
        {
            var p = des.target.Thing as Pawn;
            //var log_msg = "check_cp_designation() - pawn.Name = " + p.Name;
            //Log.Message(log_msg);

            if ((p.Map == m) /*&& (p.IsPrisonerOfColony)*/)
                return p;
            else
                return null;
        }

        public static Pawn find_prisoner_to_rape(Pawn rapist, Map m)
        {
            List<Designation> invalid_designations = null;
            Pawn best_rapee = null;
            var best_fuckability = 0.10f; // Don't rape prisoners with <10% fuckability
            DesignationDef designation_def = xxx.config.rape_me_sticky_enabled ? comfort_prisoners.designation_def : comfort_prisoners.designation_def_no_sticky;
            foreach (var des in m.designationManager.SpawnedDesignationsOfDef(designation_def))
            {
                var q = check_cp_designation(m, des);
                if (q != null)
                {
                    if ((q != rapist) && rapist.CanReserve(q, comfort_prisoners.max_rapists_per_prisoner, 0) && (!q.Position.IsForbidden(rapist)) && is_healthy_enough(q) && can_get_raped(q))
                    {
                        if (!is_animal(q) || is_zoophiliac(rapist))
                        {
                            var fuc = would_fuck(rapist, q, true);
                            //var log_msg = rapist.Name + " -> " + q.Name + " (" + fuc.ToString() + " / " + best_fuckability.ToString() + ")";
                            //Log.Message(log_msg);

                            if (xxx.config.pawns_always_rapeCP||(fuc > best_fuckability) && (Rand.Value < fuc))
                            {
                                best_rapee = q;
                                best_fuckability = fuc;
                            }
                        }
                        
                    }
                }
                else
                {
                    if (invalid_designations == null)
                        invalid_designations = new List<Designation>();
                    invalid_designations.Add(des);
                }
            }
            if (!invalid_designations.NullOrEmpty<Designation>())
                foreach (var invalid_des in invalid_designations)
                    m.designationManager.RemoveDesignation(invalid_des);
            return best_rapee;
        }
        //============↑======Section of utilities of CP Rape system===============↑==================
        //============↓======Section of utilities of the whore system===============↓==================
        public static void FailOnWhorebedNoLongerUsable(this Toil toil, TargetIndex whorebedIndex, Building_WhoreBed whorebed)
        {
            if (toil == null)
            {
                throw new ArgumentNullException(nameof(toil));
            }

            toil.FailOnDespawnedOrNull(whorebedIndex);
            toil.FailOn(() => whorebed.IsBurning());
            toil.FailOn(() => HealthAIUtility.ShouldSeekMedicalRestUrgent(toil.actor));
            toil.FailOn(() => ((toil.actor.IsColonist && !toil.actor.CurJob.ignoreForbidden) && !toil.actor.Downed) && whorebed.IsForbidden(toil.actor));
        }

        public static Building_WhoreBed FindWhoreBed(Pawn p1)
        {
            if ((p1.ownership.OwnedBed != null) && (p1.ownership.OwnedBed is Building_WhoreBed) && (p1.ownership.OwnedBed.MaxAssignedPawnsCount > 0))
            {
                return (Building_WhoreBed)p1.ownership.OwnedBed;
            }
            return null;
        }
        /* I used the above one to find whore bed since the whores need to be assigned an whorebed to make them as whores.
        public static Building_WhoreBed FindRandomWhoreBed(Pawn pawn)
        {
            Building_WhoreBed whorebed;
            if (!(from x in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_WhoreBed>()
                  where CanUse(pawn, x)
                  select x).TryRandomElementByWeight<Building_WhoreBed>(whorebed0 => VisitChanceScore(pawn, whorebed0), out whorebed))
            {
                return null;
            }
            return whorebed;
        }
        private static float VisitChanceScore(Pawn pawn, Building_WhoreBed whorebed)
        {
            Room room = whorebed.GetRoom(RegionType.Set_Passable);
            if (room == null)
            {
                return 0f;
            }
            IntVec3 vec = pawn.Position - whorebed.Position;
            float lengthHorizontal = vec.LengthHorizontal;
            float num2 = Mathf.Clamp(GenMath.LerpDouble(0f, 150f, 1f, 0.2f, lengthHorizontal), 0.2f, 1f);
            float num3 = Mathf.Max(0f, room.GetStat(RoomStatDefOf.Beauty));
            return (num2 * num3);
        }
        */

        public static IntVec3 SleepPosOfAssignedPawn(this Building_Bed bed,Pawn pawn)
        {
            if (!bed.AssignedPawns.Contains(pawn))
            {
                Log.Error("[RJW]xxx::SleepPosOfAssignedPawn - pawn is not an owner of the bed;returning bed.position");
                return bed.Position;
            }
            else
            {
                int slotIndex = 0;
                for (byte i=0;i<bed.owners.Count;i++)
                {
                    if (bed.owners[i] == pawn)
                    {
                        slotIndex = i;
                    }
                }
                return bed.GetSleepingSlotPos(slotIndex);

            }
        }
        /*
        public static IntVec3 SleepPosOfAssignedPawn(this Building_WhoreBed bed, Pawn pawn)
        {
            if (!bed.AssignedPawns.Contains(pawn))
            {
                Log.Error("[RJW]xxx::SleepPosOfAssignedPawn - pawn is not an owner of the bed;returning bed.position");
                return bed.Position;
            }
            else
            {
                int slotIndex = 0;
                for (byte i = 0; i < bed.owners.Count; i++)
                {
                    if (bed.owners[i] == pawn)
                    {
                        slotIndex = i;
                    }
                }
                return bed.GetSleepingSlotPos(slotIndex);

            }
        }
        */
        public static bool CanUse(Pawn pawn, Building_WhoreBed whorebed)
        {
            bool flag = pawn.CanReserveAndReach(whorebed, PathEndMode.InteractionCell, Danger.Unspecified, 1) && !whorebed.IsForbidden(pawn) && whorebed.AssignedPawns.Contains(pawn);
            return flag;
        }

        public static int PriceOfWhore(Pawn whore)
        {
            float price = (whore.gender==Gender.Female) ? Rand.RangeInclusive(80,120): Rand.RangeInclusive(3, 25);
            if (!xxx.has_traits(whore))
            {
                Log.Message("[RJW] xxx::PriceOfWhore - whore has no traits");
                price /= 2;
            }
            else
            {
                if (whore.story.traits.HasTrait(TraitDefOf.Greedy))
                    price *= 2;
                if (whore.story.traits.HasTrait(TraitDefOf.Beauty))
                {
                    price *= (whore.story.traits.DegreeOfTrait(TraitDefOf.Beauty) > 0) ? 1.5f : 0;
                    price *= (whore.story.traits.DegreeOfTrait(TraitDefOf.Beauty) == 2) ? 2 : 1;
                }
                if (whore.story.traits.HasTrait(TraitDef.Named("Masochist"))){
                    price *= .95f;
                }
                if (whore.story.traits.HasTrait(TraitDef.Named("Nymphomaniac")))
                {
                    price *= .7f;
                }
            }
            if (LovePartnerRelationUtility.HasAnyLovePartner(whore))
            {
                price *= 0.8f;
            }
            float NeedSexFactor = (need_some_sex(whore)>1)? (1 - (need_some_sex(whore) / 8)):1f;
            price *= NeedSexFactor;
            Log.Message("[RJW] xxx::PriceOfWhore - price is " + price);
            return (int)Math.Round(price);
        }

        public static bool CanAfford(Pawn targetPawn, Pawn whore, int priceOfWhore=-1)
        {
            if (targetPawn.Faction != whore.Faction)
            {
                int price = priceOfWhore<0? PriceOfWhore(whore): priceOfWhore;
                if (price == 0)
                    return true;

                IEnumerable<Pawn> caravanAnimals = targetPawn.Map.mapPawns.PawnsInFaction(targetPawn.Faction).Where(x => is_animal(x) && x.inventory != null && x.inventory.innerContainer != null && x.inventory.innerContainer.TotalStackCountOfDef(ThingDefOf.Silver) > 0);
                if (caravanAnimals == null)
                {
                    int totalAmountOfSilvers = targetPawn.inventory.innerContainer.TotalStackCountOfDef(ThingDefOf.Silver);
                    Log.Message("[RJW]CanAfford::(" + targetPawn.NameStringShort + "," + whore.NameStringShort + ") - totalAmountOfSilvers is " + totalAmountOfSilvers);
                    return totalAmountOfSilvers >= price;
                }
                else 
                {
                    int totalAmountOfSilvers = 0;
                    foreach (Pawn animal in caravanAnimals)
                    {
                        totalAmountOfSilvers += animal.inventory.innerContainer.TotalStackCountOfDef(ThingDefOf.Silver);
                    }
                    if (totalAmountOfSilvers >= price)
                    {
                        Log.Message("[RJW]CanAfford:: caravan can afford the price");
                        return true;
                    }
                    Log.Message("[RJW]CanAfford:: caravan cannot afford the price");
                    return false;
                }
                
            }
            else return true;
        }

        //priceOfWhore is assumed >=0, and targetPawn is assumed to be able to pay the price(either by caravan, or by himself)
        //This means that targetPawn has total stackcount of silvers >= priceOfWhore. 
        public static bool PayPriceToWhore(Pawn targetPawn, int priceOfWhore, Pawn whore)
        {
            if (targetPawn.Faction == whore.Faction||priceOfWhore==0)
            {
                Log.Message("[RJW] xxx::PayPriceToWhore - No need to pay price");
                return true;
            }
            //Caravan guestCaravan = Find.WorldObjects.Caravans.Where(x => x.Spawned && x.ContainsPawn(targetPawn) && x.Faction == targetPawn.Faction && !x.IsPlayerControlled).FirstOrDefault();
            IEnumerable<Pawn> caravanAnimals = targetPawn.Map.mapPawns.PawnsInFaction(targetPawn.Faction).Where(x => is_animal(x) && x.inventory != null && x.inventory.innerContainer != null && x.inventory.innerContainer.TotalStackCountOfDef(ThingDefOf.Silver) > 0);

            IEnumerable<Thing> TraderSilvers;
            int AmountLeft = priceOfWhore;
            if (caravanAnimals == null)
            {
                TraderSilvers = targetPawn.inventory.innerContainer.Where(x => x.def == ThingDefOf.Silver);
                foreach (Thing silver in TraderSilvers)
                {
                    if (AmountLeft <= 0)
                        return true;
                    int dropAmount = silver.stackCount >= AmountLeft ? AmountLeft : silver.stackCount;
                    if (targetPawn.inventory.innerContainer.TryDrop(silver, whore.Position, whore.Map, ThingPlaceMode.Near, dropAmount, out Thing resultingSilvers))
                    {
                        if (resultingSilvers is null)
                        {
                            Log.Message("[RJW] xxx::PayPriceToWhore - silvers is null0");
                            return false;
                        }
                        AmountLeft -= resultingSilvers.stackCount;
                        if (AmountLeft <= 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        Log.Message("[RJW] xxx::PayPriceToWhore - TryDrop failed0");
                        return false;
                    }
                }
                return AmountLeft <= 0;
            }
            else
            {
                foreach (Pawn animal in caravanAnimals)
                {
                    TraderSilvers = animal.inventory.innerContainer.Where(x => x.def == ThingDefOf.Silver);
                    foreach (Thing silver in TraderSilvers)
                    {
                        if (AmountLeft <= 0)
                            return true;
                        int dropAmount = silver.stackCount >= AmountLeft ? AmountLeft : silver.stackCount;
                        if (animal.inventory.innerContainer.TryDrop(silver, whore.Position, whore.Map, ThingPlaceMode.Near, dropAmount, out Thing resultingSilvers))
                        {
                            if (resultingSilvers is null)
                            {
                                Log.Message("[RJW] xxx::PayPriceToWhore - silvers is null1");
                                return false;
                            }
                            AmountLeft -= resultingSilvers.stackCount;
                            if (AmountLeft <= 0)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            Log.Message("[RJW] xxx::PayPriceToWhore - TryDrop failed1");
                            continue;
                        }
                    }
                }
                return AmountLeft <= 0;
            }
        }

        public static bool IsTargetPawnOkay(Pawn p)
        {
            return (xxx.is_healthy(p) && !p.Downed);
        }

        public static bool IsHookupAppealing(Pawn pSubject, Pawn pObject)
        {
            if (PawnUtility.WillSoonHaveBasicNeed(pSubject))
            {
                return false;
            }
            float num = 0f;
            if (xxx.need_some_sex(pSubject) > 1)
            {
                num += 0.5f;
            }
            num += pSubject.relations.SecondaryRomanceChanceFactor(pObject) / 1.5f;
            num *= Mathf.InverseLerp(-100f, 0f, pSubject.relations.OpinionOf(pObject));
            return (Rand.Range(0.05f, 1f) < num);
        }

        public static bool WillPawnTryHookup(Pawn p1)
        {
            if (xxx.RomanceDiversifiedIsActive && p1.story.traits.HasTrait(xxx.asexual))
            {
                return false;
            }
            Pawn other = LovePartnerRelationUtility.ExistingMostLikedLovePartner(p1, false);
            if (other == null)
            {
                return true;
            }
            float num = p1.relations.OpinionOf(other);
            float num2 = 0f;
            if (xxx.RomanceDiversifiedIsActive && p1.story.traits.HasTrait(xxx.philanderer))
            {
                if (p1.Map == other.Map)
                {
                    num2 = Mathf.InverseLerp(70f, 15f, num);
                }
                else
                {
                    num2 = Mathf.InverseLerp(100f, 50f, num);
                }
            }
            else
            {
                num2 = Mathf.InverseLerp(30f, -80f, num);
            }
            if (xxx.RomanceDiversifiedIsActive && p1.story.traits.HasTrait(xxx.faithful))
            {
                num2 = 0f;
            }
            if (need_some_sex(p1) > 1)
            {
                num2 *= 1.4f;
            }
            num2 /= 1.5f;
            return (Rand.Range(0f, 1f) < num2);
        }
        //===========↑=======Section of utilities of the whore system====================↑=============
        //============↓======Section of Building_WhoreBed system===============↓=============
        public static void Swap(ref Building_Bed bed)
        {
            Building_Bed newBed;
            if (bed is Building_WhoreBed)
            {
                newBed = (Building_Bed)MakeBed(bed, bed.def.defName.Split(new[] { "Whore" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            else
            {
                newBed = (Building_WhoreBed)MakeBed(bed, bed.def.defName + "Whore");
            }
            newBed.SetFactionDirect(bed.Faction);
            Map m = Find.VisibleMap;
            if (!(bed.Map is null))
            {
                m = bed.Map;
                Log.Message("[RJW]xxx::Swap - before GenSpawn.Spawn is called - bed.Map is not null");
            }
            var spawnedBed = (Building_Bed)GenSpawn.Spawn(newBed, bed.Position, m, bed.Rotation);
            Log.Message("[RJW]xxx::Swap - after GenSpawn.Spawn is called");
            spawnedBed.HitPoints = bed.HitPoints;
            spawnedBed.ForPrisoners = bed.ForPrisoners;


            var compQuality = spawnedBed.TryGetComp<CompQuality>();

            if (compQuality != null) compQuality.SetQuality(bed.GetComp<CompQuality>().Quality, ArtGenerationContext.Outsider);
            var compArt = bed.TryGetComp<CompArt>();
            if (compArt != null)
            {
                var art = spawnedBed.GetComp<CompArt>();
                Log.Message("xxx::Swap(Building_Bed bed) - Calling inside the compArt part");
                art.Initialize(compArt.Props);
                //    Traverse.Create(art).Field("authorNameInt").SetValue(Traverse.Create(compArt).Field("authorNameInt").GetValue());
                //    Traverse.Create(art).Field("titleInt").SetValue(Traverse.Create(compArt).Field("titleInt").GetValue());
                //    Traverse.Create(art).Field("taleRef").SetValue(Traverse.Create(compArt).Field("taleRef").GetValue());
                //
                //    // TODO: Make this work, art is now destroyed
            }
            Find.Selector.Select(spawnedBed, false);
        }

        private static Thing MakeBed(Building_Bed bed, string defName)
        {
            ThingDef newDef = DefDatabase<ThingDef>.GetNamed(defName);
            return ThingMaker.MakeThing(newDef, bed.Stuff);
        }
        //===========↑=======Section of Building_WhoreBed system====================↑=============
        //============↓======Section of processing the broken body system===============↓=============
        public static bool BodyIsBroken(Pawn p)
        {
            return p.health.hediffSet.HasHediff(feelingBroken);
        }

        public static void processBrokenBody(Pawn p)
        {
            if (p is null)
            {
                Log.Error("xxx::processBrokenBody - p is null");
                return;
            }
            p.records.Increment(GetRapedAsComfortPrisoner);
            if (is_human(p)&& !p.Dead && p.records!=null)
            {
                float numberOfRapesSuffered = p.records.GetValue(GetRapedAsComfortPrisoner);
                BodyPartRecord torso = p.RaceProps.body.AllParts.Find((bpr) => String.Equals(bpr.def.defName, "Torso"));
                if (torso is null)
                    return;
                if (numberOfRapesSuffered >= 10)
                {
                    if (numberOfRapesSuffered < 100)
                    {
                        if (!p.health.hediffSet.HasHediff(feelingBroken))
                        {
                            p.health.AddHediff(feelingBroken, torso);

                        }
                        else
                        {
                            p.health.hediffSet.GetFirstHediffOfDef(feelingBroken).Severity = 0.1f;
                        }

                    }
                    else if (numberOfRapesSuffered < 1000)
                    {
                            p.health.hediffSet.GetFirstHediffOfDef(feelingBroken).Severity = 0.5f;
                    }
                    else
                    {
                            p.health.hediffSet.GetFirstHediffOfDef(feelingBroken).Severity = 0.9f;
                    }
                }
                else if (p.story != null && p.story.adulthood != null && p.story.adulthood.identifier.Contains("rjw_broken"))
                {
                    for (int count = 0; count < 100; count++)
                    {
                        p.records.Increment(xxx.GetRapedAsComfortPrisoner);
                    }
                    if (!p.health.hediffSet.HasHediff(feelingBroken))
                    {
                        p.health.AddHediff(feelingBroken, torso);

                    }
                    p.health.hediffSet.GetFirstHediffOfDef(feelingBroken).Severity = 0.5f;

                }

            }
        }
        public static void ExtraSatisfyForBrokenCP(Pawn p)
        {
            if (!BodyIsBroken(p)||p.needs is null||p.needs.joy is null)
                return;
            float pawn_satisfaction = 0.2f;
            switch (p.health.hediffSet.GetFirstHediffOfDef(feelingBroken).CurStageIndex)
            {
                case 0:
                    break;
                case 1:
                    p.needs.TryGetNeed<Need_Sex>().CurLevel += pawn_satisfaction;
                    p.needs.joy.CurLevel += pawn_satisfaction * 0.50f;       // convert half of satisfaction to joy
                    break;
                case 2:
                    pawn_satisfaction *= 2f;
                    p.needs.TryGetNeed<Need_Sex>().CurLevel += pawn_satisfaction;
                    p.needs.joy.CurLevel += pawn_satisfaction * 0.50f;       // convert half of satisfaction to joy
                    break;
            }
        }
        //============↑======Section of processing the broken body system===============↑=============
    }

}
