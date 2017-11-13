using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Harmony;
using Verse;
using Verse.AI;
using RimWorld;
using RimWorld.Planet;


namespace rjw
{

	[HarmonyPatch(typeof(Hediff_Pregnant), "DoBirthSpawn")]
	static class PATCH_Hediff_Pregnant_DoBirthSpawn
	{
		[HarmonyPrefix]
		static bool on_begin_DoBirthSpawn(ref Pawn mother, ref Pawn father)
		{
			//TODO: Set pregnant hediff to torso
			//Log.Message("patches_pregnancy::PATCH_Hediff_Pregnant::DoBirthSpawn() called");
			var mother_name = (mother != null) ? mother.NameStringShort : "NULL";
			var father_name = (father != null) ? father.NameStringShort : "NULL";

			if (mother == null)
			{
				Log.Error("Hediff_Pregnant::DoBirthSpawn() - no mother defined");
				return false;
			}

			if (father == null)
			{
				Log.Warning("Hediff_Pregnant::DoBirthSpawn() - no father defined");
			}
			// get a reference to the hediff we are applying
			Hediff_Pregnant self = (Hediff_Pregnant)mother.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Pregnant"));

			// determine litter size
			int litter_size = (mother.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(mother.RaceProps.litterSizeCurve, 300));
			if (litter_size < 1)
			{
				litter_size = 1;
			}
			float skin_whiteness = Rand.Range(0, 1);
			string last_name = null;

			// send a message about giving birth
			//Log.Message("Hediff_Pregnancy::DoBirthSpawn( " + mother_name + ", " + father_name + ", " + chance_successful + " ) - generating baby pawns");
			if (self.Visible && PawnUtility.ShouldSendNotificationAbout(mother))
			{
				Messages.Message("GivingBirth".Translate(new object[] { mother.LabelIndefinite() }).CapitalizeFirst(), mother, MessageSound.Standard);
			}

			//Log.Message("Hediff_Pregnancy::DoBirthSpawn( " + mother_name + ", " + father_name + ", " + chance_successful + " ) - creating spawn request");


			List<Pawn> siblings = new List<Pawn>();
			for (int i = 0; i < litter_size; i++)
			{
				Pawn spawn_parent = mother;
				if (father != null && Mod_Settings.pregnancy_use_parent_method && (100 * Rand.Value) > Mod_Settings.pregnancy_weight_parent)
				{
					spawn_parent = father;
				}
				PawnGenerationRequest request = new PawnGenerationRequest(spawn_parent.kindDef, spawn_parent.Faction, PawnGenerationContext.NonPlayer, spawn_parent.Map.Tile, false, true, false, false, false, false, 1, false, true, true, false, false, null, 0, 0, null, skin_whiteness, last_name);

				//Log.Message("Hediff_GenericPregnancy::DoBirthSpawn( " + mother_name + ", " + father_name + ", " + chance_successful + " ) - spawning baby");
				Pawn baby = PawnGenerator.GeneratePawn(request);

				if (PawnUtility.TrySpawnHatchedOrBornPawn(baby, mother))
				{
					if (baby.playerSettings != null && mother.playerSettings != null)
					{
						baby.playerSettings.AreaRestriction = mother.playerSettings.AreaRestriction;
					}
					if (baby.RaceProps.IsFlesh)
					{
						baby.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
						if (father != null)
						{
							baby.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
						}

						foreach (Pawn sibling in siblings)
						{
							baby.relations.AddDirectRelation(PawnRelationDefOf.Sibling, sibling);
						}
						siblings.Add(baby);

						//inject RJW_BabyState to the newborn if RimWorldChildren is not active
						if (!xxx.RimWorldChildrenIsActive && baby.kindDef.race == ThingDefOf.Human && baby.ageTracker.CurLifeStageIndex <= 1 && baby.ageTracker.AgeBiologicalYears < 1 && !baby.Dead)
						{
							// Clean out drug randomly generated drug addictions
							baby.health.hediffSet.Clear();
							baby.health.AddHediff(HediffDef.Named("RJW_BabyState"), null, null);
							Hediff_SimpleBaby babystate = (Hediff_SimpleBaby)baby.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RJW_BabyState"));
							if (babystate != null)
							{
								babystate.GrowUpTo(0, true);
							}
						}
					}
				}
				else
				{
					Find.WorldPawns.PassToWorld(baby, PawnDiscardDecideMode.Discard);
				}
			}

			//Log.Message("Hediff_Pregnancy::DoBirthSpawn( " + mother_name + ", " + father_name + ", " + chance_successful + " ) - removing pregnancy");
			mother.health.RemoveHediff(self);

			return false;
		}
	}

