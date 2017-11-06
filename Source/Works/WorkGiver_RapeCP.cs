using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace rjw
{

	public class WorkGiver_RapeCP : WorkGiver_Scanner
	{
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn target = t as Pawn;
			if ((target == null) || (target == pawn) || target.Map == null || !xxx.is_human(pawn))
			{
				return false;
			}
			if (!comfort_prisoners.is_designated(target)) return false;
			if (!xxx.is_healthy_enough(target) || target.Position.IsForbidden(pawn))
			{
				Log.Message("[RJW]WorkGiver_RapeCP::HasJobOnThing called0 - target isn't healthy enough or is in a forbidden position.");
				//if (Find.Selector.SingleSelectedThing==pawn)
				//    Messages.Message("PawnCantRapeCP".Translate(), target, MessageSound.RejectInput);
				return false;
			}
			if (comfort_prisoners.is_designated(pawn)) return false;
			if (xxx.need_some_sex(pawn) < 1 || !xxx.is_healthy(pawn) || !xxx.can_rape(pawn, xxx.has_traits(pawn) && xxx.is_nympho_or_rapist_or_zoophiliac(pawn)))
			{
				Log.Message("[RJW]WorkGiver_RapeCP::HasJobOnThing called1 - pawn don't need sex or is not healthy, or cannot rape");
				//if (Find.Selector.SingleSelectedThing == pawn)
				//    Messages.Message("PawnCantRapeCP0".Translate(), pawn, MessageSound.RejectInput);
				return false;
			}
			if (!xxx.isSingleOrPartnerNotHere(pawn))
			{
				Log.Message("[RJW]WorkGiver_RapeCP::HasJobOnThing called2 - pawn is not single or has partner around");
				//if (Find.Selector.SingleSelectedThing == pawn)
				//    Messages.Message("PawnCantRapeCP1".Translate(), pawn, MessageSound.RejectInput);
				return false;
			}
			if (xxx.is_animal(target) && !xxx.is_zoophiliac(pawn))
			{
				Log.Message("[RJW]WorkGiver_RapeCP::HasJobOnThing called3 - pawn is not zoophiliac so can't rape animal");
				//if (Find.Selector.SingleSelectedThing == pawn)
				//    Messages.Message("PawnCantRapeCP2".Translate(), pawn, MessageSound.RejectInput);
				return false;
			}
			Log.Message("[RJW]WorkGiver_RapeCP::HasJobOnThing called4");
			float fuckability = xxx.would_fuck(pawn, target, true);
			bool roll_to_skip = xxx.config.pawns_always_rapeCP ? true : fuckability >= 0.1f && Rand.Value < fuckability;

			return roll_to_skip && pawn.CanReserve(target, comfort_prisoners.max_rapists_per_prisoner, 0) && pawn.CanReach(t, PathEndMode.OnCell, Danger.Deadly);
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Log.Message("[RJW]WorkGiver_RapeCP::JobOnThing(" + pawn.NameStringShort + "," + t.ToStringSafe() + ") is called.");
			return new Job(xxx.comfort_prisoner_rapin, t);
		}


		public override int LocalRegionsToScanFirst =>
			4;

		public override Verse.AI.PathEndMode PathEndMode =>
			Verse.AI.PathEndMode.OnCell;

		public override ThingRequest PotentialWorkThingRequest =>
			ThingRequest.ForGroup(ThingRequestGroup.Pawn);
	}

}
