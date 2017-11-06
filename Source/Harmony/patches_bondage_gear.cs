﻿
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Verse;
using Verse.AI;
using RimWorld;
using RimWorld.Planet;

using Harmony;

namespace rjw
{

	[HarmonyPatch(typeof(Pawn_ApparelTracker))]
	[HarmonyPatch("Wear")]
	static class PATCH_Pawn_ApparelTracker_Wear
	{
		// Prevents pawns from equipping a piece of apparel if it conflicts with some locked apparel that they're already wearing
		[HarmonyPrefix]
		static bool prevent_wear_by_gear(Pawn_ApparelTracker __instance, ref Apparel newApparel)
		{
			var tra = __instance;
			// Log.Message ("Pawn_ApparelTracker.Wear called for " + newApparel.Label + " on " + tra.pawn.NameStringShort);
			foreach (var app in tra.WornApparel)
				if (app.has_lock() && (!ApparelUtility.CanWearTogether(newApparel.def, app.def)))
				{
					if (PawnUtility.ShouldSendNotificationAbout(tra.pawn))
						Messages.Message(tra.pawn.NameStringShort + " can't wear " + newApparel.def.label + " (blocked by " + app.def.label + ")", tra.pawn, MessageSound.Negative);
					return false;
				}
			return true;
		}

		// Add the appropriate hediff when gear is worn
		[HarmonyPostfix]
		static void on_wear(Pawn_ApparelTracker __instance, Apparel newApparel)
		{
			var tra = __instance;
			if ((newApparel.def is bondage_gear_def def) &&
				(newApparel.Wearer == tra.pawn))
			{ // Check that the apparel was actually worn
				def.soul.on_wear(tra.pawn, newApparel);
			}
		}
	}

	// Remove the hediff when the gear is removed
	[HarmonyPatch(typeof(Pawn_ApparelTracker))]
	[HarmonyPatch("Remove")]
	static class PATCH_Pawn_ApparelTracker_Remove
	{
		[HarmonyPostfix]
		static void on_remove(Pawn_ApparelTracker __instance, Apparel ap)
		{
			var tra = __instance;
			// Log.Message ("Pawn_ApparelTracker.Remove called for " + ap.Label + " on " + tra.pawn.NameStringShort);
			if (ap.def is bondage_gear_def def)
				def.soul.on_remove(ap, tra.pawn);
		}
	}

	// Patch the TryDrop method so that locked apparel cannot be dropped by JobDriver_RemoveApparel or by stripping
	// a corpse or downed pawn
	static class PATCH_Pawn_ApparelTracker_TryDrop
	{
		// Can't call MakeByRefType in an attribute, which is why this method is necessary.
		static MethodInfo get_target()
		{
			return typeof(Pawn_ApparelTracker).GetMethod("TryDrop", new Type[] {
				typeof(Apparel),
				typeof(Apparel).MakeByRefType (),
				typeof(IntVec3),
				typeof(bool)
			});
		}

		// Can't just put [HarmonyTargetMethod] on get_target, which is why this method is necessary. BTW I don't
		// know why HarmonyTargetMethod wasn't working. I'm sure I had everything set up properly so it might be a
		// bug in Harmony itself, but I can't be bothered to look into it in detail.
		public static void apply(HarmonyInstance har)
		{
			var tar = get_target();
			var pat = typeof(PATCH_Pawn_ApparelTracker_TryDrop).GetMethod("prevent_locked_apparel_drop");
			har.Patch(tar, new HarmonyMethod(pat), null);
		}

		public static bool prevent_locked_apparel_drop(Pawn_ApparelTracker __instance, ref Apparel ap, ref bool __result)
		{
			if (!ap.has_lock())
				return true;
			else
			{
				__result = false;
				return false;
			}
		}
	}

	// Prevent locked apparel from being removed from a pawn who's out on a caravan
	// TODO: Allow the apparel to be removed if the key is in the caravan's inventory
	//[HarmonyPatch (typeof (WITab_Caravan_Gear))]
	//[HarmonyPatch ("TryRemoveFromCurrentWearer")]
	//static class PATCH_WITab_Caravan_Gear_TryRemoveFromCurrentWearer {
	//	[HarmonyPrefix]
	//	static bool prevent_locked_apparel_removal (WITab_Caravan_Gear __instance, Thing t, ref bool __result)
	//	{
	//		var app = t as Apparel;
	//		if ((app == null) || (! app.has_lock ()))
	//			return true;
	//		else {
	//			__result = false;
	//			return false;
	//		}
	//	}
	//}

	// Check for the special case where a player uses the caravan gear tab to equip a piece of lockable apparel on a pawn that
	// is wearing an identical piece of already locked apparel, and make sure that doesn't work or change anything. This is to
	// fix a bug where, even though the new apparel would fail to be equipped on the pawn, it would somehow copy the holostamp
	// info from the equipped apparel.
	[HarmonyPatch(typeof(WITab_Caravan_Gear))]
	[HarmonyPatch("TryEquipDraggedItem")]
	static class PATCH_WITab_Caravan_Gear_TryEquipDraggedItem
	{
		static Thing get_dragged_item(WITab_Caravan_Gear tab)
		{
			return (Thing)typeof(WITab_Caravan_Gear).GetField("draggedItem", xxx.ins_public_or_no).GetValue(tab);
		}

