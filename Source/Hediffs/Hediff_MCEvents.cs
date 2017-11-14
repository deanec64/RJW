﻿using System.Linq;
using Verse;

namespace rjw
{
	internal class Hediff_Orgasm : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			Messages.Message("FeltOrgasm".Translate(new object[] { this.pawn.LabelIndefinite() }).CapitalizeFirst(), pawn, MessageSound.Standard);
		}
	}

	internal class Hediff_TransportCums : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			if (pawn.gender == Gender.Female)
			{
				Messages.Message("CumsTransported".Translate(new object[] { this.pawn.LabelIndefinite() }).CapitalizeFirst(), pawn, MessageSound.Standard);
				Pawn cumSender = (from p in Find.WorldPawns.AllPawnsAlive where p.gender == Gender.Male select p).RandomElement<Pawn>();
				Log.Message("[RJW]" + this.GetType().ToString() + "PostAdd() - Sending " + cumSender.NameStringShort + "'s cum into " + pawn.NameStringShort + "'s vagina");
				xxx.impregnate(pawn, cumSender);
			}
			pawn.health.RemoveHediff(this);
		}
	}
}