using System;
using System.Linq;
using Verse;

using Harmony;
using RimWorld;

namespace rjw
{
	[HarmonyPatch(typeof(PawnBioAndNameGenerator), "SetBackstoryInSlot")]
	static class Patch_PawnBioAndNameGenerator_SetBackstoryInSlot  //This is used to make sure the generator will set Backstories based on the backstoryCategory of the pawns' kindDef
	{
		// Unmodified Version(from XnopeCore):
		// Prefix patch:
		// selects a backstory based on a pawn's kind rather than faction,
		// and only failing that does it select based on faction;
		// failing THAT, defaults.
		[HarmonyPrefix]
		static bool OnBegin_SetBackstoryInSlot(Pawn pawn, BackstorySlot slot, ref Backstory backstory)
		{
			if ((from kvp in BackstoryDatabase.allBackstories
				 where kvp.Value.shuffleable
					  && kvp.Value.spawnCategories.Contains(pawn.kindDef.backstoryCategory) // changed
					  && kvp.Value.slot == slot
					  && (slot != BackstorySlot.Adulthood || !kvp.Value.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables))
				 select kvp.Value).TryRandomElement(out backstory))
			{
				// Found backstory from PawnKindDef, cancelling original function.
				return false;
			}
			// Defaulting to original function.
			return true;
		}
		/* RJW Version:
        private readonly static PawnKindDef Nymph_pkd = PawnKindDef.Named("Nymph");

        [HarmonyPrefix]
        static bool OnBegin_SetBackstoryInSlot(Pawn pawn, BackstorySlot slot, ref Backstory backstory)  //This takes care of randomly spawned Nymph
        {
            if (xxx.is_female(pawn) &&  pawn.kindDef== Nymph_pkd &&
                (from kvp in BackstoryDatabase.allBackstories
                 where kvp.Value.shuffleable                                                // Since I haven't changed the shuffleable in nymph_backstories.cs, I don't use this version here.
                      && kvp.Value.spawnCategories.Contains(pawn.kindDef.backstoryCategory) // I should change the backstoryCategory of PawnKinds.xml to be rjw_nymphsCategory
                      && kvp.Value.slot == slot
                      && (slot != BackstorySlot.Adulthood || !kvp.Value.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables))
                 select kvp.Value).TryRandomElement(out backstory))
            {
                // Found backstory from PawnKindDef, cancelling original function.
                return false;
            }
            // Defaulting to original function.
            return true;
        }
        */
	}

	// This will generate backstories and traits for the pawns not spawned through the IncidentWorker_NymphJoins
	[HarmonyPatch(typeof(PawnGenerator), "GenerateNewNakedPawn")]
	static class Patch_PawnGenerator_GenerateNewNakedPawn
	{
		[HarmonyPrefix]
		static void OnBegin_GenerateNewNakedPawn(ref PawnGenerationRequest request)
		{
			//Log.Message("[RJW]Patch_PawnGenerator_GenerateNewNakedPawn::OnBegin_GenerateNewNakedPawn is called0");
			PawnGenerationRequest PGR = request;
			PawnKindDef pkd = PGR.KindDef;
			if (pkd != null && pkd.defName == "Nymph")
			{
				//Log.Message("[RJW]Patch_PawnGenerator_GenerateNewNakedPawn::OnBegin_GenerateNewNakedPawn is called1");
				if (pkd.minGenerationAge != 20)
				{
					Log.Message("[RJW]Patch_PawnGenerator_GenerateNewNakedPawn::OnBegin_GenerateNewNakedPawn is called2");
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
												 c => (c.story.bodyType == BodyType.Female) || (c.story.bodyType == BodyType.Thin), // Validator
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
	static class Patch_PawnGenerator_GenerateTraits
	{
		[HarmonyPostfix]
		static void After_GenerateTraits(Pawn pawn, PawnGenerationRequest request)
		{
			if (request.KindDef != null && request.KindDef.defName == "Nymph" && request.Faction != Faction.OfPlayer)
			{
				nymph_generator.set_story(pawn);

			}
		}
	}

}