		static void set_dragged_item(WITab_Caravan_Gear tab, Thing t)
		{
			typeof(WITab_Caravan_Gear).GetField("draggedItem", xxx.ins_public_or_no).SetValue(tab, t);
		}

		[HarmonyPrefix]
		static bool prevent_locked_apparel_conflict(WITab_Caravan_Gear __instance, ref Pawn p)
		{
			if (__instance == null) return true;
			if (get_dragged_item(__instance) is Apparel dragged_app && p.apparel != null && dragged_app.has_lock())
			{
				foreach (var equipped_app in p.apparel.WornApparel)
				{
					if (equipped_app.has_lock() && (!ApparelUtility.CanWearTogether(dragged_app.def, equipped_app.def)))
					{
						set_dragged_item(__instance, null);
						return false;
					}
				}
			}

			return true;
		}
	}

	// Prevent locked apparel from being removed from a corpse
	// TODO: Find out when this method is actually called (probably doesn't matter but still)
	[HarmonyPatch(typeof(Dialog_FormCaravan))]
	[HarmonyPatch("RemoveFromCorpseIfPossible")]
	static class PATCH_Dialog_FormCaravan_RemoveFromCorpseIfPossible
	{
		[HarmonyPrefix]
		static bool prevent_locked_apparel_removal(Dialog_FormCaravan __instance, Thing thing)
		{
			var app = thing as Apparel;
			return (app == null) || (!app.has_lock());
		}
	}

	// Patch a method in a pawn gear tab to give the pawn a special job driver if the player tells them to
	// drop locked apparel they have equipped
	// TODO: Search for the key and, if the pawn can access it, give them a new job that has them go to the key
	// and use it to unlock the apparel
	[HarmonyPatch(typeof(ITab_Pawn_Gear))]
	[HarmonyPatch("InterfaceDrop")]
	static class PATCH_ITab_Pawn_Gear_InterfaceDrop
	{
		static Pawn SelPawnForGear(ITab_Pawn_Gear tab)
		{
			var pro = typeof(ITab_Pawn_Gear).GetProperty("SelPawnForGear", xxx.ins_public_or_no);
			return (Pawn)pro.GetValue(tab, null);
		}

		[HarmonyPrefix]
		static bool drop_locked_apparel(ITab_Pawn_Gear __instance, Thing t)
		{
			var tab = __instance;
			var app = t as Apparel;
			if ((app == null) || (!app.has_lock()))
				return true;
			else
			{
				var p = SelPawnForGear(tab);
				if ((p.apparel != null) && (p.apparel.WornApparel.Contains(app)))
				{
					p.jobs.TryTakeOrderedJob(new Job(DefDatabase<JobDef>.GetNamed("StruggleInBondageGear"), app));
					return false;
				}
				else
					return true;
			}
		}
	}

	// Patch the method that prisoners use to find apparel in their cell so that they don't try to wear
	// something that conflicts with locked apparel. The method used to prevent pawns from trying to
	// optimize away locked apparel won't work here because prisoners don't check GetSpecialApparelScoreOffset.
	[HarmonyPatch(typeof(JobGiver_PrisonerGetDressed))]
	[HarmonyPatch("FindGarmentCoveringPart")]
	static class PATCH_JobGiver_PrisonerGetDressed_FindGarmentCoveringPart
	{
		static bool conflicts(Pawn_ApparelTracker tra, Apparel new_app)
		{
			foreach (var worn_app in tra.WornApparel)
				if (worn_app.has_lock() && (!ApparelUtility.CanWearTogether(worn_app.def, new_app.def)))
					return true;

			return false;
		}

		[HarmonyPrefix]
		static bool prevent_locked_apparel_conflict(JobGiver_PrisonerGetDressed __instance, Pawn pawn, BodyPartGroupDef bodyPartGroupDef, ref Apparel __result)
		{
			// If the prisoner is not wearing locked apparel then just run the regular method
			if (!pawn.is_wearing_locked_apparel())
				return true;

			// Otherwise, duplicate the logic in the regular method except with an additional check
			// to filter out apparel that conflicts with worn locked apparel.
			else
			{
				var room = pawn.GetRoom();
				if (room.isPrisonCell)
					foreach (var c in room.Cells)
						foreach (var t in c.GetThingList(pawn.Map))
						{
							if ((t is Apparel app) &&
								app.def.apparel.bodyPartGroups.Contains(bodyPartGroupDef) &&
								pawn.CanReserve(app) &&
								ApparelUtility.HasPartsToWear(pawn, app.def) &&
								(!conflicts(pawn.apparel, app)))
							{
								__result = app;
								return false;
							}
						}
				__result = null;
				return false;
			}
		}
	}

}
