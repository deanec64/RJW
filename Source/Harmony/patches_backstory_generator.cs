using System.Linq;
using Harmony;
using RimWorld;
using Verse;
using System;
using System.Collections.Generic;

namespace rjw
{
	
	// This will generate backstories and traits for the pawns not spawned through the IncidentWorker_NymphJoins
	[HarmonyPatch(typeof(PawnGenerator), "GenerateNewNakedPawn")]
	internal static class Patch_PawnGenerator_GenerateNewNakedPawn
	{
		[HarmonyPrefix]
		private static void OnBegin_GenerateNewNakedPawn(ref PawnGenerationRequest request)
		{
			//--Log.Message("[RJW]Patch_PawnGenerator_GenerateNewNakedPawn::OnBegin_GenerateNewNakedPawn is called0");
			PawnGenerationRequest PGR = request;
			PawnKindDef pkd = PGR.KindDef;
			if (pkd != null && pkd.defName == "Nymph")
			{
				//--Log.Message("[RJW]Patch_PawnGenerator_GenerateNewNakedPawn::OnBegin_GenerateNewNakedPawn is called1");
				if (pkd.minGenerationAge != 20)
				{
					//--Log.Message("[RJW]Patch_PawnGenerator_GenerateNewNakedPawn::OnBegin_GenerateNewNakedPawn is called2");
					pkd.minGenerationAge = 20;
					pkd.maxGenerationAge = 27;
					PGR = new PawnGenerationRequest(pkd,
												 PGR.Faction,
												 PGR.Context,
												 PGR.Tile,    // tile(default is -1)
												 PGR.ForceGenerateNewPawn, // Force generate new pawn
												 PGR.Newborn, // Newborn
												 PGR.AllowDead, // Allow dead
												 PGR.AllowDowned, // Allow downed
												 PGR.CanGeneratePawnRelations, // Can generate pawn relations
												 PGR.MustBeCapableOfViolence, // Must be capable of violence
												 PGR.ColonistRelationChanceFactor, // Colonist relation chance factor
												 PGR.ForceAddFreeWarmLayerIfNeeded, // Force add free warm layer if needed
												 PGR.AllowGay, // Allow gay
												 PGR.AllowFood, // Allow food
												 PGR.Inhabitant, // Inhabitant
												 PGR.CertainlyBeenInCryptosleep, // Been in Cryosleep
												 PGR.ForceRedressWorldPawnIfFormerColonist, //forceRedressWorldPawnIfFormerColonist
												 PGR.WorldPawnFactionDoesntMatter, //worldPawnFactionDoesntMatter
												 c => (c.story.bodyType == BodyType.Female) || (c.story.bodyType == BodyType.Thin), // Validator
												 PGR.MinChanceToRedressWorldPawn,
												 PGR.FixedBiologicalAge, // Fixed biological age
												 PGR.FixedChronologicalAge, // Fixed chronological age
												 Gender.Female, // Fixed gender
												 PGR.FixedMelanin, // Fixed melanin
												 PGR.FixedLastName); // Fixed last name
					request = PGR;
				}
			}
			//return;
		}
	}

	// This will generate backstories and traits for the pawns not spawned through the IncidentWorker_NymphJoins
	[HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
	internal static class Patch_PawnGenerator_GenerateTraits
	{
		[HarmonyPostfix]
		private static void After_GenerateTraits(Pawn pawn, PawnGenerationRequest request)
		{
			if (request.KindDef != null && request.KindDef.defName == "Nymph" && request.Faction != Faction.OfPlayer)
			{
				nymph_generator.set_story(pawn);
			}
		}
	}
}