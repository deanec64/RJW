using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobGiver_WhoreInvitingVisitors : ThinkNode_JobGiver
	{
		public static int percentRate = 100;

		private static bool Roll_to_skip(Pawn p, Pawn whore)
		{
			float fuckability = xxx.would_fuck(p, whore); // 0.0 to 1.
			return xxx.config.whores_always_findjob || (fuckability >= 0.1f && Rand.Value <= 0.75f);
		}

		/*
        public static Pawn Find_pawn_to_fuck(Pawn whore, Map map)
        {
            Pawn best_fuckee = null;
            float best_distance = 1.0e6f;
            foreach (Pawn q in map.mapPawns.FreeColonists)
                if ((q != whore) &&
                    xxx.need_some_sex(q)>0 &&
                    whore.CanReserve(q, 1, 0) &&
                    q.CanReserve(whore, 1, 0) &&
                    Roll_to_skip(whore, q) &&
                    (!q.Position.IsForbidden(whore)) &&
                    xxx.is_healthy(q))
                {
                    var dis = whore.Position.DistanceToSquared(q.Position);
                    if (dis < best_distance)
                    {
                        best_fuckee = q;
                        best_distance = dis;
                    }
                }
            return best_fuckee;
        }
        */

		private sealed class FindAttractivePawnHelper
		{
			internal Pawn p1;

			internal bool TraitCheckFail(Pawn p)
			{
				if (!xxx.has_traits(p))
					return true;
				if (!xxx.can_fuck(p) || !xxx.is_healthy(p))
					return true;
				bool result = false;
				if (xxx.RomanceDiversifiedIsActive)
				{
					result = p.story.traits.HasTrait(xxx.asexual) ||
						((p.story.traits.HasTrait(xxx.straight) || p1.story.traits.HasTrait(xxx.straight))
						&& (p.gender == p1.gender));
				}
				if ((p.story.traits.HasTrait(TraitDefOf.Gay) || p1.story.traits.HasTrait(TraitDefOf.Gay)) && (p.gender != p1.gender))
				{
					return true;
				}
				return result;
			}

			//Use this check when p is not in the same faction as the whore
			internal bool FactionCheckPass(Pawn p)
			{
				return ((p.Map == p1.Map) && (p.Faction != null && p.Faction != p1.Faction) && !p.IsPrisoner && !p.HostileTo(p1));
			}

			//Use this check when p is in the same faction as the whore
			internal bool RelationCheckPass(Pawn p)
			{
				if (xxx.isSingleOrPartnerNotHere(p) || (Rand.Value < 0.9f))
				{
					if (p != LovePartnerRelationUtility.ExistingLovePartner(p1))
					{
						return (p != p1) & (p.Map == p1.Map) && (p.Faction == p1.Faction) && p.IsColonist && xxx.IsHookupAppealing(p1, p);
					}
				}
				return false;
			}
		}

		public static Pawn FindAttractivePawn(Pawn p1, out int price)
		{
			price = 0;
			if (p1 == null)
			{
				//--Log.Message("[RJW] JobGiver_WhoreInvitingVisitors::FindAttractivePawn - p1 is null");
				return null;
			}
			FindAttractivePawnHelper findPawnHelper = new FindAttractivePawnHelper
			{
				p1 = p1
			};
			if (xxx.RomanceDiversifiedIsActive && findPawnHelper.p1.story.traits.HasTrait(xxx.asexual))
			{
				return null;
			}
			price = xxx.PriceOfWhore(p1);
			int priceOfWhore = price;
			IEnumerable<Pawn> guestsSpawned = p1.Map.mapPawns.AllPawnsSpawned;
			guestsSpawned = (guestsSpawned.Except(guestsSpawned.Where(findPawnHelper.TraitCheckFail)).Where(findPawnHelper.FactionCheckPass));
			if (guestsSpawned.Count() > 0)
				guestsSpawned = guestsSpawned.Where((x => !x.Position.IsForbidden(p1) && (!LovePartnerRelationUtility.HasAnyLovePartner(x) || x != LovePartnerRelationUtility.ExistingLovePartner(p1)) && Roll_to_skip(x, p1) && xxx.CanAfford(x, p1, priceOfWhore)));
			Pawn result = null;
			if (guestsSpawned.Count() > 0)
			{
				guestsSpawned.TryRandomElement(out result);
			}
			if (result != null)
			{
				return result;
			}
			//--Log.Message("[RJW] JobGiver_WhoreInvitingVisitors::FindAttractivePawn - found no visitors");
			if (!xxx.WillPawnTryHookup(p1))
			{
				return null;
			}
			result = null;
			IEnumerable<Pawn> freeColonistsSpawned = findPawnHelper.p1.Map.mapPawns.FreeColonistsSpawned;
			freeColonistsSpawned = (freeColonistsSpawned.Except(freeColonistsSpawned.Where(findPawnHelper.TraitCheckFail)).Where(x => findPawnHelper.RelationCheckPass(x) && !x.Position.IsForbidden(p1) && Roll_to_skip(x, p1)));
			if (freeColonistsSpawned == null || freeColonistsSpawned.Count() == 0)
			{
				return null;
			}
			freeColonistsSpawned.TryRandomElement(out result);
			if (result == null)
			{
				return null;
			}
			if (p1.relations.SecondaryRomanceChanceFactor(result) < 0.05f)
			{
				return null;
			}
			return result;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			//Log.Message("[RJW] JobGiver_WhoreInvitingVisitors::TryGiveJob( " + pawn.NameStringShort + " ) called0");
			if (pawn == null || !InteractionUtility.CanInitiateInteraction(pawn))
			{
				return null;
			}
			if (PawnUtility.WillSoonHaveBasicNeed(pawn) || !xxx.is_healthy(pawn) || !xxx.can_be_fucked(pawn)) //As long as pawns is older than minimum sex age, they can be assigned as whores.
			{
				return null;
			}
			if (Find.TickManager.TicksGame >= pawn.mindState.canLovinTick && pawn.CurJob == null)
			{
				int price;
				Pawn pawn2 = FindAttractivePawn(pawn, out price);
				//--Log.Message("[RJW] JobGiver_WhoreInvitingVisitors::TryGiveJob( " + pawn.NameStringShort + " ) called1 - pawn2 is " + (pawn2 == null ? "NULL" : pawn2.NameStringShort));
				if (pawn2 == null)
				{
					return null;
				}
				Building_WhoreBed whorebed = xxx.FindWhoreBed(pawn);
				if ((whorebed == null) || !xxx.CanUse(pawn, whorebed) || (100f * Rand.Value) > percentRate)
				{
					//Log.Message("resetting ticks");
					if (xxx.config.whores_always_findjob)
						pawn.mindState.canLovinTick = Find.TickManager.TicksGame + 5;
					else pawn.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
					return null;
				}
				//--Log.Message("[RJW] JobGiver_WhoreInvitingVisitors::TryGiveJob( " + pawn.NameStringShort + " ) called2 - " + pawn2.NameStringShort + " is pawn2.");
				whorebed.priceOfWhore = price;
				return new Job(xxx.inviting_visitors, pawn2, whorebed);
			}
			return null;
		}
	}
}