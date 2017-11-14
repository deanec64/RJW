using System;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	// Add a fail condition to JobDriver_Lovin that prevents pawns from lovin' if they aren't physically able
	[HarmonyPatch(typeof(JobDriver_Lovin))]
	[HarmonyPatch("MakeNewToils")]
	internal static class PATCH_JobDriver_Lovin_MakeNewToils
	{
		[HarmonyPrefix]
		private static bool on_begin_lovin(JobDriver_Lovin __instance)
		{
			//Log.Message("[RJW]patches_lovin::PATCH_JobDriver_Lovin_MakeNewToils is called0");
			//if (__instance == null) return true;
			__instance.FailOn(() => (!xxx.can_fuck(__instance.pawn)));
			return true;
		}
	}

	//JobDriver_DoLovinCasual from RomanceDiversified should have handled whether pawns can do casual lovin,
	//so I don't bothered to do a check here,unless some bugs occur due to this.

	// Call xxx.aftersex after pawns have finished lovin'
	// You might be thinking, "wouldn't it be easier to add this code as a finish condition to JobDriver_Lovin in the patch above?" I tried that
	// at first but it didn't work because the finish condition is always called regardless of how the job ends (i.e. if it's interrupted or not)
	// and there's no way to find out from within the finish condition how the job ended. I want to make sure not apply the effects of sex if the
	// job was interrupted somehow.
	[HarmonyPatch(typeof(JobDriver))]
	[HarmonyPatch("Cleanup")]
	internal static class PATCH_JobDriver_Cleanup
	{
		private readonly static Type JobDriverDoLovinCasual = AccessTools.TypeByName("JobDriver_DoLovinCasual");

		private static Pawn find_partner(JobDriver_Lovin lov)
		{
			var any_ins = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return (Pawn)(typeof(JobDriver_Lovin).GetProperty("Partner", any_ins).GetValue(lov, null));
		}

		[HarmonyPrefix]
		private static bool on_cleanup_driver(JobDriver __instance, JobCondition condition)
		{
			if (__instance == null) return true;
			var lov = __instance as JobDriver_Lovin;
			//Edited by nizhuan-jjr: The PostFix method cannot do this.
			if ((lov != null) && (condition == JobCondition.Succeeded))
			{
				//--Log.Message("[RJW]patches_lovin::on_cleanup_driver is called0");
				var par = find_partner(lov);
				xxx.aftersex(lov.pawn, par, false, true); // note that JobDriver_Lovin will be called for both pawns
														  //lov.pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(lov.pawn);
														  //if (par != null)
														  //    par.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(par);
			}
			else if (xxx.RomanceDiversifiedIsActive && condition == JobCondition.Succeeded && __instance.GetType() == JobDriverDoLovinCasual)
			{
				//--Log.Message("[RJW]patches_lovin::on_cleanup_driver is called1");
				var any_ins = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				var casuallovin_par = (Pawn)(__instance.GetType().GetProperty("Partner", any_ins).GetValue(__instance, null));
				if (casuallovin_par != null)
				{
					xxx.aftersex(__instance.pawn, casuallovin_par, false, true); // note that JobDriver_DoLovinCasual will be called for both pawns
																				 //lov.pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(lov.pawn);
																				 //casuallovin_par.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin(casuallovin_par);
				}
			}
			return true;
		}
	}
}