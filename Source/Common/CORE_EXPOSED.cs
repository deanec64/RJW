
/*
 *
 *
 * ALL THE CODE IN THIS FILE HAS JUST BEEN COPIED FROM THE DECOMPILED CORE ASSEMBLY.
 * Except for one little thing I had to change so it would (re)compile.
 *
 *
 */

using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw_CORE_EXPOSED
{

	public static class JobDriver_Lovin
	{
		public static readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
		{
			new CurvePoint(1f,  12f),
			new CurvePoint(16f, 6f),
			new CurvePoint(22f, 9f),
			new CurvePoint(30f, 12f),
			new CurvePoint(50f, 18f),
			new CurvePoint(75f, 24f)
		};
	}

	public static class LovePartnerRelationUtility
	{
		public static float LovinMtbSinglePawnFactor(Pawn pawn)
		{
			float num = 1f;
			num /= 1f - pawn.health.hediffSet.PainTotal;
			float efficiency = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness);
			if (efficiency < 0.5f)
			{
				num /= efficiency * 2f;
			}

			if (!pawn.RaceProps.Humanlike)
			{
				return num * 4f;
			}
			if (RimWorld.LovePartnerRelationUtility.ExistingLovePartner(pawn) != null)
			{
				num *= 2f; //This is a factor which makes pawns with love partners less likely to do fappin/random raping/rapingCP/beastiality/necro.
			}
			else if (pawn.gender == Gender.Male)
			{
				num /= 1.25f; //This accounts for single men
			}
			return num / GenMath.FlatHill(0.0001f, 8f, 13f, 28f, 50f, 0.15f, pawn.ageTracker.AgeBiologicalYearsFloat);
		}
	}

	public static class MedicalRecipesUtility
	{
		public static bool IsCleanAndDroppable(Pawn pawn, BodyPartRecord part)
		{
			return !pawn.Dead && !pawn.RaceProps.Animal && part.def.spawnThingOnRemoved != null && IsClean(pawn, part);
		}

		public static bool IsClean(Pawn pawn, BodyPartRecord part)
		{
			return !pawn.Dead && !(from x in pawn.health.hediffSet.hediffs
								   where x.Part == part
								   select x).Any<Hediff>();
		}

		public static void RestorePartAndSpawnAllPreviousParts(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
		{
			SpawnNaturalPartIfClean(pawn, part, pos, map);
			SpawnThingsFromHediffs(pawn, part, pos, map);
			pawn.health.RestorePart(part, null, true);
		}

		public static Thing SpawnNaturalPartIfClean(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
		{
			if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
			{
				return GenSpawn.Spawn(part.def.spawnThingOnRemoved, pos, map);
			}
			return null;
		}

		public static void SpawnThingsFromHediffs(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
		{
			if (!pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).Contains(part))
			{
				return;
			}
			IEnumerable<Hediff> enumerable = from x in pawn.health.hediffSet.hediffs
											 where x.Part == part
											 select x;
			foreach (Hediff current in enumerable)
			{
				if (current.def.spawnThingOnRemoved != null)
				{
					GenSpawn.Spawn(current.def.spawnThingOnRemoved, pos, map);
				}
			}
			for (int i = 0; i < part.parts.Count; i++)
			{
				MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part.parts[i], pos, map);
			}
		}
	}

	public class Recipe_RemoveBodyPart : Recipe_Surgery
	{
		private const float ViolationGoodwillImpact = 20f;

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			IEnumerable<BodyPartRecord> parts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined);
			foreach (BodyPartRecord part in parts)
			{
				if (pawn.health.hediffSet.HasDirectlyAddedPartFor(part))
				{
					yield return part;
				}
				if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
				{
					yield return part;
				}
				if (part != pawn.RaceProps.body.corePart && !part.def.dontSuggestAmputation && pawn.health.hediffSet.hediffs.Any((Hediff d) => !(d is Hediff_Injury) && d.def.isBad && d.Visible && d.Part == part))
				{
					yield return part;
				}
			}
		}

		public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
		{
			return pawn.Faction != billDoerFaction && HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients)
		{
			bool flag = MedicalRecipesUtility.IsClean(pawn, part);
			bool flag2 = this.IsViolationOnPawn(pawn, part, Faction.OfPlayer);
			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
				MedicalRecipesUtility.SpawnNaturalPartIfClean(pawn, part, billDoer.Position, billDoer.Map);
				MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part, billDoer.Position, billDoer.Map);
			}
			pawn.TakeDamage(new DamageInfo(DamageDefOf.SurgicalCut, 99999, -1f, null, part, null));
			if (flag)
			{
				if (pawn.Dead)
				{
					ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.OrganHarvesting);
				}
				else
				{
					ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn);
				}
			}
			if (flag2)
			{
				pawn.Faction.AffectGoodwillWith(billDoer.Faction, -20f);
			}
		}

		public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
		{
			if (pawn.RaceProps.IsMechanoid || pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part))
			{
				return RecipeDefOf.RemoveBodyPart.LabelCap;
			}
			BodyPartRemovalIntent bodyPartRemovalIntent = HealthUtility.PartRemovalIntent(pawn, part);
			if (bodyPartRemovalIntent == BodyPartRemovalIntent.Harvest)
			{
				return "Harvest".Translate();
			}
			if (bodyPartRemovalIntent != BodyPartRemovalIntent.Amputate)
			{
				throw new InvalidOperationException();
			}
			if (part.depth == BodyPartDepth.Inside)
			{
				return "RemoveOrgan".Translate();
			}
			return "Amputate".Translate();
		}
	}

}
