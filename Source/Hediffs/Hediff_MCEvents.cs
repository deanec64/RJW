using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace rjw
{
	internal class Hediff_Orgasm : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			Messages.Message("FeltOrgasm".Translate(new object[] { this.pawn.LabelIndefinite() }).CapitalizeFirst(), pawn, MessageTypeDefOf.NeutralEvent);
		}
	}

	internal class Hediff_TransportCums : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			if (pawn.gender == Gender.Female)
			{
				Messages.Message("CumsTransported".Translate(new object[] { this.pawn.LabelIndefinite() }).CapitalizeFirst(), pawn, MessageTypeDefOf.NeutralEvent);
				PawnGenerationRequest req = new PawnGenerationRequest(PawnKindDefOf.Drifter,Faction.OfSpacer,fixedGender:Gender.Male );
				Pawn cumSender = PawnGenerator.GeneratePawn(req);
				Find.WorldPawns.PassToWorld(cumSender);
				//Pawn cumSender = (from p in Find.WorldPawns.AllPawnsAlive where p.gender == Gender.Male select p).RandomElement<Pawn>();
				//--Log.Message("[RJW]" + this.GetType().ToString() + "PostAdd() - Sending " + cumSender.NameStringShort + "'s cum into " + pawn.NameStringShort + "'s vagina");
				xxx.impregnate(pawn, cumSender);
			}
			pawn.health.RemoveHediff(this);
		}
	}
}