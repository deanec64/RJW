using Harmony;
using System;
using System.Linq;
using System.Reflection;
using Verse;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using rjw;
using Verse.AI.Group;
using Verse.AI;

namespace rjw
{

	[HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
	static class Patches_ABF_PawnMakeRaper
	{
		public static void Postfix(Pawn __result, ref PawnGenerationRequest request)
		{
			if (__result == null) return;

			if (__result.RaceProps.IsMechanoid)
			{
				BodyPartRecord genitalPart = __result.RaceProps.body.AllParts.Find(bpr => bpr.def.defName == "Genitals");
				__result.health.AddHediff(HediffDef.Named("BionicPenis"), genitalPart);
			}

			if (PawnGenerationContext.NonPlayer.Includes(request.Context))
			{
				Need need = __result.needs.TryGetNeed<Need_Sex>();
				if (need != null)
				{
					//need.ForceSetLevel(Rand.Range(0f,1f));
					need.ForceSetLevel(Rand.Range(0.01f, 0.2f));
				}

			}
		}
	}

	[HarmonyPatch(typeof(LordJob_AssaultColony), "CreateGraph")]
	static class atches_ABF_AssaultColonyForRape
	{
		public static void Postfix(StateGraph __result)
		{
			//--Log.Message("[ABF]AssaultColonyForRape::CreateGraph");
			if (__result == null) return;
			foreach (var trans in __result.transitions)
			{
				if (HasDesignatedTransition(trans))
				{
					foreach (Trigger t in trans.triggers)
					{
						if (t.filters == null)
						{
							t.filters = new List<TriggerFilter>() { new Trigger_SexSatisfy(0.3f) };
						}
						else
						{
							t.filters.Add(new Trigger_SexSatisfy(0.3f));
						}
					}
					//--Log.Message("[ABF]AssaultColonyForRape::CreateGraph Adding SexSatisfyTrigger to " + trans.ToString());
				}
			}
		}
		static bool HasDesignatedTransition(Transition t)
		{
			if (t.target == null) return false;
			if (t.target.GetType() == typeof(LordToil_KidnapCover)) return true;

			foreach (Trigger ta in t.triggers)
			{
				if (ta.GetType() == typeof(Trigger_FractionColonyDamageTaken)) return true;
			}
			return false;
		}
	}


}