	/*
    [HarmonyPatch(typeof(Hediff_Pregnant), "Tick")]
    class PATCH_Hediff_Pregnant_Tick {
        [HarmonyPrefix]
        static bool on_begin_Tick( Hediff_Pregnant __instance ) {
            //if (__instance.pawn.IsHashIntervalTick(1000)) {
            //    //Log.Message("patches_pregnancy::PATCH_Hediff_Pregnant::Tick( " + __instance.pawn.NameStringShort + " ) - gestation_progress = " + __instance.GestationProgress);
            //    if (__instance.Severity < 0.95f) {
            //        __instance.Severity = 0.95f;
            //    }
            //}
            return true;
            
        }
    }
    */

	/*
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
    class PATCH_PawnRenderer_RenderPawnInternal {
        [HarmonyPrefix]
        static bool on_begin_RenderPawnInternal(PawnRenderer __instance, Vector3 rootLoc, Quaternion quat, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType = RotDrawMode.Fresh, bool portrait = false, bool headStump = false) {
            Log.Message("PATCH_PawnRenderer_RenderPawnInternal() called");

            return true;
        }
    }

    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
    class PATCH_PawnGraphicSet_ResolveAllGraphics {
        [HarmonyPrefix]
        static bool on_begin_ResolveAllGraphics(PawnGraphicSet __instance) {
            Log.Message("PATCH_PawnGraphicSet_ResolveAllGraphics::ResolveAllGraphics() called");
            if (__instance.pawn.RaceProps.Humanlike && __instance.pawn.ageTracker.CurLifeStageIndex < 4) {
                Log.Message("   " + __instance.pawn.NameStringShort + ":  humanlike = true, lifeStage = " + __instance.pawn.ageTracker.CurLifeStageIndex);
                if (__instance.nakedGraphic != null) {
                    if (__instance.nakedGraphic.drawSize != null) {
                        __instance.nakedGraphic.drawSize *= 0.5f;
                    } else {
                        Log.Message("   __instance.nakedGraphic.drawSize is null");
                    }
                } else {
                    Log.Message("   __instance.nakedGraphic is null");
                }
                if (__instance.apparelGraphics != null) {
                    Log.Message("   __instance.apparelGraphic is present");
                    
                } else {
                    Log.Message("   __instance.apparelGraphic is null");
                }
                if (__instance.rottingGraphic != null) {
                    Log.Message("   __instance.rottingGraphic is present");
                } else {
                    Log.Message("   __instance.rottingGraphic is null");
                }

                
            }

 
            if (__instance.pawn.RaceProps.Humanlike && __instance.pawn.ageTracker.CurLifeStageIndex < 4) {
                Log.Message("PATCH_PawnGraphicSet_ResolveAllGraphics::ResolveAllGraphics() - adjusting draw size for " + __instance.pawn.NameStringShort + "");
                if (__instance != null) {
                    var x = __instance.nakedGraphic.drawSize.x;
                    var y = __instance.nakedGraphic.drawSize.y;
                    Log.Message("   current size = " + x + "/" + y + ", new size = " + x * 0.5f + "/" + y * 0.5f);
                } else {
                    Log.Message("   __instance == null");
                
                }

            }

            return true;
        }
    }
    */

}
